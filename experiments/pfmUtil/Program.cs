using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using libpfm;

namespace pfmUtil
{
    class Program
    {
        private static void ReadAndWriteTestImage(string filename) {
            Console.WriteLine("Writing a test hdrImage.");
            PFMImage image = CreateTestImage();
            DisplayInfo(image);
            PrintImageContents(image);
            image.SaveImage(filename);
            Console.WriteLine();

            Console.WriteLine("Reading a test hdrImage.");
            PFMImage loadedImage = PFMImage.LoadImage(filename);
            DisplayInfo(loadedImage);
            PrintImageContents(loadedImage);
        }

        private static void ReadAndWriteExistingImage(string filename)
        {
            Console.WriteLine("Reading an existing hdrImage.");
            PFMImage image = PFMImage.LoadImage(filename);
            DisplayInfo(image);
            Console.WriteLine();

            Console.WriteLine("Writing a copy of an existing hdrImage.");
            string copyFilename = filename + ".out";
            image.SaveImage(copyFilename);
            Console.WriteLine("Reading a copy of an existing hdrImage.");
            PFMImage loadedImage = PFMImage.LoadImage(copyFilename);
            DisplayInfo(loadedImage);
        }

        private static void DisplayInfo(PFMImage image) {
            Console.WriteLine("Portable float-map");
            Console.WriteLine("Pixel format: {0}", image.PixelFormat);
            Console.WriteLine("Width: {0}, height: {1}", image.Width, image.Height);
            Console.WriteLine("Scale: {0}", image.Scale);
            Console.WriteLine("Endianness: {0}", image.Endianness);
        }

        public static PFMImage CreateTestImage()
        {
            PFMImage image = new PFMImage(5, 7, PixelFormat.Greyscale);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    image.Image[x, y, 0] = (x + 1) + (10 * y) + 0.5f;
                }
            }
            return image;
        }

        private static void PrintImageContents(PFMImage image)
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
