using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace Reflection.Randomness {
    public class FromDistribution : Attribute {
        public IContinousDistribution Distribution { get; }

        public FromDistribution(Type type, params object[] args) {
            try {
                Distribution = (IContinousDistribution) Activator.CreateInstance(type, args);
            }
            catch (Exception) {
                throw new ArgumentException(type.Name);
            }
        }
    }

    public class Generator<T> {
        public class PropertyKeeper {
            private readonly string _propertyName;
            private readonly Generator<T> _parent;

            public PropertyKeeper(string propertyName, Generator<T> parent) {
                _propertyName = propertyName;
                _parent = parent;
            }

            public Generator<T> Set(IContinousDistribution distribution) {
                _parent._propertiesToDistribution[_propertyName] = distribution;
                _parent._needToRecompileBuildingFunc = true;
                return _parent;
            }
        }

        private readonly Dictionary<string, IContinousDistribution> _propertiesToDistribution =
            new Dictionary<string, IContinousDistribution>();

        private Func<Dictionary<string, IContinousDistribution>, Random, T> _buildingFunc;
        private bool _needToRecompileBuildingFunc;

        private void InitPropertiesDict() {
            foreach (var propertyWithCustomAttribute in typeof(T)
                .GetProperties()
                .Where(p => p.GetCustomAttribute<FromDistribution>() != null)
                .Select(p => new {p.Name, Attribute = p.GetCustomAttribute<FromDistribution>()})
            ) {
                _propertiesToDistribution[propertyWithCustomAttribute.Name] =
                    propertyWithCustomAttribute.Attribute.Distribution;
            }
        }

        private void CompileBuildingFunc() {
            // (rnd, dict) => new T{P = dict["P"].Generate(rnd), ...}
            var rnd = Expression.Parameter(typeof(Random), "rnd");
            var dict =
                Expression.Parameter(typeof(Dictionary<string, IContinousDistribution>), "dict");

            var bindings = new List<MemberBinding>();
            foreach (var property in typeof(T)
                .GetProperties()
                .Where(p => _propertiesToDistribution.ContainsKey(p.Name))
                .Select(p => new {p.Name, Info = p})
            ) {
                var dictValue = Expression.Parameter(typeof(IContinousDistribution));

                var dictAccess = Expression.Block(
                    new[] {dictValue},
                    Expression.Assign(
                        dictValue,
                        Expression.Property(
                            dict,
                            "Item",
                            Expression.Constant(property.Name)
                        )
                    )
                );

                var methodCall = Expression.Call(
                    dictAccess,
                    "Generate",
                    null,
                    rnd
                );

                var binding = Expression.Bind(
                    property.Info,
                    methodCall
                );

                bindings.Add(binding);
            }

            var body = Expression.MemberInit(
                Expression.New(typeof(T).GetConstructor(new Type[0])),
                bindings
            );

            var lambda =
                Expression.Lambda<Func<Dictionary<string, IContinousDistribution>, Random, T>>(
                    body,
                    dict,
                    rnd
                );

            _buildingFunc = lambda.Compile();
            _needToRecompileBuildingFunc = false;
        }

        public Generator() {
            InitPropertiesDict();
            CompileBuildingFunc();
        }

        public T Generate(Random rnd) {
            if (_needToRecompileBuildingFunc)
                CompileBuildingFunc();
            return _buildingFunc(_propertiesToDistribution, rnd);
        }

        public PropertyKeeper For(Expression<Func<T, object>> selector) {
            var expression = selector.Body;
            if (!(expression is UnaryExpression))
                throw new ArgumentException();

            var unaryExpression = (UnaryExpression) expression;
            if (!(unaryExpression.Operand is MemberExpression))
                throw new ArgumentException();

            var memberExpression = (MemberExpression) unaryExpression.Operand;
            if (memberExpression.Expression.NodeType != ExpressionType.Parameter)
                throw new ArgumentException();

            return new PropertyKeeper(memberExpression.Member.Name, this);
        }
    }
}