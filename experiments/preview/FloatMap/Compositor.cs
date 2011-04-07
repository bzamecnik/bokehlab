using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BokehLab.FloatMap
{
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
        private static FloatMapImage Composite(this FloatMapImage imageA, FloatMapImage imageB)
        {
            return null;
        }

        /// <summary>
        /// A over B. A has alpha channel, B hasn't.
        /// Both A and B are not alpha pre-multiplied.
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

            bool hasAlphaOut = outputImage.PixelFormat.HasAlpha();

            for (uint y = 0; y < height; y++)
            {
                for (uint x = 0; x < width; x++)
                {
                    float alphaA = imA[x, y, alphaBand];
                    for (uint band = 0; band < bands; band++)
                    {
                        imOut[x, y, band] = alphaA * imA[x, y, band] + (1 - alphaA) * imB[x, y, band];
                    }
                }
            }
            return outputImage;
        }

        //public static FloatMapImage NormalizeByAlpha(this FloatMapImage image)
        //{
        //    if (!image.PixelFormat.HasAlpha())
        //    {
        //        return image;
        //    }
        //    uint width = image.Width;
        //    uint height = image.Height;
        //    uint bands = image.PixelFormat.GetColorChannelsCount();
        //    uint alphaBand = bands;

        //    float[, ,] im = image.Image;

        //    for (uint y = 0; y < height; y++)
        //    {
        //        for (uint x = 0; x < width; x++)
        //        {
        //            float alphaInv = 1 / im[x, y, alphaBand];
        //            for (uint band = 0; band < bands; band++)
        //            {
        //                im[x, y, band] = alphaInv * im[x, y, band];
        //            }
        //            im[x, y, alphaBand] = 1;
        //        }
        //    }
        //    return image;
        //}

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
                    "The upper image must have alpha channel. Pixel format: {0}",
                    imageA.PixelFormat));
            }
        }
    }
}
