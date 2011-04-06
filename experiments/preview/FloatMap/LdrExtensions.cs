using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace BokehLab.FloatMap
{
    /// <summary>
    /// Extensions methods for conversion between low dynamic range bitmaps
    /// and high dynamic range float maps.
    /// </summary>
    public static class LdrExtensions
    {
        public static Bitmap ToBitmap(this FloatMapImage image)
        {
            return ToBitmap(image, false);
        }

        public static Bitmap ToBitmap(this FloatMapImage image, bool tonemappingEnabled)
        {
            return ToBitmap(image, tonemappingEnabled, 1.0f, 0.0f);
        }

        public static Bitmap ToBitmap(this FloatMapImage image, bool tonemappingEnabled, float scale, float shift)
        {
            int width = (int)image.Width;
            int height = (int)image.Height;

            Bitmap outputImage = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            int bands = (int)image.ChannelsCount;
            int maxBand = bands - 1;

            float minValue = float.MaxValue;
            float maxValue = float.MinValue;
            if (tonemappingEnabled)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        for (int band = maxBand; band >= 0; band--)
                        {
                            float value = image.Image[x, y, band];
                            minValue = Math.Min(minValue, value);
                            maxValue = Math.Max(maxValue, value);
                        }
                    }
                }
            }

            BitmapData outputData = outputImage.LockBits(new Rectangle(0, 0, width, height),
               ImageLockMode.ReadOnly, outputImage.PixelFormat);
            unsafe
            {
                bool isGreyscale = (image.PixelFormat == PixelFormat.Greyscale);
                float scaleRangeInv = 1.0f / (maxValue - minValue); // for tone-mapping
                for (int y = 0; y < image.Height; y++)
                {
                    byte* outputRow = (byte*)outputData.Scan0 + (y * outputData.Stride);
                    for (int x = 0; x < image.Width; x++)
                    {
                        for (int band = 2; band >= 0; band--)
                        {
                            int hdrBand = (isGreyscale) ? 0 : maxBand - band;
                            // translate RGB input image to BGR output image
                            float intensity = image.Image[x, y, hdrBand];
                            intensity = (intensity + shift) * scale;
                            if (tonemappingEnabled)
                            {
                                // do a simple tone-mapping - linear scaling
                                // from [min; max] to [0.0; 1.0]
                                intensity = (intensity - minValue) * scaleRangeInv;
                            }
                            outputRow[x * 3 + band] = (byte)MathHelper.Clamp(intensity * 255.0f, 0.0f, 255.0f);
                        }
                    }
                }
            }
            outputImage.UnlockBits(outputData);

            return outputImage;
        }

        public static FloatMapImage ToFloatMap(this System.Drawing.Bitmap ldrImage)
        {
            Bitmap inputImage = ldrImage;
            int width = ldrImage.Width;
            int height = ldrImage.Height;
            PixelFormat pixelFormat;
            switch (ldrImage.PixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    inputImage = ldrImage.Clone(new Rectangle(0, 0, width, height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    pixelFormat = PixelFormat.RGB;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    pixelFormat = PixelFormat.RGB;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                    pixelFormat = PixelFormat.Greyscale;
                    break;
                default:
                    throw new ArgumentException(String.Format("Unsupported input LDR image pixel format: {0}", ldrImage.PixelFormat));
            }

            FloatMapImage hdrImage = new FloatMapImage((uint)width, (uint)height, pixelFormat);

            BitmapData inputData = inputImage.LockBits(new Rectangle(0, 0, width, height),
               ImageLockMode.ReadOnly, inputImage.PixelFormat);
            float conversionFactor = 1 / 255.0f;
            unsafe
            {
                bool isGreyscale = (hdrImage.PixelFormat == PixelFormat.Greyscale);
                int bands = (int)hdrImage.ChannelsCount;
                int maxBand = bands - 1;
                for (int y = 0; y < height; y++)
                {
                    byte* inputRow = (byte*)inputData.Scan0 + (y * inputData.Stride);
                    for (int x = 0; x < width; x++)
                    {
                        for (int band = maxBand; band >= 0; band--)
                        {
                            int hdrBand = (isGreyscale) ? 0 : maxBand - band;
                            // translate BGR input image to RGB output image in case of a RGB input image
                            hdrImage.Image[x, y, hdrBand] = inputRow[x * bands + band] * conversionFactor;
                        }
                    }
                }
            }
            inputImage.UnlockBits(inputData);
            return hdrImage;
        }
    }
}
