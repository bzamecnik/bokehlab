namespace BokehLab.Spreading
{
    using BokehLab.FloatMap;

    public interface ISpreadingFilter
    {
        FloatMapImage FilterImage(FloatMapImage inputImage, FloatMapImage outputImage);
    }
}
