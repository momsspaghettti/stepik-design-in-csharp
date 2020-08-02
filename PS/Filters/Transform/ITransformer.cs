using System.Drawing;

namespace MyPhotoshop {
    public interface ITransformer<in TParameters>
        where TParameters : IParameters, new() {
        void Prepare(Size oldSize, TParameters parameters);
        Size ResultSize { get; }
        Point? MapPoint(Point newPoint);
    }
}