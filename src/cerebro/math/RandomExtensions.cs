using System;

namespace cerebro.math
{
    public static class RandomExtensions
    {
        public static double NextDouble(this Random rng, double minValue, double maxValue)
        {
            return rng.NextDouble() * (maxValue - minValue) + minValue;
        }
    }
}