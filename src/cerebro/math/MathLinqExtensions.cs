using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace cerebro.math
{
    public static class MathLinqExtensions
    {
        public static int Product(this IEnumerable<int> items)
        {
            int prod = 1;
            foreach (var item in items) prod *= item;
            return prod;
        }

        public static void SparseZipMap<T>(
            this IEnumerable<T> seq1,
            IEnumerable<T> seq2,
            Comparison<T> comparer,
            Action<T, int, int> onEqual = null,
            Action<T, int, T, int> onLess1 = null,
            Action<T, int, T, int> onLess2 = null)
            where T : IComparable
        {
            var iter1 = seq1.GetEnumerator();
            var iter2 = seq2.GetEnumerator();

            int index1 = 0;
            int index2 = 0;

            bool iter1Active = iter1.MoveNext();
            bool iter2Active = iter2.MoveNext();

            if (!iter1Active || !iter2Active) return; /// nothing to do

            T a = iter1.Current;
            T b = iter2.Current;

            int comp;

            while (iter1Active && iter2Active)
            {
                a = iter1.Current;
                b = iter2.Current;

                comp = comparer(a, b);

                switch (comp)
                {
                    case 0:
                        onEqual?.Invoke(a, index1, index2);

                        iter1Active = iter1.MoveNext();
                        iter2Active = iter2.MoveNext();

                        index1++;
                        index2++;
                        break;
                    case -1:
                        onLess1?.Invoke(a, index1, b, index2);
                        iter1Active = iter1.MoveNext();
                        index1++;
                        break;
                    case 1:
                        onLess2?.Invoke(a, index1, b, index2);
                        iter2Active = iter2.MoveNext();
                        index2++;
                        break;
                }
            }
        }

        /// <summary>
        /// Counts how many shared active bits are between <paramref name="seq1"> and <paramref name="seq2">
        /// </summary>
        /// <param name="seq1"></param>
        /// <param name="seq2"></param>
        /// <returns></returns>
        public static int OverlapCount(this IEnumerable<int> seq1, IEnumerable<int> seq2)
        {
            int count = 0;
            SparseZipMap(seq1, seq2, Comparer<int>.Default.Compare, onEqual: (x, i, j) => count++);
            return count;
        }

        public static void PermutationSort<T>(this IReadOnlyList<T> items, int[] indices)
        {
            Array.Sort(indices, (int i, int j) => Comparer<T>.Default.Compare(items[i], items[j]));
        }

        public static int[] PermutationSort<T>(this IReadOnlyList<T> items)
        {
            var indices = CMath.Iota(0, items.Count);
            PermutationSort(items, indices);
            return indices;
        }

        public static T[] KMax<T>(this IReadOnlyList<T> items, int k)
        {
            var sortedIndices = PermutationSort(items);
            return sortedIndices.Select(i => items[i]).Reverse().Take(k).ToArray();
        }

        public static SparseBitArray ToSparseBitArray(this IEnumerable<int> indices, int capacity)
        {
            return new SparseBitArray(indices, capacity);
        }
    }
}