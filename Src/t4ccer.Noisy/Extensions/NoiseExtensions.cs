using System.Threading.Tasks;

namespace t4ccer.Noisy
{
    public static class NoiseExtensions
    {
        public static Plane AtPlane(this INoise noise, double startX, double startY, int width, int height, double increment)
            => noise.AtPlane(startX, startY, 0, width, height, increment);
        public static Plane AtPlane(this INoise noise, double startX, double startY, double z, int width, int height, double increment)
        {
            var values = new double[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var val = noise.At(startX + x * increment, startY + y * increment, z);
                    values[x, y] = val;
                }
            }
            return values;
        }

        public static Plane AtPlaneParallel(this INoise noise, double startX, double startY, int width, int height, double increment)
            => noise.AtPlaneParallel(startX, startY, 0, width, height, increment);
        public static Plane AtPlaneParallel(this INoise noise, double startX, double startY, double z, int width, int height, double increment)
        {
            var values = new double[width, height];

            Parallel.For(0, width * height, i =>
            {
                int x = i / height;
                int y = i % height;
                var val = noise.At(startX + x * increment, startY + y * increment, z);
                values[x, y] = val;
            });

            return values;
        }

        public static Line AtLine(this INoise noise, double startX, int width, double increment)
            => AtLine(noise, startX, 0, 0, width, increment);
        public static Line AtLine(this INoise noise, double startX, double y, double z, int width, double increment)
        {
            var values = new double[width];

            for (int x = 0; x < width; x++)
            {
                var val = noise.At(startX + x * increment, y, z);
                values[x] = val;
            }
            return values;
        }
    }
}
