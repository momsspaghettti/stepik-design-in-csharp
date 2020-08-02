using System;
using System.Drawing;
using System.Windows.Forms;

namespace MyPhotoshop {
    class MainClass {
        [STAThread]
        public static void Main(string[] args) {
            var window = new MainWindow();
            window.AddFilter(new PixelFilter<LighteningParameters>(
                "Осветление/затемнение",
                (pixel, parameters) => pixel * parameters.Coefficient
            ));

            window.AddFilter(new PixelFilter<EmptyParameters>(
                "Чёрно-белый фильтр",
                (pixel, parameters) => {
                    var lightness = 0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B;
                    return new Pixel(lightness, lightness, lightness);
                }));

            window.AddFilter(new TransformFilter(
                "Отразить по вертикали",
                size => size,
                (point, size) => new Point(point.X, size.Height - point.Y - 1)
            ));

            window.AddFilter(new TransformFilter(
                "Отразить по горизонтали",
                size => size,
                (point, size) => new Point(size.Width - point.X - 1, point.Y)
            ));

            window.AddFilter(new TransformFilter(
                "Повернуть по ч/с на 90",
                size => new Size(size.Height, size.Width),
                (point, size) => new Point(size.Width - point.Y - 1, size.Height - point.X - 1)
            ));

            window.AddFilter(new TransformFilter<RotationParameters>(
                "Повернуть изображение",
                new RotateTransformer()
            ));

            Application.Run(window);
        }
    }
}