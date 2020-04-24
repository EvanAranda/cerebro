using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace cerebro.math
{
    public class SparseBitArray : IEnumerable<int>
    {
        public int Capacity { get; }
        public int Active => _Indices.Count;
        public double Density => (double)Active / (double)Capacity;
        public double Sparsity => 1.0 - Density;
        public int this[int i] => Contains(i) ? 1 : 0;

        public void TurnOn(IEnumerable<int> indices)
        {
            throw new NotImplementedException();
        }

        public bool Contains(int i) => _Indices.Contains(i);

        public int OverlapCount(SparseBitArray arr)
        {
            return _Indices.OverlapCount(arr._Indices);
        }

        private void AssertSizesAreEqual(SparseBitArray arr)
        {
            if (Capacity != arr.Capacity)
            {
                throw new Exception("sizes are not equal");
            }
        }

        public SparseBitArray LogicalNot()
        {
            var result = new List<int>();

            var j = 0;
            var currentIndex = _Indices[j];
            for (int i = 0; i < Capacity; i++)
            {
                if (i != currentIndex)
                {
                    result.Add(i);
                }
                else
                {
                    j++;
                    currentIndex = _Indices[j];
                }
            }

            return new SparseBitArray(result, Capacity);
        }

        SparseBitArray LogicalAnd(SparseBitArray arr)
        {
            AssertSizesAreEqual(arr);

            var result = new List<int>();

            this.SparseZipMap(arr, Comparer<int>.Default.Compare, onEqual: (x, i, j) => result.Add(x));

            return new SparseBitArray(result, Capacity);
        }

        SparseBitArray LogicalOr(SparseBitArray arr)
        {
            AssertSizesAreEqual(arr);

            var result = new List<int>();

            this.SparseZipMap(arr, Comparer<int>.Default.Compare,
                (x, i, j) => result.Add(x),
                (x, i, y, j) => result.Add(x),
                (x, i, y, j) => result.Add(y));

            return new SparseBitArray(result, Capacity);
        }

        public IEnumerator<int> GetEnumerator()
        {
            return _Indices.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Indices.GetEnumerator();
        }

        public static SparseBitArray operator !(SparseBitArray arr) => arr.LogicalNot();

        public static SparseBitArray operator &(SparseBitArray arr1, SparseBitArray arr2) => arr1.LogicalAnd(arr2);

        public static SparseBitArray operator |(SparseBitArray arr1, SparseBitArray arr2) => arr1.LogicalOr(arr2);

        public SparseBitArray(int size)
        {
            Capacity = size;
            _Indices = new List<int>();
        }

        public SparseBitArray(IEnumerable<int> indices, int size)
        {
            Capacity = size;
            _Indices = indices.ToList();

            foreach (var i in _Indices)
            {
                if (i >= size)
                {
                    throw new ArgumentException($"size in not large enough to contain values in indices, {i} > {size}");
                }
            }
        }

        private List<int> _Indices;
    }
}