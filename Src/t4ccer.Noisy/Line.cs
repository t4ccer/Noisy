using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace t4ccer.Noisy
{
    /// <summary>
    /// Line is simple wrapper for 1D double array
    /// </summary>
    public class Line
    {
        public int width;
        readonly double[] values;

        public Line(double[] values)
        {
            this.values = values ?? throw new ArgumentNullException(nameof(values));
            width = values.Length;
        }
        public double this[int x]
        {
            get => values[x];
            set => values[x] = value;
        }
        public static implicit operator Line(double[] vs) => new Line(vs);


        /// <summary>
        /// Transforms line with predicate
        /// </summary>
        /// <param name="predicate">Transform predicate</param>
        /// <returns>Transformed line</returns>
        public Line Transform(Func<double, double> predicate)
        {
            for (int x = 0; x < width; x++)
            {
                values[x] = predicate(values[x]);
            }
            return this;
        }
        /// <summary>
        /// Makes all values in line between 0 and 1
        /// </summary>
        /// <returns>Transformed line</returns>
        public Line Normalize()
        {
            double max = double.MinValue;
            double min = double.MaxValue;

            for (int x = 0; x < width; x++)
            {
                var val = values[x];
                if (val > max) max = val;
                if (val < min) min = val;
            }
            return Map(min, max, 0, 1);
        }
        /// <summary>
        /// Maps all values in line from start1 and stop1 to start2 and stop2
        /// </summary>
        /// <param name="start1">Current start</param>
        /// <param name="stop1">Current stop</param>
        /// <param name="start2">Target start</param>
        /// <param name="stop2">Target stop</param>
        /// <returns>Transformed line</returns>
        public Line Map(double start1, double stop1, double start2, double stop2)
        {
            Transform(x => x.Map(start1, stop1, start2, stop2));
            return this;
        }
        /// <summary>
        /// Converts line to grayscale bitmap
        /// </summary>
        /// <returns></returns>
        public Bitmap ToGrayscaleBitmap()
        {
            var img = new Bitmap(width, 1);

            for (int x = 0; x < width; x++)
            {
                var val = (int)values[x];
                img.SetPixel(x, 0, Color.FromArgb(val, val, val));
            }
            return img;
        }

    }
}
