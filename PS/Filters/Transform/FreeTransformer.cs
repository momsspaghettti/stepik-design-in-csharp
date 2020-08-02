using System;
using System.Drawing;

namespace MyPhotoshop {
    public class FreeTransformer : ITransformer<EmptyParameters> {
        private readonly Func<Size, Size> _sizeTransformer;
        private readonly Func<Point, Size, Point> _pointTransformer;

        public FreeTransformer(Func<Size, Size> sizeTransformer, Func<Point, Size, Point> pointTransformer) {
            _sizeTransformer = sizeTransformer;
            _pointTransformer = pointTransformer;
        }

        private Size _oldSize;

        public void Prepare(Size oldSize, EmptyParameters parameters) {
            _oldSize = oldSize;
            ResultSize = _sizeTransformer(oldSize);
        }

        public Size ResultSize { get; private set; }

        public Point? MapPoint(Point newPoint) {
            return _pointTransformer(newPoint, _oldSize);
        }
    }
}