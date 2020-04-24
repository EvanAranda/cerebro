using System;
namespace cerebro.math
{
    public static class CMath
    {
        public static int[] Iota(int start, int stop)
        {
            var n = stop - start;
            var iota = new int[n];

            for (int i = 0; i < n; i++)
            {
                iota[i] = i + start;
            }

            return iota;
        }

        static int BinomialCoeff(int n, int k)
        {
            int[] C = new int[k + 1];

            // nC0 is 1 
            C[0] = 1;

            for (int i = 1; i <= n; i++)
            {
                // Compute next row of pascal  
                // triangle using the previous 
                // row 
                for (int j = Math.Min(i, k); j > 0; j--)
                    C[j] = C[j] + C[j - 1];
            }
            return C[k];
        }
    }
}