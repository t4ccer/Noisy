namespace t4ccer.Noisy
{
    public interface INoise
    {
        public double At(double x, double y);
        public double At(double x, double y, double z) => At(x, y);
    }
}
