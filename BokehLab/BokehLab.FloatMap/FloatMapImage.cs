namespace BokehLab.FloatMap
{
    using System;

    /// <summary>
    /// Represents an in-memory HDR float-map image.
    /// </summary>
    /// <remarks>
    /// Supports RGB (3-channel) and grayscale (single-channel) images
    /// Also supports transparent RGBA and GA images with alpha channel.
    /// </remarks>
    public class FloatMapImage : ICloneable
    {
        // TODO:
        // - try single-dimensional table instead of multi-dimensional
        //   - can it be any faster at the cost of worse usage?
        public float[, ,] Image { get; private set; }

        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public PixelFormat PixelFormat { get; private set; }
        public float Scale { get; private set; }

        public uint ColorChannelsCount { get; set; }
        public uint TotalChannelsCount { get; set; }

        public FloatMapImage(uint width, uint height) :
            this(width, height, PixelFormat.RGB)
        { }

        public FloatMapImage(uint width, uint height, PixelFormat pixelFormat) :
            this(width, height, pixelFormat, 1.0f)
        { }

        public FloatMapImage(uint width, uint height, PixelFormat pixelFormat, float scale)
        {
            Width = width;
            Height = height;
            PixelFormat = pixelFormat;
            ColorChannelsCount = pixelFormat.GetColorChannelsCount();
            TotalChannelsCount = pixelFormat.GetTotalChannelsCount();
            Scale = scale;
            Image = new float[Width, Height, TotalChannelsCount];
        }

        protected FloatMapImage(FloatMapImage image)
        {
            Width = image.Width;
            Height = image.Height;
            PixelFormat = image.PixelFormat;
            ColorChannelsCount = PixelFormat.GetColorChannelsCount();
            TotalChannelsCount = PixelFormat.GetTotalChannelsCount();
            Scale = image.Scale;
            Image = (float[, ,])image.Image.Clone();
        }

        // TODO: is this really useful?

        public void Dispose()
        {
            Image = null;
        }

        public object Clone()
        {
            return new FloatMapImage(this);
        }
    }
}
