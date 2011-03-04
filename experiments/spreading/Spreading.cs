using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace spreading
{
    class RectangleSpreadingFilter
    {
        public static readonly int DEFAULT_BLUR_RADIUS = 1;
        public int BlurRadius { get; set; }

        public RectangleSpreadingFilter() {
            BlurRadius = DEFAULT_BLUR_RADIUS;
        }

        public Bitmap SpreadPSF(Bitmap inputImage, Bitmap outputImage)
        {
            if (inputImage == null) return null;

            int width = inputImage.Width;
            int height = inputImage.Height;

            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();
            long start = 0;

            if (outputImage == null)
            {
                start = sw.ElapsedMilliseconds;
                outputImage = new Bitmap(width, height, inputImage.PixelFormat);
                Console.WriteLine("Creating new Bitmap: {0} ms", sw.ElapsedMilliseconds - start);
            }

            if (width < 1 || height < 1) return null;

            // TODO:
            // - add a normalization channel
            // - fix situation with no blur

            int bands = 3;
            start = sw.ElapsedMilliseconds;
            double[,,] table = new double[width, height, 3];
            Console.WriteLine("Allocating float table[{1}][{2}][3]: {0} ms", sw.ElapsedMilliseconds - start, width, height);

            // zero out the table
            start = sw.ElapsedMilliseconds;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
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
            BitmapData inputData = inputImage.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, inputImage.PixelFormat);
            Console.WriteLine("Locking input image: {0} ms", sw.ElapsedMilliseconds - start);

            start = sw.ElapsedMilliseconds;
            unsafe
            {
                int radius = BlurRadius;
                double areaInv = 1.0 / ((radius * 2 + 1) * (radius * 2 + 1));
                double colorNormalizationFactor = 1.0 / 255.0;

                for (int y = 0; y < height; y++)
                {
                    byte* inputRow = (byte*)inputData.Scan0 + (y * inputData.Stride);
                    for (int x = 0; x < width; x++)
                    {
                        //int radius = getBlurRadius(x, y);

                        int top = clamp(y - radius, 0, height - 1);
                        int bottom = clamp(y + radius, 0, height - 1);
                        int left = clamp(x - radius, 0, width - 1);
                        int right = clamp(x + radius, 0, width - 1);

                        //double color = inputImage.GetPixel(x, y).GetBrightness();
                        //Color color = inputImage.GetPixel(x, y);

                        for (int band = 2; band >= 0; band--)
                        {
                            byte color = inputRow[x * 3 + band];
                            double intensity = color * colorNormalizationFactor;
                            double cornerValue = intensity * areaInv;

                            // DEBUG:
                            //double boost = 1.0;// 20.0;
                            //cornerValue *= boost;

                            table[left, top, band] += cornerValue; // upper left
                            table[right, top, band] -= cornerValue; // upper right
                            table[left, bottom, band] -= cornerValue; // lower left
                            table[right, bottom, band] += cornerValue; // lower right
                        }
                    }
                }
            }
            Console.WriteLine("Phase 1, reading input image: {0} ms", sw.ElapsedMilliseconds - start);

            start = sw.ElapsedMilliseconds;
            inputImage.UnlockBits(inputData);
            Console.WriteLine("Unlocking input image: {0} ms", sw.ElapsedMilliseconds - start);

            //printTable(table);

            //// draw the corners
            //for (int y = 0; y < height; y++)
            //{
            //    for (int x = 0; x < width; x++)
            //    {
            //        bool positive = table[x, y] > 0;
            //        bool negative = table[x, y] < 0;
            //        Color color = Color.FromArgb(positive ? 255 : 0, negative ? 255 : 0, 0);
            //        outputImage.SetPixel(x, y, color);
            //    }
            //}

            // phase 2: accumulate the corners into rectangles
            start = sw.ElapsedMilliseconds;
            for (int y = 0; y < height; y++)
            {
                for (int x = 1; x < width; x++)
                {
                    for (int band = 0; band < bands; band++)
                    {
                        table[x, y, band] += table[x - 1, y, band];
                    }
                }
            }
            Console.WriteLine("Phase 2, horizontal: {0} ms", sw.ElapsedMilliseconds - start);

            start = sw.ElapsedMilliseconds;
            for (int x = 0; x < width; x++)
            {
                for (int y = 1; y < height; y++)
                {
                    for (int band = 0; band < bands; band++)
                    {
                        table[x, y, band] += table[x, y - 1, band];
                    }
                }
            }
            Console.WriteLine("Phase 2, vertical: {0} ms", sw.ElapsedMilliseconds - start);

            start = sw.ElapsedMilliseconds;
            BitmapData outputData = outputImage.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, outputImage.PixelFormat);
            Console.WriteLine("Locking output image: {0} ms", sw.ElapsedMilliseconds - start);

            start = sw.ElapsedMilliseconds;
            unsafe
            {
                for (int y = 0; y < height; y++)
                {
                    byte* outputRow = (byte*)outputData.Scan0 + (y * outputData.Stride);
                    for (int x = 0; x < width; x++)
                    {
                        for (int band = 2; band >= 0; band--)
                        {
                            double color = clamp(table[x, y, band] * 255.0, 0.0, 255.0);
                            outputRow[x * 3 + band] = (byte)color;
                        }

                    }
                }
            }
            Console.WriteLine("Writing output image: {0} ms", sw.ElapsedMilliseconds - start);

            start = sw.ElapsedMilliseconds;
            outputImage.UnlockBits(outputData);
            Console.WriteLine("Unlocking output image: {0} ms", sw.ElapsedMilliseconds - start);

            sw.Stop();
            Console.WriteLine();

            table = null;
            return outputImage;
        }

        private int getBlurRadius(int x, int y)
        {
            return BlurRadius;
        }

        private double clamp(double number, double min, double max)
        {
            if (number < min) return min;
            if (number > max) return max;
            return number;
        }

        private int clamp(int number, int min, int max)
        {
            if (number < min) return min;
            if (number > max) return max;
            return number;
        }

        private double clamp(double number)
        {
            return clamp(number, 0.0, 1.0);
        }

        private void printTable(double[,] table)
        {
            int width = table.GetLength(0);
            int height = table.GetLength(1);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    double value = table[x, y];
                    if (Math.Abs(value) > 0)
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
