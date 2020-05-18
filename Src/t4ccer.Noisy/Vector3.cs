namespace t4ccer.Noisy
{
    internal class Vector3
    {
        internal double x, y, z;

        public Vector3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public double SquareDistaceTo(Vector3 p)
            => (x - p.x) * (x - p.x) + (y - p.y) * (y - p.y) + (z - p.z) * (z - p.z);
        public double DistanceTo(Vector3 p)
            => SquareDistaceTo(p).Sqrt();
    }
}
