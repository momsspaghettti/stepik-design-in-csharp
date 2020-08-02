using System;

namespace MyPhotoshop {
    public struct Pixel {
        public Pixel(double r, double g, double b) {
            _red = CheckValue(r, "Red");
            _green = CheckValue(g, "Green");
            _blue = CheckValue(b, "Blue");
        }

        private double _red;

        public double R {
            get => _red;
            set => _red = CheckValue(value, "Red");
        }

        private double _green;

        public double G {
            get => _green;
            set => _green = CheckValue(value, "Green");
        }

        private double _blue;

        public double B {
            get => _blue;
            set => _blue = CheckValue(value, "Blue");
        }

        private static double CheckValue(double value, string fieldName) {
            if (value > 1 || value < 0)
                throw new ArgumentException($"{fieldName} param out of range!");
            return value;
        }

        public static Pixel operator *(Pixel old, double k) {
            return new Pixel(
                k * old.R,
                k * old.G,
                k * old.B
            );
        }

        public static Pixel operator *(double k, Pixel old) {
            return old * k;
        }
    }
}