using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incapsulation.Weights {
    class Indexer {
        public readonly int Length;
        private readonly int _start;
        private readonly double[] _data;

        public Indexer(double[] data, int start, int length) {
            CheckStart(data.Length, start);
            CheckLength(data.Length, start, length);

            _data = data;
            _start = start;
            Length = length;
        }

        public double this[int index] {
            get {
                CheckIndex(index);
                return _data[_start + index];
            }
            set {
                CheckIndex(index);
                _data[_start + index] = value;
            }
        }

        private void CheckIndex(int index) {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException("CheckIndex");
        }

        private static void CheckStart(int dataLength, int start) {
            if (start < 0)
                throw new ArgumentException("CheckStart");
        }

        private static void CheckLength(int dataLength, int start, int length) {
            if (length < 0)
                throw new ArgumentException("CheckLength");
            if (start + length > dataLength)
                throw new ArgumentException("CheckLength");
        }
    }
}