using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using spreading.PSF.Perimeter;

namespace PSFDeltaVisualizer
{
    public class PSFDeltaVisualizer
    {
        /// <summary>
        /// Differentiate the image in horizontal direction.
        /// </summary>
        /// <param name="inputImage"></param>
        /// <returns></returns>
        public static Bitmap DiffImageHorizontally(Bitmap inputImage)
        {
            int width = inputImage.Width + 1;
            int height = inputImage.Height;
            Bitmap diffImage = new Bitmap(width, height, inputImage.PixelFormat);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color diffColor = VisualizeDelta(
                        GetPixel(inputImage, x, y) -
                        GetPixel(inputImage, x - 1, y));
                    diffImage.SetPixel(x, y, diffColor);
                }
            }
            return diffImage;
        }

        public static Bitmap VisualizePSFDeltas(List<Delta> deltas, int width, int height)
        {
            Bitmap image = new Bitmap(width + 1, height,
                PixelFormat.Format32bppArgb);
            foreach (Delta delta in deltas)
            {                
                image.SetPixel(delta.x, delta.y, VisualizeDelta((int)(delta.value * 255)));
            }
            return image;
        }


        /// <summary>
        /// Return intensity of a pixel at [x; y] as [0; 255] integer.
        /// Be kind to reads beyond scaledImage borders. In such a situation
        /// return 0 (black).
        /// </summary>
        /// <param name="scaledImage"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static int GetPixel(Bitmap image, int x, int y)
        {
            if ((x >= 0) && (x < image.Width) &&
                (y >= 0) && (x < image.Height))
            {
                return (int)(image.GetPixel(x, y).GetBrightness() * 255);
            }
            else
            {
                return 0;
            }
        }

        // Visualize positive delta in green and negative delta in red.
        private static Color VisualizeDelta(int intensity)
        {
            int alpha = (intensity == 0) ? 0 : 255;
            return Color.FromArgb(alpha, Math.Max(-intensity, 0), Math.Max(intensity, 0), 0);
        }
    }
}
