using System;
using System.Drawing;

namespace MyPhotoshop {
    public class TransformFilter : TransformFilter<EmptyParameters> {
        public TransformFilter(
            string name, Func<Size, Size> sizeTransformer, Func<Point, Size, Point> pointTransformer
        ) : base(name, new FreeTransformer(sizeTransformer, pointTransformer)) { }
    }
}