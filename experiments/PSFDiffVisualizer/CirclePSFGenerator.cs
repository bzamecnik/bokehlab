using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace PSFDeltaVisualizer
{
    public class CirclePSFGenerator
    {
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


    }
}
