using System.Linq;
using System;
using cerebro.math;

namespace cerebro.encoders
{
    public class ScalarEncoder : IEncoder<double>
    {
        public double MinValue { get; }
        public double MaxValue { get; }
        public int BucketCount { get; }
        public int ActiveBitsCount { get; }
        public double Range => MaxValue - MinValue;

        public SparseBitArray Encode(double input)
        {
            var n = BucketCount + ActiveBitsCount;

            if (input < MinValue) input = MinValue;
            if (input > MaxValue) input = MaxValue;

            var i = (int)Math.Floor(BucketCount * (input - MinValue) / Range);
            var activeBitIndices = Enumerable.Range(i, ActiveBitsCount);
            return new SparseBitArray(activeBitIndices, n);
        }

        public ScalarEncoder(
            double minValue,
            double maxValue,
            int buckets,
            int activeBits)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            BucketCount = buckets;
            ActiveBitsCount = activeBits;
        }

        public static ScalarEncoder Default { get; } =
            new ScalarEncoder(0, 100, 100, 21);

    }


}