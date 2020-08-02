using System;
using System.Collections.Generic;
using System.Linq;


namespace Delegates.PairsAnalysis {
    public static class EnumerableExtension {
        public static IEnumerable<Tuple<T, T>> Pairs<T>(this IEnumerable<T> source) {
            var first = default(T);
            int counter = 0;

            foreach (var value in source) {
                if (counter++ > 0)
                    yield return new Tuple<T, T>(first, value);

                first = value;
            }
        }

        public static int MaxIndex<T>(this IEnumerable<T> source)
            where T : IComparable, new() {
            var itemsList = source.ToList();

            if (itemsList.Count == 0)
                throw new ArgumentException();

            return itemsList.IndexOf(itemsList.Max());
        }
    }

    public static class Analysis {
        private static void CheckLength<T>(IEnumerable<T> data) {
            if (data.Count() < 2)
                throw new ArgumentException();
        }

        public static int FindMaxPeriodIndex(params DateTime[] data) {
            CheckLength(data);
            return data
                .Pairs()
                .Select(t => t.Item2 - t.Item1)
                .MaxIndex();
        }

        public static double FindAverageRelativeDifference(params double[] data) {
            CheckLength(data);
            return data
                .Pairs()
                .Select(t => (t.Item2 - t.Item1) / t.Item1)
                .Average();
        }
    }
}