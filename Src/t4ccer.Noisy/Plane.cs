using System;
using System.Drawing;

namespace t4ccer.Noisy
{
    /// <summary>
    /// Plane is simple wrapper for 2D double array.
    /// </summary>
    public class Plane
    {
        public int width, height;
        readonly double[,] values;

        public Plane(double[,] values)
        {
            this.values = values ?? throw new ArgumentNullException(nameof(values));
            width = values.GetLength(0);
            height = values.GetLength(1);
        }

        public double this[int x, int y]
        {
            get => values[x, y];
            set => values[x, y] = value;
        }
        public static implicit operator Plane(double[,] vs) => new Plane(vs);

        /// <summary>
        /// Transforms plane with predicate
        /// </summary>
        /// <param name="predicate">Transform predicate</param>
        /// <returns>Transformed plane</returns>
        public Plane Transform(Func<double, double> predicate)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    values[x, y] = predicate(values[x, y]);
                }
            }
            return this;
        }
        /// <summary>
        /// Makes all values in plane between 0 and 1
        /// </summary>
        /// <returns>Transformed plane</returns>
        public Plane Normalize()
        {
            double max = double.MinValue;
            double min = double.MaxValue;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var val = values[x, y];
                    if (val > max) max = val;
                    if (val < min) min = val;
                }
            }
            return Map(min, max, 0, 1);
        }
        /// <summary>
        /// Maps all values in plane from start1 and stop1 to start2 and stop2
        /// </summary>
        /// <param name="start1">Current start</param>
        /// <param name="stop1">Current stop</param>
        /// <param name="start2">Target start</param>
        /// <param name="stop2">Target stop</param>
        /// <returns>Transformed plane</returns>
        public Plane Map(double start1, double stop1, double start2, double stop2)
        {
            Transform(x => x.Map(start1, stop1, start2, stop2));
            return this;
        }
        /// <summary>
        /// Converts plane to grayscale bitmap
        /// </summary>
        /// <returns></returns>
        public Bitmap ToGrayscaleBitmap()
        {
            var img = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var val = (int)values[x, y];
                    img.SetPixel(x, y, Color.FromArgb(val, val, val));
                }
            }
            return img;
        }

    }
}
