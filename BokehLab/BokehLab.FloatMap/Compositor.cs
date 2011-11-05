namespace BokehLab.FloatMap
{
    using System;

    // TODO:
    // - make one general composition operation
    // - implement various operations via the general one
    // - provide both in-place compositing and compositing to a new image
    // - provide extension methods for FloatMapImage

    /// <summary>
    /// Alpha blending compositor.
    /// </summary>
    public static class Compositor
    {
        /// <summary>
        /// Multiplies colors by alpha channel. Needed for combining
        /// semi-transparent pixels (such as compositing, blurring, etc.).
        /// Creates a new image for the result. Alpha channel remains the same.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static FloatMapImage PremultiplyByAlpha(this FloatMapImage image)
        {
            if (!image.PixelFormat.HasAlpha())
            {
                throw new ArgumentException(String.Format(
                    "The image must have an alpha channel. Pixel format: {0}",
                    image.PixelFormat));
            }

            uint width = image.Width;
            uint height = image.Height;
            uint bands = image.PixelFormat.GetColorChannelsCount();
            uint alphaBand = bands;

            FloatMapImage outputImage = new FloatMapImage(width, height, image.PixelFormat);
            float[, ,] imIn = image.Image;
            float[, ,] imOut = outputImage.Image;

            for (uint y = 0; y < height; y++)
            {
                for (uint x = 0; x < width; x++)
                {
                    float alpha = imIn[x, y, alphaBand];
                    imOut[x, y, alphaBand] = alpha;
                    for (uint band = 0; band < bands; band++)
                    {
                        imOut[x, y, band] = alpha * imIn[x, y, band];
                    }
                }
            }
            return outputImage;
        }

        /// <summary>
        /// Composites images: A over B. Both A and B are alpha pre-multiplied.
        /// Assumes that image A has an alpha channel and image B is completely
        /// opaque (does not have any alpha channel).
        /// </summary>
        /// <param name="imageA"></param>
        /// <param name="imageB"></param>
        /// <returns></returns>
        public static FloatMapImage Over(this FloatMapImage imageA, FloatMapImage imageB)
        {
            CheckImageCompatibility(imageA, imageB);
            uint width = imageA.Width;
            uint height = imageA.Height;
            uint bands = imageA.PixelFormat.GetColorChannelsCount();
            uint alphaBand = bands;

            FloatMapImage outputImage = new FloatMapImage(width, height, imageA.PixelFormat.RemoveAlpha());
            float[, ,] imA = imageA.Image;
            float[, ,] imB = imageB.Image;
            float[, ,] imOut = outputImage.Image;

            for (uint y = 0; y < height; y++)
            {
                for (uint x = 0; x < width; x++)
                {
                    float alphaA = imA[x, y, alphaBand];
                    for (uint band = 0; band < bands; band++)
                    {
                        imOut[x, y, band] = imA[x, y, band] + (1 - alphaA) * imB[x, y, band];
                    }
                }
            }
            return outputImage;
        }

        ///// <summary>
        ///// A Over B, then normalize by resulting alpha.
        ///// </summary>
        ///// <param name="imageA"></param>
        ///// <param name="imageB"></param>
        ///// <returns></returns>
        //public static FloatMapImage Blend(this FloatMapImage imageA, FloatMapImage imageB)
        //{
        //    CheckImageCompatibility(imageA, imageB);
        //    uint width = imageA.Width;
        //    uint height = imageA.Height;
        //    uint bands = imageA.PixelFormat.GetColorChannelsCount();
        //    uint alphaBand = bands;

        //    FloatMapImage outputImage = new FloatMapImage(width, height, imageA.PixelFormat.RemoveAlpha());
        //    float[, ,] imA = imageA.Image;
        //    float[, ,] imB = imageB.Image;
        //    float[, ,] imOut = outputImage.Image;

        //    bool hasAlphaB = imageB.PixelFormat.HasAlpha();

        //    for (uint y = 0; y < height; y++)
        //    {
        //        for (uint x = 0; x < width; x++)
        //        {
        //            float alphaA = imA[x, y, alphaBand];
        //            float alphaAInv = 1 / alphaA;
        //            for (uint band = 0; band < bands; band++)
        //            {
        //                imOut[x, y, band] = (imA[x, y, band] + (1 - alphaA) * imB[x, y, band]) * alphaAInv;
        //            }
        //        }
        //    }
        //    return outputImage;
        //}

        private static void CheckImageCompatibility(FloatMapImage imageA, FloatMapImage imageB)
        {
            if ((imageA.Width != imageB.Width) || (imageA.Height != imageB.Height))
            {
                throw new ArgumentException(String.Format(
                    "The images must have the same dimensions. Image A: [{0}x{1}], Image B: [{2}x{3}]",
                    imageA.Width, imageA.Height, imageB.Width, imageB.Height));
            }
            if (imageA.PixelFormat.RemoveAlpha() != imageB.PixelFormat.RemoveAlpha())
            {
                throw new ArgumentException(String.Format(
                    "The images must have compatible same pixel formats. Image A: {0}, Image B: {1}",
                    imageA.PixelFormat, imageB.PixelFormat));
            }
            if (!imageA.PixelFormat.HasAlpha())
            {
                throw new ArgumentException(String.Format(
                    "The upper image must have an alpha channel. Pixel format: {0}",
                    imageA.PixelFormat));
            }
        }
    }
}
