namespace BokehLab.FloatMap
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using BokehLab.Math;

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
            float[, ,] floatMap = image.Image;

            System.Drawing.Imaging.PixelFormat outputPixelFormat = image.PixelFormat.ToBitmapFormat();
            Bitmap outputImage = new Bitmap(width, height, outputPixelFormat);

            bool shouldWriteAlpha = image.PixelFormat.HasAlpha();
            int colorBands = (int)image.ColorChannelsCount;
            int maxColorBand = colorBands - 1;
            int alphaBand = colorBands;
            int bands = (int)image.TotalChannelsCount;

            float minValue = float.MaxValue;
            float maxValue = float.MinValue;
            if (tonemappingEnabled)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int band = maxColorBand; band >= 0; band--)
                        {
                            float value = floatMap[x, y, band];
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
                // Note: convert greyscale to RGB
                bool isGreyscale = (image.PixelFormat == PixelFormat.Greyscale);
                int maxOutputColorBand = 2;
                int outputBands = 3 + (image.PixelFormat.HasAlpha() ? 1 : 0);
                float scaleRangeInv = 1.0f / (maxValue - minValue); // for tone-mapping
                for (int y = 0; y < height; y++)
                {
                    byte* outputRow = (byte*)outputData.Scan0 + (y * outputData.Stride);
                    for (int x = 0; x < width; x++)
                    {
                        for (int band = maxOutputColorBand; band >= 0; band--)
                        {
                            int hdrBand = (isGreyscale) ? 0 : maxOutputColorBand - band;
                            // translate RGB input image to BGR output image
                            float intensity = floatMap[x, y, hdrBand];
                            intensity = (intensity + shift) * scale;
                            if (tonemappingEnabled)
                            {
                                // do a simple tone-mapping - linear scaling
                                // from [min; max] to [0.0; 1.0]
                                intensity = (intensity - minValue) * scaleRangeInv;
                            }
                            outputRow[x * outputBands + band] = (byte)MathHelper.Clamp(intensity * 255.0f, 0.0f, 255.0f);
                        }
                        if (shouldWriteAlpha)
                        {
                            float intensity = floatMap[x, y, alphaBand];
                            outputRow[x * outputBands + alphaBand] = (byte)MathHelper.Clamp(intensity * 255.0f, 0.0f, 255.0f);
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
            PixelFormat pixelFormat = PixelFormatExtensions.FromBitmapFormat(ldrImage.PixelFormat);
            bool shouldWriteAlpha = pixelFormat.HasAlpha();
            FloatMapImage hdrImage = new FloatMapImage((uint)width, (uint)height, pixelFormat);

            BitmapData inputData = inputImage.LockBits(new Rectangle(0, 0, width, height),
               ImageLockMode.ReadOnly, inputImage.PixelFormat);
            float conversionFactor = 1 / 255.0f;
            float[, ,] floatMap = hdrImage.Image;
            unsafe
            {
                int colorBands = (int)hdrImage.ColorChannelsCount;
                int maxColorBand = colorBands - 1;
                int alphaBand = colorBands;
                int bands = (int)hdrImage.TotalChannelsCount;
                for (int y = 0; y < height; y++)
                {
                    byte* inputRow = (byte*)inputData.Scan0 + (y * inputData.Stride);
                    for (int x = 0; x < width; x++)
                    {
                        // BGR -> RGB or BGRA -> RGBA
                        for (int band = maxColorBand; band >= 0; band--)
                        {
                            int hdrBand = maxColorBand - band;
                            // translate BGR input image to RGB output image in case of a RGB input image
                            floatMap[x, y, hdrBand] = inputRow[x * bands + band] * conversionFactor;
                        }
                        if (shouldWriteAlpha)
                        {
                            floatMap[x, y, alphaBand] = inputRow[x * bands + alphaBand] * conversionFactor;
                        }
                    }
                }
            }
            inputImage.UnlockBits(inputData);
            return hdrImage;
        }
    }
}
