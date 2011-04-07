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

        /// <summary>
        /// Returns a new image consisting only of the alpha channel.
        /// </summary>
        /// <returns>The image with alpha channel or null if the original
        /// image contained no alpha channel.</returns>
        public FloatMapImage ExtractAlphaChannel()
        {
            if (!PixelFormat.HasAlpha())
            {
                return null;
            }
            FloatMapImage alphaImage = new FloatMapImage(Width, Height, PixelFormat.Greyscale, Scale);
            int inputAlphaBand = (int)PixelFormat.GetTotalChannelsCount() - 1;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    alphaImage.Image[x, y, 0] = Image[x, y, inputAlphaBand];
                }
            }
            return alphaImage;
        }

        /// <summary>
        /// Returns a new image consisting only of the color channel removing
        /// the alpha channel if it was present in the original image.
        /// </summary>
        /// <returns>The image with only color channels.</returns>
        public FloatMapImage ExtractColorChannels()
        {
            if (!PixelFormat.HasAlpha())
            {
                return this;
            }
            FloatMapImage colorImage = new FloatMapImage(Width, Height, PixelFormat.RemoveAlpha(), Scale);
            int bands = (int)PixelFormat.GetColorChannelsCount();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int band = 0; band < bands; band++)
                    {
                        colorImage.Image[x, y, band] = Image[x, y, band];
                    }
                }
            }
            return colorImage;
        }

        public object Clone()
        {
            return new FloatMapImage(this);
        }
    }

    public enum PixelFormat
    {
        /// <summary>
        /// Red, green, blue.
        /// </summary>
        RGB,
        /// <summary>
        /// 1-channel greyscale.
        /// </summary>
        Greyscale,
        /// <summary>
        /// Red, green, blue, alpha.
        /// </summary>
        RGBA,
        /// <summary>
        /// Greyscale, alpha.
        /// </summary>
        GreyscaleA
    }

    public static class PixelFormatExtensions
    {
        public static bool HasAlpha(this PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.RGBA:
                case PixelFormat.GreyscaleA:
                    return true;
                default:
                    return false;
            }
        }

        public static PixelFormat AddAlpha(this PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Greyscale:
                case PixelFormat.GreyscaleA:
                    return PixelFormat.GreyscaleA;
                case PixelFormat.RGB:
                case PixelFormat.RGBA:
                    return PixelFormat.RGBA;
                default:
                    throw new ArgumentException(String.Format("Unsupported pixel format: {0}", pixelFormat));
            }
        }

        public static PixelFormat RemoveAlpha(this PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.GreyscaleA:
                case PixelFormat.Greyscale:
                    return PixelFormat.Greyscale;
                case PixelFormat.RGBA:
                case PixelFormat.RGB:
                    return PixelFormat.RGB;
                default:
                    throw new ArgumentException(String.Format("Unsupported pixel format: {0}", pixelFormat));
            }
        }

        public static System.Drawing.Imaging.PixelFormat ToBitmapFormat(this PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Greyscale:
                    return System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                case PixelFormat.RGB:
                    return System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                case PixelFormat.RGBA:
                    return System.Drawing.Imaging.PixelFormat.Format32bppArgb;
                default:
                    throw new ArgumentException(String.Format("Unsupported pixel format: {0}", pixelFormat));
            }
        }

        public static PixelFormat FromBitmapFormat(System.Drawing.Imaging.PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    return PixelFormat.RGB;
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    return PixelFormat.RGBA;
                default:
                    throw new ArgumentException(String.Format("Unsupported pixel format: {0}", pixelFormat));
            }
        }

        public static uint GetColorChannelsCount(this PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Greyscale:
                case PixelFormat.GreyscaleA:
                    return 1;
                case PixelFormat.RGB:
                case PixelFormat.RGBA:
                    return 3;
                default:
                    throw new ArgumentException(String.Format("Unsupported pixel format: {0}", pixelFormat));
            }
        }

        public static uint GetTotalChannelsCount(this PixelFormat pixelFormat)
        {
            return pixelFormat.GetColorChannelsCount() + (uint)(pixelFormat.HasAlpha() ? 1 : 0);
        }
    }
}
