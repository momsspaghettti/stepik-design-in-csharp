using System;

namespace MyPhotoshop {
    public class PixelFilter<TParameters> : ParametrizedFilter<TParameters>
        where TParameters : IParameters, new() {
        private readonly string _name;
        private readonly Func<Pixel, TParameters, Pixel> _processor;

        public PixelFilter(string name, Func<Pixel, TParameters, Pixel> processor) {
            _name = name;
            _processor = processor;
        }

        public override Photo Process(Photo original, TParameters parameters) {
            var result = new Photo(original.Width, original.Height);
            for (int i = 0; i < result.Width; ++i) {
                for (int j = 0; j < result.Height; ++j) {
                    result[i, j] = _processor(original[i, j], parameters);
                }
            }

            return result;
        }

        public override string ToString() {
            return _name;
        }
    }
}