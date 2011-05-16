namespace BokehLab.FloatMap
{
    using System;

    public static class ImageProcessing
    {
        /// <summary>
        /// Returns a new image consisting only of the alpha channel.
        /// </summary>
        /// <returns>The image with alpha channel or null if the original
        /// image contained no alpha channel.</returns>
        public static FloatMapImage ExtractAlphaChannel(this FloatMapImage inputImage)
        {
            if (!inputImage.PixelFormat.HasAlpha())
            {
                return null;
            }
            int width = (int)inputImage.Width;
            int height = (int)inputImage.Height;
            FloatMapImage alphaImage = new FloatMapImage((uint)width, (uint)height, PixelFormat.Greyscale, inputImage.Scale);
            int inputAlphaBand = (int)inputImage.PixelFormat.GetTotalChannelsCount() - 1;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    alphaImage.Image[x, y, 0] = inputImage.Image[x, y, inputAlphaBand];
                }
            }
            return alphaImage;
        }

        /// <summary>
        /// Returns a new image consisting only of the color channel removing
        /// the alpha channel if it was present in the original image.
        /// </summary>
        /// <returns>The image with only color channels.</returns>
        public static FloatMapImage ExtractColorChannels(this FloatMapImage inputImage)
        {
            if (!inputImage.PixelFormat.HasAlpha())
            {
                return inputImage;
            }
            int width = (int)inputImage.Width;
            int height = (int)inputImage.Height;
            FloatMapImage colorImage = new FloatMapImage((uint)width, (uint)height, inputImage.PixelFormat.RemoveAlpha(), inputImage.Scale);
            int bands = (int)inputImage.PixelFormat.GetColorChannelsCount();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int band = 0; band < bands; band++)
                    {
                        colorImage.Image[x, y, band] = inputImage.Image[x, y, band];
                    }
                }
            }
            return colorImage;
        }

        public static FloatMapImage Integrate(this FloatMapImage inputImage)
        {
            return Integrate(inputImage, false);
        }

        /// <summary>
        /// Integrate an image into a Summed Area Table (SAT).
        /// </summary>
        /// <param name="inputFloatMap"></param>
        /// <returns></returns>
        public static FloatMapImage Integrate(this FloatMapImage inputImage, bool inPlace)
        {
            uint bands = inputImage.TotalChannelsCount;
            int width = (int)inputImage.Width;
            int height = (int)inputImage.Height;

            FloatMapImage outputImage;
            PrepareOutputImage(inputImage, inPlace, out outputImage);
            float[, ,] input = inputImage.Image;
            float[, ,] output = outputImage.Image;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int band = 0; band < bands; band++)
                    {
                        float intensity = input[x, y, band];
                        // clamp indexes at borders
                        if (x - 1 >= 0)
                        {
                            intensity += output[x - 1, y, band];
                        }
                        if (y - 1 >= 0)
                        {
                            intensity += output[x, y - 1, band];
                        }
                        if ((x - 1 >= 0) && (y - 1 >= 0))
                        {
                            intensity -= output[x - 1, y - 1, band];
                        }
                        output[x, y, band] = intensity;
                    }
                }
            }
            return outputImage;
        }

        public static FloatMapImage Differentiate(this FloatMapImage inputImage)
        {
            return Differentiate(inputImage, false);
        }

        /// <summary>
        /// Differentiate an image.
        /// </summary>
        /// <param name="inputFloatMap"></param>
        /// <returns></returns>
        public static FloatMapImage Differentiate(this FloatMapImage inputImage, bool inPlace)
        {
            uint bands = inputImage.TotalChannelsCount;
            int width = (int)inputImage.Width;
            int height = (int)inputImage.Height;

            FloatMapImage outputImage;
            PrepareOutputImage(inputImage, inPlace, out outputImage);
            float[, ,] input = inputImage.Image;
            float[, ,] output = outputImage.Image;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int band = 0; band < bands; band++)
                    {
                        float intensity = input[x, y, band];
                        // clamp indexes at borders
                        if (x - 1 >= 0)
                        {
                            intensity -= input[x - 1, y, band];
                        }
                        if (y - 1 >= 0)
                        {
                            intensity -= input[x, y - 1, band];
                        }
                        if ((x - 1 >= 0) && (y - 1 >= 0))
                        {
                            intensity += input[x - 1, y - 1, band];
                        }
                        output[x, y, band] = intensity;
                    }
                }
            }
            return outputImage;
        }

        public static FloatMapImage IntegrateHorizontally(this FloatMapImage inputImage)
        {
            return IntegrateHorizontally(inputImage, false);
        }

        public static FloatMapImage IntegrateHorizontally(this FloatMapImage inputImage, bool inPlace)
        {
            uint bands = inputImage.TotalChannelsCount;
            int width = (int)inputImage.Width;
            int height = (int)inputImage.Height;

            FloatMapImage outputImage;
            PrepareOutputImage(inputImage, inPlace, out outputImage);
            float[, ,] input = inputImage.Image;
            float[, ,] output = outputImage.Image;

            for (int y = 0; y < height; y++)
            {
                for (int x = 1; x < width; x++)
                {
                    for (int band = 0; band < bands; band++)
                    {
                        output[x, y, band] += input[x - 1, y, band];
                    }
                }
            }

            return outputImage;
        }

        public static FloatMapImage IntegrateVertically(this FloatMapImage inputImage)
        {
            return IntegrateVertically(inputImage, false);
        }

        public static FloatMapImage IntegrateVertically(this FloatMapImage inputImage, bool inPlace)
        {
            uint bands = inputImage.TotalChannelsCount;
            int width = (int)inputImage.Width;
            int height = (int)inputImage.Height;

            FloatMapImage outputImage;
            PrepareOutputImage(inputImage, inPlace, out outputImage);
            float[, ,] input = inputImage.Image;
            float[, ,] output = outputImage.Image;

            for (int x = 0; x < width; x++)
            {
                for (int y = 1; y < height; y++)
                {
                    for (int band = 0; band < bands; band++)
                    {
                        output[x, y, band] += input[x, y - 1, band];
                    }
                }
            }

            return outputImage;
        }


        /// <summary>
        /// Divide one image by another. Create a new image for the output.
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="divisorImage"></param>
        /// <returns></returns>
        public static FloatMapImage DivideBy(this FloatMapImage inputImage, FloatMapImage divisorImage)
        {
            return DivideBy(inputImage, divisorImage, false);
        }

        /// <summary>
        /// Divide one image by another.
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="divisorImage"></param>
        /// <param name="inPlace">if true put the output to the inputImage; otherwise create a new image</param>
        /// <returns></returns>
        public static FloatMapImage DivideBy(this FloatMapImage inputImage, FloatMapImage divisorImage, bool inPlace)
        {
            return DivideBy(inputImage, divisorImage, null, inPlace);
        }

        /// <summary>
        /// Divide one image by another.
        /// </summary>
        /// <remarks>
        /// If inPlace
        /// </remarks>
        /// <param name="inputImage"></param>
        /// <param name="divisorImage"></param>
        /// <param name="outputImage"></param>
        /// <param name="inPlace"></param>
        /// <returns></returns>
        public static FloatMapImage DivideBy(this FloatMapImage inputImage, FloatMapImage divisorImage, FloatMapImage outputImage, bool inPlace)
        {
            uint bands = inputImage.TotalChannelsCount;
            int width = (int)inputImage.Width;
            int height = (int)inputImage.Height;

            if (outputImage == null)
            {
                PrepareOutputImage(inputImage, inPlace, out outputImage);
            }
            else
            {
                width = Math.Min(width, (int)outputImage.Width);
                height = Math.Min(height, (int)outputImage.Height);
            }
            float[, ,] input = inputImage.Image;
            float[, ,] output = outputImage.Image;
            float[, ,] divisor = divisorImage.Image;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float normalization = 1 / divisor[x, y, 0];
                    for (int band = 0; band < bands; band++)
                    {
                        output[x, y, band] = input[x, y, band] * normalization;
                    }
                }
            }
            return outputImage;
        }

        private static void PrepareOutputImage(FloatMapImage inputImage, bool inPlace, out FloatMapImage outputImage)
        {
            outputImage = (inPlace) ? inputImage :
                new FloatMapImage(
                    (uint)inputImage.Width,
                    (uint)inputImage.Height,
                    inputImage.PixelFormat);
        }
    }
}
