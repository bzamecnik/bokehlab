namespace BokehLab.ImageBasedRayCasting
{
    using BokehLab.Math;

    public interface IIntersectable
    {
        Intersection Intersect(Ray ray);
    }
}
