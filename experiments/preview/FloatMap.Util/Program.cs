using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using BokehLab.FloatMap;
using System.Drawing;
using System.Drawing.Imaging;

namespace BokehLab.FloatMap.Util
{
    class Program
    {
        private static void ReadAndWriteTestImage(string filename) {
            Console.WriteLine("Writing a test image.");
            FloatMapImage image = CreateTestImage();
            DisplayInfo(image);
            PrintImageContents(image);
            PortableFloatMap.SaveImage(image, filename);
            Console.WriteLine();

            Console.WriteLine("Reading a test image.");
            FloatMapImage loadedImage = PortableFloatMap.LoadImage(filename);
            DisplayInfo(loadedImage);
            PrintImageContents(loadedImage);
        }

        private static void ReadAndWriteExistingImage(string filename)
        {
            Console.WriteLine("Reading an existing image.");
            FloatMapImage image = PortableFloatMap.LoadImage(filename);
            DisplayInfo(image);
            Console.WriteLine();

            Console.WriteLine("Writing a copy of an existing image.");
            string copyFilename = filename + ".out";
            PortableFloatMap.SaveImage(image, copyFilename);
            Console.WriteLine("Reading a copy of an existing image.");
            FloatMapImage loadedImage = PortableFloatMap.LoadImage(copyFilename);
            DisplayInfo(loadedImage);
        }

        private static void DisplayInfo(FloatMapImage image) {
            Console.WriteLine("Portable float-map");
            Console.WriteLine("Pixel format: {0}", image.PixelFormat);
            Console.WriteLine("Width: {0}, height: {1}", image.Width, image.Height);
            Console.WriteLine("Scale: {0}", image.Scale);
        }

        public static FloatMapImage CreateTestImage()
        {
            FloatMapImage image = new FloatMapImage(5, 7, PixelFormat.Greyscale);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    image.Image[x, y, 0] = (x + 1) + (10 * y) + 0.5f;
                }
            }
            return image;
        }

        private static void PrintImageContents(FloatMapImage image)
        {
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Console.Write("[");
                    for (int band = 0; band < image.ColorChannelsCount; band++)
                    {
                        Console.Write("{0}", String.Format(CultureInfo.CreateSpecificCulture("en-US"), "{0}", image.Image[x, y, band]));
                        if (band < image.ColorChannelsCount - 1)
                        {
                            Console.Write(", ");
                        }
                    }
                    Console.Write("]");
                    if (x < image.Width - 1)
                    {
                        Console.Write(", ");
                    }
                }
                Console.WriteLine();
            }
        }

        private static FloatMapImage LoadFile(string filename)
        {
            if (filename.ToLower().EndsWith(".pfm")) {
                return PortableFloatMap.LoadImage(filename);
            } else if (filename.ToLower().EndsWith(".png")) {
                return ((Bitmap)Bitmap.FromFile(filename)).ToFloatMap();
            }
            else if (filename.ToLower().EndsWith(".jpg"))
            {
                return ((Bitmap)Bitmap.FromFile(filename)).ToFloatMap();
            }
            throw new ArgumentException("Unknown file format: {0}", filename);
        }

        private static void SaveFile(FloatMapImage image, string filename)
        {            
            if (filename.ToLower().EndsWith(".pfm"))
            {
                image.SaveImage(filename);
            }
            else if (filename.ToLower().EndsWith(".png"))
            {
                image.ToBitmap().Save(filename, ImageFormat.Png);
            }
            else
            {
                throw new ArgumentException(String.Format("Unknown file format: {0}", filename));
            }
        }

        private static void Composite(string fileA, string fileB, string outputFile)
        {
            FloatMapImage imageA = LoadFile(fileA);
            Console.WriteLine(fileA);
            DisplayInfo(imageA);
            FloatMapImage imageB = LoadFile(fileB);
            Console.WriteLine(fileB);
            DisplayInfo(imageB);

            SaveFile(imageA.Over(imageB), outputFile);
        }

        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                return;
            }

            try
            {
                Composite(args[0], args[1], args[2]);

                //FloatMapImage image = PortableFloatMap.LoadImage(args[0]);
                //DisplayInfo(image);

                //ReadAndWriteExistingImage(args[0]);
                //ReadAndWriteATestImage(args[0]);
            }
            catch (OutOfMemoryException ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex.StackTrace);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
