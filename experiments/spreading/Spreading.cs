﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using libpfm;
using mathHelper;

// TODO:
// - implement non-integer PSF size - interpolation between two integer PSFs
// - shouldn't the table be of the same size as the input image?
//   - now it seems it has to be larger by 1px
// - should the spreading table and normalization channel be squashed into
//   one image for better locality?

namespace spreading
{
    class RectangleSpreadingFilter
    {
        public static readonly int DEFAULT_BLUR_RADIUS = 25;
        public int MaxBlurRadius { get; set; }

        public RectangleSpreadingFilter()
        {
            MaxBlurRadius = DEFAULT_BLUR_RADIUS;
        }

        public PFMImage SpreadPSF(PFMImage inputImage, PFMImage outputImage)
        {
            return SpreadPSF(inputImage, outputImage, null);
        }

        public PFMImage SpreadPSF(PFMImage inputImage, PFMImage outputImage, PFMImage depthMap)
        {
            if (inputImage == null) return null;

            uint width = inputImage.Width;
            uint height = inputImage.Height;

            if (width < 1 || height < 1) return null;

            if ((depthMap != null) && ((depthMap.Width != width) || (depthMap.Height != height)))
            {
                throw new ArgumentException(String.Format(
                    "Depth map must have the same dimensions as the input image"
                    + " {0}x{1}, but it's size was {2}x{3}.", width, height, depthMap.Width, depthMap.Height));
            }

            if (outputImage == null)
            {
                outputImage = new PFMImage(width, height, inputImage.PixelFormat);
            }

            PFMImage spreadingTable = new PFMImage(width + 1, height + 1, inputImage.PixelFormat);
            PFMImage normalizationTable = new PFMImage(width + 1, height + 1, PixelFormat.Greyscale);

            // initialize the spreading table to 0.0 and the normalization channel to 1.0
            InitializeTables(spreadingTable, normalizationTable);

            //Blur blur = new ConstantBlur(MaxBlurRadius);
            Blur blur = CreateBlurFunction(depthMap, width, height);

            // phase 1: distribute corners into the table
            Spread(inputImage, spreadingTable, normalizationTable, blur);

            // phase 2: accumulate the corners into rectangles
            Integrate(spreadingTable, normalizationTable);

            Normalize(spreadingTable, normalizationTable, outputImage);

            Console.WriteLine();

            // TODO: dispose the spreadingTable and normalizationTable
            spreadingTable.Dispose();
            normalizationTable.Dispose();

            return outputImage;
        }

        private static void InitializeTables(PFMImage spreadingTable, PFMImage normalizationTable)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Reset();
            //sw.Start();

            uint bands = spreadingTable.ChannelsCount;
            // NOTE: the following properties are stored as an optimization
            float[, ,] spreadingImage = spreadingTable.Image;
            float[, ,] normalizationImage = normalizationTable.Image;
            int tableWidth = (int)spreadingTable.Width;
            int tableHeight = (int)spreadingTable.Height;

            for (int x = 0; x < tableWidth; x++)
            {
                for (int y = 0; y < tableHeight; y++)
                {
                    for (int band = 0; band < bands; band++)
                    {
                        spreadingImage[x, y, band] = 0;
                    }
                    normalizationImage[x, y, 0] = 0;
                }
            }
            //Console.WriteLine("Initializing spreading and normalization tables: {0} ms", sw.ElapsedMilliseconds);
        }

        private static void Spread(PFMImage inputImage, PFMImage spreadingTable, PFMImage normalizationTable, Blur blur)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Reset();
            //sw.Start();

            uint bands = inputImage.ChannelsCount;
            uint width = inputImage.Width;
            uint height = inputImage.Height;

            float[, ,] origImage = inputImage.Image;
            float[, ,] spreadingImage = spreadingTable.Image;
            float[, ,] normalizationImage = normalizationTable.Image;

            int tableWidth = (int)spreadingTable.Width;
            int tableHeight = (int)spreadingTable.Height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int radius = (int)blur.GetPSFRadius(x, y);
                    float areaInv = 1.0f / ((radius * 2 + 1) * (radius * 2 + 1));

                    int top = MathHelper.Clamp<int>(y - radius, 0, tableHeight - 1);
                    int bottom = (int)MathHelper.Clamp<int>(y + radius + 1, 0, tableHeight - 1);
                    int left = (int)MathHelper.Clamp<int>(x - radius, 0, tableWidth - 1);
                    int right = (int)MathHelper.Clamp<int>(x + radius + 1, 0, tableWidth - 1);

