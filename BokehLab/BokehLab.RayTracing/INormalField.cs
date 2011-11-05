namespace BokehLab.RayTracing
{
    using OpenTK;

    public interface INormalField
    {
        Vector3d GetNormal(Vector3d position);
    }
}
