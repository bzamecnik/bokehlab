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
            // - support a PSF of a non-uniform size
            // - add a normalization channel (for non-uniform PSF size)
            // *- fix situation with no blur
            // x- fix spreading at borders - add some more area to the table
            // *- implement spreading HDR images - write PFM library
            // - try single-dimensional table instead of multi-dimensional

            uint bands = inputImage.ChannelsCount;
            start = sw.ElapsedMilliseconds;
            // TODO: use a PFMImage instead
            int tableWidth = (int)width + 1;
            int tableHeight = (int)height + 1;
            float[, ,] table = new float[tableWidth, tableHeight, 3];
            Console.WriteLine("Allocating float table[{1}][{2}][3]: {0} ms",
                sw.ElapsedMilliseconds - start, tableWidth, tableHeight);

            // zero out the table
            start = sw.ElapsedMilliseconds;
            for (int x = 0; x < tableWidth; x++)
            {
                for (int y = 0; y < tableHeight; y++)
                {
                    for (int band = 0; band < bands; band++)
                    {
                        table[x, y, band] = 0;
                    }
                }
            }
            Console.WriteLine("Zeroing table: {0} ms", sw.ElapsedMilliseconds - start);

            // phase 1: distribute corners into the table
            start = sw.ElapsedMilliseconds;

            //int radius = BlurRadius;
            //float areaInv = 1.0f / ((radius * 2 + 1) * (radius * 2 + 1));

            float widthInv = 1.0f / (float)width;
            float heightInv = 1.0f / (float)height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int radius = getBlurRadius(x * widthInv, y * heightInv);
                    float areaInv = 1.0f / ((radius * 2 + 1) * (radius * 2 + 1));

                    int top = MathHelper.Clamp<int>(y - radius, 0, (int)tableHeight - 1);
                    int bottom = (int)MathHelper.Clamp<int>(y + radius + 1, 0, (int)tableHeight - 1);
                    int left = (int)MathHelper.Clamp<int>(x - radius, 0, (int)tableWidth - 1);
                    int right = (int)MathHelper.Clamp<int>(x + radius + 1, 0, (int)tableWidth - 1);

                    //float color = inputLdrImage.GetPixel(x, y).GetBrightness();
                    //Color color = inputLdrImage.GetPixel(x, y);

                    for (int band = 0; band < bands; band++)
                    {
                        float intensity = inputImage.Image[x, y, band];
                        float cornerValue = intensity * areaInv;

                        table[left, top, band] += cornerValue; // upper left
                        table[right, top, band] -= cornerValue; // upper right
                        table[left, bottom, band] -= cornerValue; // lower left
                        table[right, bottom, band] += cornerValue; // lower right
                    }
                }
            }
            Console.WriteLine("Phase 1, reading input image: {0} ms", sw.ElapsedMilliseconds - start);

            //printTable(table);

            //// draw the corners
            //for (int y = 0; y < height; y++)
            //{
            //    for (int x = 0; x < width; x++)
            //    {
            //        bool positive = table[x, y, 0] > 0;
            //        bool negative = table[x, y, 0] < 0;
            //        Color color = Color.FromArgb(positive ? 255 : 0, negative ? 255 : 0, 0);
            //        outputLdrImage.SetPixel(x, y, color);
            //    }
            //}

            // phase 2: accumulate the corners into rectangles
            start = sw.ElapsedMilliseconds;
            for (int y = 0; y < tableHeight; y++)
            {
                for (int x = 1; x < tableWidth; x++)
                {
                    for (int band = 0; band < bands; band++)
                    {
                        table[x, y, band] += table[x - 1, y, band];
                    }
                }
            }
            Console.WriteLine("Phase 2, horizontal: {0} ms", sw.ElapsedMilliseconds - start);

            start = sw.ElapsedMilliseconds;
            for (int x = 0; x < tableWidth; x++)
            {
                for (int y = 1; y < tableHeight; y++)
                {
                    for (int band = 0; band < bands; band++)
                    {
                        table[x, y, band] += table[x, y - 1, band];
                    }
                }
            }
            Console.WriteLine("Phase 2, vertical: {0} ms", sw.ElapsedMilliseconds - start);

            start = sw.ElapsedMilliseconds;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int band = 0; band < bands; band++)
                    {
                        outputImage.Image[x, y, band] = table[x, y, band];
                    }

                }
            }
            Console.WriteLine("Writing output image: {0} ms", sw.ElapsedMilliseconds - start);

            //for (int y = 0; y < height; y++)
            //{
            //    for (int x = 0; x < width; x++)
            //    {
            //        int intensity = (int)MathHelper.Clamp(table[x, y, 0] * 255.0, 0.0, 255.0);
            //        Color color = Color.FromArgb(intensity, intensity, intensity);
            //        outputLdrImage.SetPixel(x, y, color);
            //    }
            //}

            sw.Stop();
            Console.WriteLine();

            table = null;
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
