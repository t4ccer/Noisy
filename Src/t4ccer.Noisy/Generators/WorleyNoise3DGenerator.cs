using System;
using System.Collections.Generic;
using System.Linq;

namespace t4ccer.Noisy
{
    public class WorleyNoise3DGenerator : INoise
    {
        List<Vector3> points;
        readonly int n;

        public WorleyNoise3DGenerator(uint pointCount, uint n, int minPointX, int minPointY,int minPointZ, int maxPointX, int maxPointY, int maxPointZ)
        {
            var rng = new Random();
            this.n = (int)n;

            CreatePoints(pointCount, minPointX, minPointY, minPointZ, maxPointX, maxPointY, maxPointZ, rng);
        }
        public WorleyNoise3DGenerator(uint pointCount, uint n, int minPointX, int minPointY, int minPointZ, int maxPointX, int maxPointY, int maxPointZ, int seed)
        {
            var rng = new Random(seed);
            this.n = (int)n;

            CreatePoints(pointCount, minPointX, minPointY, minPointZ, maxPointX, maxPointY, maxPointZ, rng);
        }

        private void CreatePoints(uint pointCount, int minPointX, int minPointY, int minPointZ, int maxPointX, int maxPointY, int maxPointZ, Random rng)
        {
            points = new List<Vector3>((int)pointCount);
            for (int i = 0; i < pointCount; i++)
            {
                double x = MathEx.Map(rng.NextDouble(), 0, 1, minPointX, maxPointX);
                double y = MathEx.Map(rng.NextDouble(), 0, 1, minPointY, maxPointY);
                double z = MathEx.Map(rng.NextDouble(), 0, 1, minPointZ, maxPointZ);
                points.Add(new Vector3(x, y, z));
            }
        }

        public double At(double x, double y)
            => At(x, y, 0);

        public double At(double x, double y, double z)
            => At(new Vector3(x, y, z));
        double At(Vector3 p2)
            => points.Select(p => p.SquareDistaceTo(p2)).OrderBy(x => x).ElementAt(n).Sqrt();
    }
}
