using System.Collections.Generic;
using System.Linq;
using System;
using cerebro.math;
using System.Collections;

namespace cerebro.encoders
{
    public class InputGenerator<T> : IEnumerable<SparseBitArray>
    {
        public IEncoder<T> Encoder { get; }
        private IEnumerable<T> Values { get; }

        public InputGenerator(IEncoder<T> encoder, Func<T, T> func, IEnumerable<T> range)
        {
            Encoder = encoder;
            Values = range.Select(func);
        }

        public IEnumerator<SparseBitArray> GetEnumerator()
        {
            foreach (var value in Values)
                yield return Encoder.Encode(value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public static InputGenerator<double> SinWave(double amplitude, double x0, double x1, int steps)
        {
            var range = x1 - x0;
            var dx = range / (double)(steps - 1);

            return new InputGenerator<double>(
                new ScalarEncoder(-amplitude, amplitude, 100, 21),
                x => amplitude * Math.Sin(x),
                Enumerable.Range(0, steps).Select(i => x0 + dx * i)
            );
        }
    }
}