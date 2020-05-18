namespace t4ccer.Noisy
{
    internal class Vector2
    {
        internal double x, y;

        public Vector2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double SquareDistaceTo(Vector2 p)
            => (x - p.x) * (x - p.x) + (y - p.y) * (y - p.y);
        public double DistanceTo(Vector2 p)
            => SquareDistaceTo(p).Sqrt();
    }
}
