using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libpfm;

namespace pfmUtil
{
    class Program
    {
        public static void DisplayInfo(string filename) {
            PFMImage image = PFMImage.LoadImage(filename);
            Console.WriteLine("Portable float-map");
            Console.WriteLine("Pixel format: {0}", image.PixelFormat);
            Console.WriteLine("Width: {0}, height: {1}", image.Width, image.Height);
            Console.WriteLine("Scale: {0}", image.Scale);
            Console.WriteLine("Endianness: {0}", image.Endianness);
        }

        static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                return;
            }

            try
            {
                DisplayInfo(args[0]);
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
