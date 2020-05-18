using System;

namespace t4ccer.Noisy
{
    internal static class MathEx
    {
        public static double Sqrt(this double x)
            => Math.Sqrt(x);
        public static double Map(this double n, double start1, double stop1, double start2, double stop2)
            => ((n - start1) / (stop1 - start1)) * (stop2 - start2) + start2;
    }
}
