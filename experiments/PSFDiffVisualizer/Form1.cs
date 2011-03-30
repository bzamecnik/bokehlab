using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PSFDeltaVisualizer
{
    // TODO:
    // - generate PDFDescription instances instead of just images
    // - fix nearest-neighbor scaling
    // - support loading arbitrary images
    // - rasterize circle without needing System.Drawing FillEllipse()

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

            psf = CirclePSFGenerator.CreateCircle(Radius, SmoothingMode);
            psfDiffX = PSFDeltaVisualizer.DiffImageHorizontally(psf);

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
