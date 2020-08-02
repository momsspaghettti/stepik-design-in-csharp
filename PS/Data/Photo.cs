using System;

namespace MyPhotoshop {
    public class Photo {
        public readonly int Width;
        public readonly int Height;
        private readonly Pixel[,] _data;

        public Photo(int width, int height) {
            CheckConstructorParams(width, height);
            Width = width;
            Height = height;
            _data = new Pixel[width, height];
        }

        public Pixel this[int i, int j] {
            get {
                CheckIndexes(i, j);
                return _data[i, j];
            }
            set {
                CheckIndexes(i, j);
                _data[i, j] = value;
            }
        }

        private void CheckIndexes(int i, int j) {
            if (i < 0 || i >= Width)
                throw new ArgumentException("Height index out of range");
            if (j < 0 || j >= Height)
                throw new ArgumentException("Width index out of range");
        }

        private static void CheckConstructorParams(int width, int height) {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Wrong params in Photo constructor");
        }
    }
}