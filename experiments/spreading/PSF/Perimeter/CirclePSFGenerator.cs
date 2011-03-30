using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace spreading.PSF.Perimeter
{
    // TODO: sample the deltas using a circle eqation instead of
    // Windows.Drawing rasterization

    public class CirclePSFGenerator : IPSFGenerator
    {
        /// <summary>
        /// Generate PSF deltas for radii [0; maxRadius]
        /// </summary>
        /// <param name="maxRadius"></param>
        /// <returns></returns>
        public PSFDescription GeneratePSF(int maxRadius)
        {
            PSFDescription desc = new PSFDescription();
            desc.deltasByRadius = new Delta[maxRadius + 1][];
            for (int radius = 0; radius <= maxRadius; radius++)
            {
                Bitmap psfImage = CirclePSFGenerator.CreateCircle(radius, SmoothingMode.None);

                List<Delta> deltas = DiffHorizontally(psfImage);
                desc.deltasByRadius[radius] = deltas.ToArray();
            }
            return desc;
        }

        public static List<Delta> DiffHorizontally(Bitmap psfImage)
        {
            List<Delta> deltas = new List<Delta>();
            int width = psfImage.Width + 1;
            int height = psfImage.Height;
            Bitmap diffImage = new Bitmap(width, height, psfImage.PixelFormat);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int delta = GetPixel(psfImage, x, y) - GetPixel(psfImage, x - 1, y);
                    if (delta != 0)
                    {
                        deltas.Add(new Delta(x, y, (float)delta / 255.0f));
                    }
                }
            }
            return deltas;
        }

        /// <summary>
        /// Rasterize a circle of specified radius in an image.
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="smoothingMode"></param>
        /// <returns></returns>
        public static Bitmap CreateCircle(int radius, SmoothingMode smoothingMode)
        {
            int size = Math.Max(2 * radius, 1);
            Bitmap image = new Bitmap(size, size, PixelFormat.Format32bppArgb);

            Graphics g = Graphics.FromImage(image);

            g.SmoothingMode = smoothingMode;
            g.FillRectangle(Brushes.Black, 0, 0, size, size);
            g.FillEllipse(Brushes.White, new RectangleF(
                new PointF(-0.5f, -0.5f), new SizeF(size, size)));

            g.Dispose();
            return image;
        }

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
    }
}
