namespace BokehLab.RayTracing
{
    using System.Drawing;
    using BokehLab.FloatMap;

    public interface IRenderer
    {
        int SampleCount { get; set; }

        FloatMapImage RenderImage(Size imageSize);
    }
}
