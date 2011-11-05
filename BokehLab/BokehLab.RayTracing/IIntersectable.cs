namespace BokehLab.RayTracing
{
    using BokehLab.Math;

    public interface IIntersectable
    {
        Intersection Intersect(Ray ray);
    }
}
