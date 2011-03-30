using System;
using System.Drawing;

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

        /// <summary>
        /// Return intensity of a pixel at [x; y] as [0; 255] integer.
        /// Be kind to reads beyond image borders. In such a situation
        /// return 0 (black).
        /// </summary>
        /// <param name="image"></param>
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
