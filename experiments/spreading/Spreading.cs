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

            int bands = 3;
            double[,,] table = new double[width, height, 3];

            // zero out the table
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

            double[] intensities = new double[bands];

            // phase 1: distribute corners into the table
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int radius = getBlurRadius(x, y);
                    double area = (radius * 2 + 1) * (radius * 2 + 1);

                    int top = clamp(y - radius, 0, height - 1);
                    int bottom = clamp(y + radius, 0, height - 1);
                    int left = clamp(x - radius, 0, width - 1);
                    int right = clamp(x + radius, 0, width - 1);

                    //double color = inputImage.GetPixel(x, y).GetBrightness();
                    Color color = inputImage.GetPixel(x, y);
                    
                    intensities[0] = color.R / 255.0;
                    intensities[1] = color.G / 255.0;
                    intensities[2] = color.B / 255.0;

                    for (int band = 0; band < bands; band++)
                    {

                        double cornerValue = intensities[band] / area;
                        // DEBUG:
                        double boost = 1.0; // 20.0;
                        cornerValue *= boost;

                        table[left, top, band] += cornerValue; // upper left
                        table[right, top, band] -= cornerValue; // upper right
                        table[left, bottom, band] -= cornerValue; // lower left
                        table[right, bottom, band] += cornerValue; // lower right
                    }
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
                    for (int band = 0; band < bands; band++)
                    {
                        table[x, y, band] += table[x - 1, y, band];
                    }
                }
            }

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

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int band = 0; band < bands; band++)
                    {
                        intensities[band] = clamp(table[x, y, band] * 255.0, 0.0, 255.0);
                    }
                    Color color = Color.FromArgb((int)intensities[0], (int)intensities[1], (int)intensities[2]);
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
