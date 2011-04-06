using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Drawing;
using System.Drawing.Imaging;

namespace BokehLab.FloatMap
{
    /// <summary>
    /// Represents an in-memory HDR float-map image.
    /// </summary>
    /// <remarks>
    /// Supports RGB (3-channel) and grayscale (single-channel) images.
    /// Supports 
    /// </remarks>
    public class FloatMapImage
    {
        // TODO:
        // - try single-dimensional table instead of multi-dimensional
        //   - can it be any faster at the cost of worse usage?
        public float[, ,] Image { get; private set; }

        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public PixelFormat PixelFormat { get; private set; }
        public float Scale { get; private set; }

        public uint ChannelsCount { get; set; }

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
            ChannelsCount = GetChannelsCount(pixelFormat);
            Scale = scale;
            Image = new float[Width, Height, ChannelsCount];
        }

        private static uint GetChannelsCount(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.RGB:
                    return 3;
                case PixelFormat.Greyscale:
                    return 1;
                default:
                    throw new ArgumentException(String.Format("Unsupported pixel format: {0}", pixelFormat));
            }
        }

        public void Dispose()
        {
            Image = null;
        }

        
    }

    public enum PixelFormat
    {
        RGB,
        Greyscale,
    }
}
