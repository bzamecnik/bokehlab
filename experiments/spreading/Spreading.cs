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
        public static readonly int DEFAULT_BLUR_RADIUS = 1;
        public int BlurRadius { get; set; }

        public RectangleSpreadingFilter()
        {
            BlurRadius = DEFAULT_BLUR_RADIUS;
        }

        public PFMImage SpreadPSF(PFMImage inputImage, PFMImage outputImage)
        {
            if (inputImage == null) return null;

            uint width = inputImage.Width;
            uint height = inputImage.Height;

            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();
            long start = 0;

            if (outputImage == null)
            {
                start = sw.ElapsedMilliseconds;
                outputImage = new PFMImage(width, height, inputImage.PixelFormat);
                Console.WriteLine("Creating new Bitmap: {0} ms", sw.ElapsedMilliseconds - start);
            }

            if (width < 1 || height < 1) return null;

            // TODO:
            // *- support a PSF of a non-uniform size
            // - add a normalization channel (for non-uniform PSF size)
            // *- fix situation with no blur
            // x- fix spreading at borders - add some more area to the table
            // *- implement spreading HDR images - write PFM library
            // - try single-dimensional table instead of multi-dimensional
            // - shouldn't the table be of the same size as the input image?
            // - implment non-integer PSF size - interpolation between two integer PSFs

            uint bands = inputImage.ChannelsCount;

            start = sw.ElapsedMilliseconds;
            PFMImage spreadingTable = new PFMImage(width + 1, height + 1, inputImage.PixelFormat);
            Console.WriteLine("Allocating float table[{1}][{2}][3]: {0} ms",
                sw.ElapsedMilliseconds - start, width + 1, height + 1);

            // TODO: the spreading table and normalization channel could be squashed into
            // one image for better locality
            PFMImage normalizationTable = new PFMImage(width + 1, height + 1, PixelFormat.Greyscale);

            // initialize the spreading table to 0.0 and the normalization channel to 1.0
            start = sw.ElapsedMilliseconds;
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
            Console.WriteLine("Zeroing table: {0} ms", sw.ElapsedMilliseconds - start);

            // phase 1: distribute corners into the table
            start = sw.ElapsedMilliseconds;

            float widthInv = 1.0f / (float)width;
            float heightInv = 1.0f / (float)height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int radius = getBlurRadius(x * widthInv, y * heightInv);
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
            Console.WriteLine("Phase 1, reading input image: {0} ms", sw.ElapsedMilliseconds - start);

            // phase 2: accumulate the corners into rectangles
            start = sw.ElapsedMilliseconds;
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
            Console.WriteLine("Phase 2, horizontal: {0} ms", sw.ElapsedMilliseconds - start);

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
            Console.WriteLine("Phase 2, vertical: {0} ms", sw.ElapsedMilliseconds - start);

            start = sw.ElapsedMilliseconds;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float normalization = 1 / normalizationTable.Image[x, y, 0];
                    for (int band = 0; band < bands; band++)
                    {
                        outputImage.Image[x, y, band] = spreadingTable.Image[x, y, band] * normalization;
                        //outputImage.Image[x, y, band] = spreadingTable.Image[x, y, band];
                        //outputImage.Image[x, y, band] = normalization;
                    }

                }
            }
            Console.WriteLine("Writing output image: {0} ms", sw.ElapsedMilliseconds - start);

            //for (int y = 0; y < height; y++)
            //{
            //    for (int x = 0; x < width; x++)
            //    {
            //        int intensity = (int)MathHelper.Clamp(spreadingTable.Image[x, y, 0] * 255.0, 0.0, 255.0);
            //        Color color = Color.FromArgb(intensity, intensity, intensity);
            //        outputLdrImage.SetPixel(x, y, color);
            //    }
            //}

            sw.Stop();
            Console.WriteLine();

            // TODO: dispose the spreadingTable and normalizationTable

            return outputImage;
        }

        //private int getBlurRadius(int x, int y)
        //{
        //    return BlurRadius;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">normalized [0.0; 1.0]</param>
        /// <param name="y">normalized [0.0; 1.0]</param>
        /// <returns></returns>
        private int getBlurRadius(float x, float y)
        {
            return (int)(BlurRadius * Math.Abs(2 * y - 1));
        }

        private void printTable(float[,] table)
        {
            int width = table.GetLength(0);
            int height = table.GetLength(1);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float value = table[x, y];
                    if (Math.Abs(value) > 0.0f)
                    {
                        Console.Write("{0}, ", value);
                    }
                    else
                    {
                        Console.Write(" , ", value);
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
