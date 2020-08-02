using System;
using System.Drawing;

namespace MyPhotoshop {
    public class RotateTransformer : ITransformer<RotationParameters> {
        public void Prepare(Size oldSize, RotationParameters parameters) {
            _oldSize = oldSize;
            _angle = Math.PI * parameters.Angle / 180;
            ResultSize = new Size(
                (int) (oldSize.Width * Math.Abs(Math.Cos(_angle)) + oldSize.Height * Math.Abs(Math.Sin(_angle))),
                (int) (oldSize.Height * Math.Abs(Math.Cos(_angle)) + oldSize.Width * Math.Abs(Math.Sin(_angle))));
        }

        private Size _oldSize;
        private double _angle;
        private readonly RotationParameters _rotationParameters = new RotationParameters();

        public Size ResultSize { get; private set; }

        public Point? MapPoint(Point newPoint) {
            newPoint = new Point(newPoint.X - ResultSize.Width / 2, newPoint.Y - ResultSize.Height / 2);
            var x = _oldSize.Width / 2 + (int) (newPoint.X * Math.Cos(_angle) + newPoint.Y * Math.Sin(_angle));
            var y = _oldSize.Height / 2 + (int) (-newPoint.X * Math.Sin(_angle) + newPoint.Y * Math.Cos(_angle));
            if (x < 0 || x >= _oldSize.Width || y < 0 || y >= _oldSize.Height) return null;
            return new Point(x, y);
        }
    }
}