﻿// TODO:
// - should the spreading table and normalization channel be squashed into
//   one image for better locality?
// - could alpha channel be used as a normalization channel?
//   - if we want to spread transparent images, a separate normalization channel is needed
// - implement better PSFs:
//   - perimeter spreading
//   - polynomial spreading

namespace BokehLab.Spreading
{
    using BokehLab.FloatMap;

    public abstract class AbstractSpreadingFilter : ISpreadingFilter
    {
        public BlurMap Blur;
        public bool SpreadOneRoundedPSF { get; set; }

        public FloatMapImage FilterImage(FloatMapImage inputImage, FloatMapImage outputImage)
        {
            if (inputImage == null) return null;

            if (inputImage.PixelFormat.HasAlpha())
            {
                inputImage = inputImage.PremultiplyByAlpha();
            }

            uint width = inputImage.Width;
            uint height = inputImage.Height;

            if (width < 1 || height < 1) return null;

            if (outputImage == null)
            {
                outputImage = new FloatMapImage(width, height, inputImage.PixelFormat);
            }

            FloatMapImage spreadingTable = new FloatMapImage(width, height, inputImage.PixelFormat);
            FloatMapImage normalizationTable = new FloatMapImage(width, height, PixelFormat.Greyscale);

            //var blurMap = GetBlurMap(width, height);
            //blurMap.ToBitmap(true).Save("blurmap.png");
            //blurMap.Differentiate().ToBitmap(true).Save("blurmap-diff.png");

            Filter(inputImage, spreadingTable, normalizationTable);

            Normalize(spreadingTable, normalizationTable, outputImage);

            spreadingTable.Dispose();
            normalizationTable.Dispose();

            return outputImage;
        }

        protected abstract void Filter(FloatMapImage inputImage, FloatMapImage spreadingTable, FloatMapImage normalizationTable);

        protected void Spread(FloatMapImage inputImage, FloatMapImage spreadingTable, FloatMapImage normalizationTable)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Reset();
            //sw.Start();

            uint bands = inputImage.TotalChannelsCount;
            uint width = inputImage.Width;
            uint height = inputImage.Height;

            float[, ,] origImage = inputImage.Image;
            float[, ,] spreadingImage = spreadingTable.Image;
            float[, ,] normalizationImage = normalizationTable.Image;

            int tableWidth = (int)spreadingTable.Width;
            int tableHeight = (int)spreadingTable.Height;

            if (SpreadOneRoundedPSF)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        float radius = Blur.GetPSFRadius(x, y);
                        SpreadPSF(x, y, (int)radius, 1.0f, origImage, spreadingImage, normalizationImage, tableWidth, tableHeight, bands);
                    }
                }
            }
            else
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        float radius = Blur.GetPSFRadius(x, y);

                        // spread PSFs of a non-integer radius using two weighted
                        // PSFs of integer size close to the original radius
                        int smallerRadius = (int)radius;
                        int biggerRadius = smallerRadius + 1;
                        float weight = biggerRadius - radius;

                        SpreadPSF(x, y, smallerRadius, weight, origImage, spreadingImage, normalizationImage, tableWidth, tableHeight, bands);
                        SpreadPSF(x, y, biggerRadius, 1 - weight, origImage, spreadingImage, normalizationImage, tableWidth, tableHeight, bands);
                    }
                }
            }
            //Console.WriteLine("Phase 1, spreading: {0} ms", sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <param name="weight"></param>
        /// <param name="origImage"></param>
        /// <param name="spreadingImage"></param>
        /// <param name="tableWidth"></param>
        /// <param name="tableHeight"></param>
        /// <param name="bands">Color bands in original image.</param>
        internal abstract void SpreadPSF(int x, int y, int radius, float weight, float[, ,] origImage, float[, ,] spreadingImage, float[, ,] normalizationImage, int tableWidth, int tableHeight, uint bands);

        protected static void IntegrateVertically(FloatMapImage spreadingTable, FloatMapImage normalizationTable)
        {
            spreadingTable.IntegrateVertically(true);
            normalizationTable.IntegrateVertically(true);
        }

        protected static void IntegrateHorizontally(FloatMapImage spreadingTable, FloatMapImage normalizationTable)
        {
            spreadingTable.IntegrateHorizontally(true);
            normalizationTable.IntegrateHorizontally(true);
        }

        private static void Normalize(FloatMapImage spreadingTable, FloatMapImage normalizationTable, FloatMapImage outputImage)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Reset();
            //sw.Start();

            outputImage = spreadingTable.DivideBy(normalizationTable, outputImage, true);

            //normalizationTable.ToBitmap(true).Save("normalization.png");
            //spreadingTable.ToBitmap(true).Save("unnormalized.png");

            //Console.WriteLine("Normalizing to output image: {0} ms", sw.ElapsedMilliseconds);
        }

        private FloatMapImage GetBlurMap(uint width, uint height)
        {
            FloatMapImage blurMap = new FloatMapImage(width, height, PixelFormat.Greyscale);
            var blurMapImage = blurMap.Image;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    blurMapImage[x, y, 0] = Blur.GetPSFRadius(x, y);
                }
            }
            return blurMap;
        }
    }
}
