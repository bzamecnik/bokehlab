using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using libpfm;
using mathHelper;

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

            if (outputImage == null)
            {
                outputImage = new PFMImage(width, height, inputImage.PixelFormat);
            }

            // TODO:
            // *- support a PSF of a non-uniform size
            // *- add a normalization channel (for non-uniform PSF size)
            // *- fix situation with no blur
            // x- fix spreading at borders - add some more area to the table
            //    *- fixed by normalization
            // *- implement spreading HDR images - write PFM library
            // - try single-dimensional table instead of multi-dimensional
            // - shouldn't the table be of the same size as the input image?
            //   - now it seems it has to be larger by 1px
            // - implment non-integer PSF size - interpolation between two integer PSFs

            PFMImage spreadingTable = new PFMImage(width + 1, height + 1, inputImage.PixelFormat);

            // TODO: the spreading table and normalization channel could be squashed into
            // one image for better locality
            PFMImage normalizationTable = new PFMImage(width + 1, height + 1, PixelFormat.Greyscale);

            // initialize the spreading table to 0.0 and the normalization channel to 1.0
            InitializeTables(spreadingTable, normalizationTable);

            Blur blur = CreateBlurFunction(depthMap, width, height);

            // phase 1: distribute corners into the table
            Spread(inputImage, spreadingTable, normalizationTable, blur);

            // phase 2: accumulate the corners into rectangles
            Integrate(spreadingTable, normalizationTable);

            Normalize(spreadingTable, normalizationTable, outputImage);

            Console.WriteLine();

            // TODO: dispose the spreadingTable and normalizationTable

            return outputImage;
        }

        private static void InitializeTables(PFMImage spreadingTable, PFMImage normalizationTable)
        {
            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();
            uint bands = spreadingTable.ChannelsCount;
            for (int x = 0; x < spreadingTable.Width; x++)
            {
                for (int y = 0; y < spreadingTable.Height; y++)
                {
                    for (int band = 0; band < bands; band++)
                    {
                        spreadingTable.Image[x, y, band] = 0;
                    }
                    normalizationTable.Image[x, y, 0] = 0;
                }
            }
            Console.WriteLine("Initializing spreading and normalization tables: {0} ms", sw.ElapsedMilliseconds);
        }

        private static void Spread(PFMImage inputImage, PFMImage spreadingTable, PFMImage normalizationTable, Blur blur)
        {
            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();
            
            uint bands = inputImage.ChannelsCount;
            uint width = inputImage.Width;
            uint height = inputImage.Height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int radius = (int)blur.GetPSFRadius(x, y);
                    float areaInv = 1.0f / ((radius * 2 + 1) * (radius * 2 + 1));

                    int top = MathHelper.Clamp<int>(y - radius, 0, (int)spreadingTable.Height - 1);
                    int bottom = (int)MathHelper.Clamp<int>(y + radius + 1, 0, (int)spreadingTable.Height - 1);
                    int left = (int)MathHelper.Clamp<int>(x - radius, 0, (int)spreadingTable.Width - 1);
                    int right = (int)MathHelper.Clamp<int>(x + radius + 1, 0, (int)spreadingTable.Width - 1);

                    for (int band = 0; band < bands; band++)
                    {
                        float intensity = inputImage.Image[x, y, band];
                        float cornerValue = intensity * areaInv;

                        spreadingTable.Image[left, top, band] += cornerValue; // upper left
                        spreadingTable.Image[right, top, band] -= cornerValue; // upper right
                        spreadingTable.Image[left, bottom, band] -= cornerValue; // lower left
                        spreadingTable.Image[right, bottom, band] += cornerValue; // lower right
                    }
                    // Note: intensity for the normalization is 1.0
                    normalizationTable.Image[left, top, 0] += areaInv; // upper left
                    normalizationTable.Image[right, top, 0] -= areaInv; // upper right
                    normalizationTable.Image[left, bottom, 0] -= areaInv; // lower left
                    normalizationTable.Image[right, bottom, 0] += areaInv; // lower right
                }
            }
            Console.WriteLine("Phase 1, spreading: {0} ms", sw.ElapsedMilliseconds);
        }

        private static void Integrate(PFMImage spreadingTable, PFMImage normalizationTable)
        {
            long start = 0;
            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();

            uint bands = spreadingTable.ChannelsCount;
            for (int y = 0; y < spreadingTable.Height; y++)
            {
                for (int x = 1; x < spreadingTable.Width; x++)
                {
                    for (int band = 0; band < bands; band++)
                    {
                        spreadingTable.Image[x, y, band] += spreadingTable.Image[x - 1, y, band];
                    }
                    normalizationTable.Image[x, y, 0] += normalizationTable.Image[x - 1, y, 0];
                }
            }
            Console.WriteLine("Phase 2, horizontal integration: {0} ms", sw.ElapsedMilliseconds);

            start = sw.ElapsedMilliseconds;
            for (int x = 0; x < spreadingTable.Width; x++)
            {
                for (int y = 1; y < spreadingTable.Height; y++)
                {
                    for (int band = 0; band < bands; band++)
                    {
                        spreadingTable.Image[x, y, band] += spreadingTable.Image[x, y - 1, band];
                    }
                    normalizationTable.Image[x, y, 0] += normalizationTable.Image[x, y - 1, 0];
                }
            }
            Console.WriteLine("Phase 2, vertical intergration: {0} ms", sw.ElapsedMilliseconds - start);
        }

        private static void Normalize(PFMImage spreadingTable, PFMImage normalizationTable, PFMImage outputImage)
        {
            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();

            uint bands = outputImage.ChannelsCount;
            for (int y = 0; y < outputImage.Height; y++)
            {
                for (int x = 0; x < outputImage.Width; x++)
                {
                    float normalization = 1 / normalizationTable.Image[x, y, 0];
                    for (int band = 0; band < bands; band++)
                    {
                        outputImage.Image[x, y, band] = spreadingTable.Image[x, y, band] * normalization;
                    }
                }
            }
            Console.WriteLine("Normalizing to output image: {0} ms", sw.ElapsedMilliseconds);
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
            return blur;
        }
    }

    interface Blur {
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

    class ProceduralBlur : Blur {
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
}
