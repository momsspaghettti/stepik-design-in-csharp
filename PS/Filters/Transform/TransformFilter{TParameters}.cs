using System;
using System.Drawing;

namespace MyPhotoshop {
    public class TransformFilter<TParameters> : ParametrizedFilter<TParameters>
        where TParameters : IParameters, new() {
        private readonly string _name;

        private ITransformer<TParameters> _transformer;

        public TransformFilter(string name, ITransformer<TParameters> transformer) {
            _name = name;
            _transformer = transformer;
        }

        public override Photo Process(Photo original, TParameters parameters) {
            var oldSize = new Size(original.Width, original.Height);
            _transformer.Prepare(oldSize, parameters);
            var result = new Photo(_transformer.ResultSize.Width, _transformer.ResultSize.Height);
            for (int x = 0; x < result.Width; ++x) {
                for (int y = 0; y < result.Height; ++y) {
                    var newPoint = new Point(x, y);
                    var originalPoint = _transformer.MapPoint(newPoint);
                    if (originalPoint.HasValue)
                        result[x, y] = original[originalPoint.Value.X, originalPoint.Value.Y];
                }
            }

            return result;
        }

        public override string ToString() {
            return _name;
        }
    }
}