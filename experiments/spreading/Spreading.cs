using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.IO;
using System.IO.Compression;

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

            if (outputImage == null)
            {
                outputImage = new Bitmap(width, height, inputImage.PixelFormat);
            }

            if (width < 1 || height < 1) return null;

            // TODO:
            // - use more color channels: RGB, RGBA
            // - directly access the image via LockBits and pointers

            double[,] table = new double[width, height];

            // zero out the table
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    table[x, y] = 0;
                }
            }

            // phase 1: distribute corners into the table
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int radius = getBlurRadius(x, y);
                    double area = (radius * 2 + 1) * (radius * 2 + 1);

                    double color = inputImage.GetPixel(x, y).GetBrightness();
                    double cornerValue = color / area;
                    // DEBUG:
                    double boost = 2.0; //10.0;
                    cornerValue *= boost;

                    int top = clamp(y - radius, 0, height - 1);
                    int bottom = clamp(y + radius, 0, height - 1);
                    int left = clamp(x - radius, 0, width - 1);
                    int right = clamp(x + radius, 0, width - 1);

                    table[left, top] += cornerValue; // upper left
                    table[right, top] -= cornerValue; // upper right
                    table[left, bottom] -= cornerValue; // lower left
                    table[right, bottom] += cornerValue; // lower right
                }
            }

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
            for (int y = 0; y < height; y++)
            {
                for (int x = 1; x < width; x++)
                {
                    table[x, y] += table[x - 1, y];
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 1; y < height; y++)
                {
                    table[x, y] += table[x, y - 1];
                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int intensity = (int)(clamp(table[x, y] * 255.0, 0.0, 255.0));
                    Color color = Color.FromArgb(intensity, intensity, intensity);
                    outputImage.SetPixel(x, y, color);
                }
            }

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