                    for (int band = 0; band < bands; band++)
                    {
                        float cornerValue = origImage[x, y, band] * areaInv;
                        PutCorners(spreadingImage, top, bottom, left, right, band, cornerValue);
                    }
                    // Note: intensity for the normalization is 1.0
                    PutCorners(normalizationImage, top, bottom, left, right, 0, areaInv);
                }
            }
            //Console.WriteLine("Phase 1, spreading: {0} ms", sw.ElapsedMilliseconds);
        }

        private static void PutCorners(float[, ,] table, int top, int bottom, int left, int right, int band, float value)
        {
            table[left, top, band] += value;
            table[right, top, band] -= value;
            table[left, bottom, band] -= value;
            table[right, bottom, band] += value;
        }

        private static void Integrate(PFMImage spreadingTable, PFMImage normalizationTable)
        {
            //long start = 0;
            //Stopwatch sw = new Stopwatch();
            //sw.Reset();
            //sw.Start();

            uint bands = spreadingTable.ChannelsCount;
            float[, ,] spreadingImage = spreadingTable.Image;
            float[, ,] normalizationImage = normalizationTable.Image;
            int tableWidth = (int)spreadingTable.Width;
            int tableHeight = (int)spreadingTable.Height;

            for (int y = 0; y < tableHeight; y++)
            {
                for (int x = 1; x < tableWidth; x++)
                {
                    for (int band = 0; band < bands; band++)
                    {
                        spreadingImage[x, y, band] += spreadingImage[x - 1, y, band];
                    }
                    normalizationImage[x, y, 0] += normalizationImage[x - 1, y, 0];
                }
            }
            //Console.WriteLine("Phase 2, horizontal integration: {0} ms", sw.ElapsedMilliseconds);

            //start = sw.ElapsedMilliseconds;
            for (int x = 0; x < tableWidth; x++)
            {
                for (int y = 1; y < tableHeight; y++)
                {
                    for (int band = 0; band < bands; band++)
                    {
                        spreadingImage[x, y, band] += spreadingImage[x, y - 1, band];
                    }
                    normalizationImage[x, y, 0] += normalizationImage[x, y - 1, 0];
                }
            }
            //Console.WriteLine("Phase 2, vertical intergration: {0} ms", sw.ElapsedMilliseconds - start);
        }

        private static void Normalize(PFMImage spreadingTable, PFMImage normalizationTable, PFMImage outputImage)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Reset();
            //sw.Start();

            uint bands = outputImage.ChannelsCount;
            float[, ,] image = outputImage.Image;
            float[, ,] spreadingImage = spreadingTable.Image;
            float[, ,] normalizationImage = normalizationTable.Image;
            int width = (int)outputImage.Width;
            int height = (int)outputImage.Height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float normalization = 1 / normalizationImage[x, y, 0];
                    for (int band = 0; band < bands; band++)
                    {
                        image[x, y, band] = spreadingImage[x, y, band] * normalization;
                    }
                }
            }
            //Console.WriteLine("Normalizing to output image: {0} ms", sw.ElapsedMilliseconds);
        }

        private Blur CreateBlurFunction(PFMImage depthMap, uint width, uint height)
        {
            Blur blur;
            if (depthMap != null)
            {
                blur = new DepthMapBlur(depthMap, MaxBlurRadius);
            }
            else
            {
                blur = new ProceduralBlur((int)width, (int)height, MaxBlurRadius);
            }
            //new ConstantBlur(MaxBlurRadius);
            return blur;
        }
    }

    interface Blur
    {
        float GetPSFRadius(int x, int y);
    }

    class DepthMapBlur : Blur
    {
        PFMImage DepthMap { get; set; }
        float MaxPSFRadius { get; set; }

        public DepthMapBlur(PFMImage depthMap, int maxPSFRadius)
        {
            DepthMap = depthMap;
            MaxPSFRadius = maxPSFRadius;
        }

        public float GetPSFRadius(int x, int y)
        {
            return DepthMap.Image[x, y, 0] * MaxPSFRadius;
        }
    }

    class ProceduralBlur : Blur
    {
        float MaxPSFRadius { get; set; }

        float widthInv;
        float heightInv;

        public ProceduralBlur(int width, int height, int maxPSFRadius)
        {
            widthInv = 1.0f / (float)width;
            heightInv = 1.0f / (float)height;
            MaxPSFRadius = maxPSFRadius;
        }

        public float GetPSFRadius(int x, int y)
        {
            // coordinates normalized to [0.0; 1.0]
            //float xNorm = x * widthInv;
            float yNorm = y * heightInv;
            return MaxPSFRadius * Math.Abs(2 * yNorm - 1);
        }
    }

    class ConstantBlur : Blur
    {
        float PSFRadius { get; set; }

        public ConstantBlur(int psfRadius)
        {
            PSFRadius = psfRadius;
        }

        public float GetPSFRadius(int x, int y)
        {
            return PSFRadius;
        }
    }
}
