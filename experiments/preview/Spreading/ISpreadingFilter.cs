using BokehLab.FloatMap;

namespace BokehLab.Spreading
{
    public interface ISpreadingFilter
    {
        FloatMapImage FilterImage(FloatMapImage inputImage, FloatMapImage outputImage);
    }
}
