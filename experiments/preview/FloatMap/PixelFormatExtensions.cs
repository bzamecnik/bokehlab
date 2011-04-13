using System;

namespace BokehLab.FloatMap
{
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
