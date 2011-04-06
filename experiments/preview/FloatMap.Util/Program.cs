using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using BokehLab.FloatMap;

namespace BokehLab.FloatMap.Util
{
    class Program
    {
        private static void ReadAndWriteTestImage(string filename) {
            Console.WriteLine("Writing a test hdrImage.");
            FloatMapImage image = CreateTestImage();
            DisplayInfo(image);
            PrintImageContents(image);
            PortableFloatMap.SaveImage(image, filename);
            Console.WriteLine();

            Console.WriteLine("Reading a test hdrImage.");
            FloatMapImage loadedImage = PortableFloatMap.LoadImage(filename);
            DisplayInfo(loadedImage);
            PrintImageContents(loadedImage);
        }

        private static void ReadAndWriteExistingImage(string filename)
        {
            Console.WriteLine("Reading an existing hdrImage.");
            FloatMapImage image = PortableFloatMap.LoadImage(filename);
            DisplayInfo(image);
            Console.WriteLine();

            Console.WriteLine("Writing a copy of an existing hdrImage.");
            string copyFilename = filename + ".out";
            PortableFloatMap.SaveImage(image, copyFilename);
            Console.WriteLine("Reading a copy of an existing hdrImage.");
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
                    for (int band = 0; band < image.ChannelsCount; band++)
                    {
                        Console.Write("{0}", String.Format(CultureInfo.CreateSpecificCulture("en-US"), "{0}", image.Image[x, y, band]));
                        if (band < image.ChannelsCount - 1)
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

        static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                return;
            }

            try
            {
                ReadAndWriteExistingImage(args[0]);
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
