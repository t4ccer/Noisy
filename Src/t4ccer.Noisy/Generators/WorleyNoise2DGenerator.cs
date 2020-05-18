using System;
using System.Collections.Generic;
using System.Linq;

namespace t4ccer.Noisy
{
    public class WorleyNoise2DGenerator : INoise
    {
        List<Vector2> points;
        int n;

        public WorleyNoise2DGenerator(uint pointCount, uint n, int minPointX, int minPointY, int maxPointX, int maxPointY)
        {
            var rng = new Random();
            CreatePoints(pointCount, n, minPointX, minPointY, maxPointX, maxPointY, rng);
        }
        public WorleyNoise2DGenerator(uint pointCount, uint n, int minPointX, int minPointY, int maxPointX, int maxPointY, int seed)
        {
            var rng = new Random(seed);
            CreatePoints(pointCount, n, minPointX, minPointY, maxPointX, maxPointY, rng);
        }

        private void CreatePoints(uint pointCount, uint n, int minPointX, int minPointY, int maxPointX, int maxPointY, Random rng)
        {
            points = new List<Vector2>((int)pointCount);
            this.n = (int)n;
            for (int i = 0; i < pointCount; i++)
            {
                var x = rng.NextDouble().Map(0, 1, minPointX, maxPointX);
                var y = rng.NextDouble().Map(0, 1, minPointY, maxPointY);

                points.Add(new Vector2(x, y));
            }
        }

        public double At(double x, double y)
            => At(new Vector2(x, y));
        double At(Vector2 p2)
            => points.Select(p => p.SquareDistaceTo(p2)).OrderBy(x => x).ElementAt(n).Sqrt();
    }
}
