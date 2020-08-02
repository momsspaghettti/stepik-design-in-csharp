using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incapsulation.RationalNumbers {
    public class Rational {
        private readonly int _numerator;

        public int Numerator => _numerator;

        private readonly int _denominator;

        public int Denominator => _denominator;

        private bool _isNan;

        public bool IsNan => _isNan;

        public Rational(int num, int den) {
            _numerator = num;
            _denominator = den;

            if (_denominator < 0) {
                _denominator *= -1;
                _numerator *= -1;
            }

            if (_denominator == 0)
                _isNan = true;
            else {
                _isNan = false;
                if (_numerator != 0) {
                    var gcd = Gcd(Math.Abs(num), Math.Abs(den));
                    _numerator /= gcd;
                    _denominator /= gcd;
                }
                else
                    _denominator = 1;
            }
        }

        public Rational(int num) : this(num, 1) { }

        private static int Gcd(int a, int b) {
            if (a == b)
                return a;
            if (a > b)
                return Gcd(a - b, b);
            return Gcd(b - a, a);
        }

        public static Rational operator +(Rational l, Rational r) {
            return new Rational(
                l._numerator * r._denominator + r._numerator * l._denominator,
                l._denominator * r._denominator
            );
        }

        public static Rational operator -(Rational l, Rational r) {
            return new Rational(
                l._numerator * r._denominator - r._numerator * l._denominator,
                l._denominator * r._denominator
            );
        }

        public static Rational operator *(Rational l, Rational r) {
            return new Rational(
                l._numerator * r._numerator,
                l._denominator * r._denominator
            );
        }

        public static Rational operator /(Rational l, Rational r) {
            if (r.IsNan)
                return new Rational(0, 0);
            return new Rational(
                l._numerator * r._denominator,
                l._denominator * r._numerator
            );
        }

        public static implicit operator double(Rational r) {
            if (r.IsNan)
                return double.NaN;
            return r._numerator * 1.0 / r._denominator;
        }

        public static implicit operator Rational(int num) {
            return new Rational(num);
        }

        public static explicit operator int(Rational r) {
            if (r.IsNan)
                throw new Exception();
            if (r.Numerator % r.Denominator != 0)
                throw new Exception();
            return r.Numerator / r.Denominator;
        }
    }
}