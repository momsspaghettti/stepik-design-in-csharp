using System;
using System.Linq;
using System.Linq.Expressions;


namespace Reflection.Differentiation {
    public static class Algebra {
        public static Expression<Func<double, double>> Differentiate(Expression<Func<double, double>> function) {
            return Expression.Lambda<Func<double, double>>(
                Differentiate(function.Body),
                function.Parameters
            );
        }

        private static Expression Differentiate(Expression expression) {
            if (expression is BinaryExpression binaryExpression)
                return Differentiate(binaryExpression);
            if (expression is ConstantExpression constantExpression)
                return Differentiate(constantExpression);
            if (expression is ParameterExpression parameterExpression)
                return Differentiate(parameterExpression);
            if (expression is MethodCallExpression methodCallExpression)
                return Differentiate(methodCallExpression);

            throw new ArgumentException("Unknown type of expression!");
        }

        private static Expression Differentiate(BinaryExpression expression) {
            switch (expression.NodeType) {
                case ExpressionType.Add:
                    return Expression.Add(
                        Differentiate(expression.Left),
                        Differentiate(expression.Right)
                    );

                case ExpressionType.Subtract:
                    return Expression.Subtract(
                        Differentiate(expression.Left),
                        Differentiate(expression.Right)
                    );

                case ExpressionType.Multiply:
                    return Expression.Add(
                        Expression.Multiply(
                            Differentiate(expression.Left),
                            expression.Right
                        ),
                        Expression.Multiply(
                            expression.Left,
                            Differentiate(expression.Right)
                        )
                    );

                case ExpressionType.Divide:
                    return Expression.Divide(
                        Expression.Subtract(
                            Expression.Multiply(
                                Differentiate(expression.Left),
                                expression.Right
                            ),
                            Expression.Multiply(
                                expression.Left,
                                Differentiate(expression.Right)
                            )
                        ),
                        Expression.Power(
                            expression.Right,
                            Expression.Constant(2)
                        )
                    );
            }

            throw new ArgumentException("Unknown binary operation!");
        }

        private static Expression Differentiate(ConstantExpression expression) {
            return Expression.Constant(0.0);
        }

        private static Expression Differentiate(ParameterExpression expression) {
            return Expression.Constant(1.0);
        }

        private static Expression Differentiate(MethodCallExpression expression) {
            var param = expression.Arguments.First();
            var paramDiff = Differentiate(param);
            var method = expression.Method;

            if (method == typeof(Math).GetMethod("Sin")) {
                return Expression.Multiply(
                    paramDiff,
                    Expression.Call(
                        typeof(Math).GetMethod("Cos"),
                        param
                    )
                );
            }

            if (method == typeof(Math).GetMethod("Cos")) {
                return Expression.Multiply(
                    Expression.Constant(-1.0),
                    Expression.Multiply(
                        paramDiff,
                        Expression.Call(
                            typeof(Math).GetMethod("Sin"),
                            param
                        )
                    )
                );
            }

            throw new ArgumentException("Unknown method call!");
        }
    }
}