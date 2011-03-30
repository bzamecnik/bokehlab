using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace PSFDiffVisualizer
{
    public partial class Form1 : Form
    {
        int Radius { get; set; }
        bool AntiAliasEnabled { get; set; }
        bool ShowDiffX { get; set; }

        int ScaleFactor { get; set; }

        Bitmap psf;
        Bitmap psfDiffX;

        Bitmap psfScaled;
        Bitmap psfDiffXScaled;

        SmoothingMode SmoothingMode
        {
            get
            {
                return AntiAliasEnabled ? SmoothingMode.AntiAlias : SmoothingMode.None;
            }
        }

        public Form1()
        {
            InitializeComponent();
            ScaleFactor = 1;
            Radius = 20;
            psfRadiusNumeric.Value = Radius;
            AntiAliasEnabled = false;
            antiAliasCheckBox.Checked = AntiAliasEnabled;
            ShowDiffX = true;
            diffXcheckBox.Checked = ShowDiffX;
            

            psfPictureBox.SizeMode = PictureBoxSizeMode.CenterImage;

            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            Recompute();
        }

        //private void panel1_Paint(object sender, PaintEventArgs e)
        //{
        //    int right = e.ClipRectangle.Width / 2;
        //    int bottom = e.ClipRectangle.Height / 2;

        //    e.Graphics.TranslateTransform(right, bottom);
        //    e.Graphics.FillRectangle(Brushes.Black, e.Graphics.ClipBounds);

        //    e.Graphics.DrawLine(Pens.Gray, new Point(-right, 0), new Point(right, 0));
        //    e.Graphics.DrawLine(Pens.Gray, new Point(0, -bottom), new Point(0, bottom));

        //    e.Graphics.DrawImage(ShowDiffX ? psfDiffX : psf, new Point(-Radius, -Radius));
        //}

        private void Recompute()
        {
            if (psf != null)
            {
                psf.Dispose();
            }
            if (psfDiffX != null)
            {
                psfDiffX.Dispose();
            }

            psf = CreateCircle(Radius, SmoothingMode);
            psfDiffX = DifferenceX(psf);

            ScaleImages();
            SetImageToPictureBox();
        }

        private void SetImageToPictureBox()
        {
            psfPictureBox.Image = ShowDiffX ? psfDiffXScaled : psfScaled;
        }

        private void ScaleImages()
        {
            if ((psf == null) || (psfDiffX == null))
            {
                return;
            }
            if (psfScaled != null)
            {
                psfScaled.Dispose();
            }
            if (psfDiffXScaled != null)
            {
                psfDiffXScaled.Dispose();
            }

            psfScaled = ScaleImage(psf);
            psfDiffXScaled = ScaleImage(psfDiffX);
        }

        private Bitmap ScaleImage(Bitmap inputImage)
        {
            int width = ScaleFactor * inputImage.Width;
            int height = ScaleFactor * inputImage.Height;
            Bitmap scaledImage = new Bitmap(width, height, inputImage.PixelFormat);
            using (Graphics g = Graphics.FromImage(scaledImage))
            {
                g.FillRectangle(Brushes.Black, 0, 0, width, height);
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                // Note: adding ScaleFactor is really odd!
                //g.DrawImage(inputImage, 0, 0, width + ScaleFactor, height + ScaleFactor);
                g.DrawImage(inputImage, 0, 0, width, height);                
            }
            return scaledImage;
        }

        private Bitmap CreateCircle(int radius, SmoothingMode smoothingMode)
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

        private Bitmap DifferenceX(Bitmap inputImage)
        {
            int width = inputImage.Width + 1;
            int height = inputImage.Height;
            Bitmap diffImage = new Bitmap(width, height, inputImage.PixelFormat);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color diffColor = VisualizeSubtract(
                        GetPixel(inputImage, x ,y),
                        GetPixel(inputImage, x - 1, y));
                    diffImage.SetPixel(x, y, diffColor);
                }
            }
            return diffImage;
        }

        private int GetPixel(Bitmap image, int x, int y)
        {
            if ((x >= 0) && (x < image.Width) &&
                (y >= 0) && (x < image.Height))
            {
                return (int) (image.GetPixel(x, y).GetBrightness() * 255);
            }
            else
            {
                return 0;
            }
        }

        // a - b
        private Color VisualizeSubtract(int a, int b)
        {
            int intensity = a - b;
            int alpha = (intensity == 0) ? 0 : 255;
            return Color.FromArgb(alpha, Math.Max(-intensity, 0), Math.Max(intensity, 0), 0);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Radius = (int)psfRadiusNumeric.Value;
            Recompute();
        }

        private void antiAliasCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            AntiAliasEnabled = antiAliasCheckBox.Checked;
            Recompute();
        }

        private void diffXcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ShowDiffX = diffXcheckBox.Checked;
            SetImageToPictureBox();
        }

        private void scaleFactorNumeric_ValueChanged(object sender, EventArgs e)
        {
            ScaleFactor = (int)Math.Pow(2, (int)scaleFactorNumeric.Value);
            ScaleImages();
            SetImageToPictureBox();
        }
    }
}
