using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using spreading.PSF.Perimeter;

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

        Bitmap PsfImage { get; set; }
        Bitmap PsfDiffXImage { get; set; }

        Bitmap PsfImageScaled { get; set; }
        Bitmap PsfDiffXImageScaled { get; set; }

        List<Delta> Deltas { get; set; }

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
            if (PsfImage != null)
            {
                PsfImage.Dispose();
            }
            if (PsfDiffXImage != null)
            {
                PsfDiffXImage.Dispose();
            }

            PsfImage = CirclePSFGenerator.CreateCircle(Radius, SmoothingMode);
            Deltas = CirclePSFGenerator.DiffHorizontally(PsfImage);
            RedrawDeltas();
            //PsfDiffXImage = PSFDeltaVisualizer.DiffImageHorizontally(PsfImage);

            ScaleImages();

            SetImageToPictureBox();
        }

        private void RedrawDeltas()
        {
            PsfDiffXImage = PSFDeltaVisualizer.VisualizePSFDeltas(Deltas, PsfImage.Width, PsfImage.Height);
        }

        private void ScaleImages()
        {
            if ((PsfImage == null) || (PsfDiffXImage == null))
            {
                return;
            }
            if (PsfImageScaled != null)
            {
                PsfImageScaled.Dispose();
            }
            if (PsfDiffXImageScaled != null)
            {
                PsfDiffXImageScaled.Dispose();
            }

            PsfImageScaled = ScaleImage(PsfImage, ScaleFactor);
            PsfDiffXImageScaled = ScaleImage(PsfDiffXImage, ScaleFactor);
        }


        public static Bitmap ScaleImage(Bitmap inputImage, int scalingFactor)
        {
            int width = inputImage.Width;
            int height = inputImage.Height;
            Bitmap scaledImage = new Bitmap(scalingFactor * width, scalingFactor * height,
                inputImage.PixelFormat);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color color = inputImage.GetPixel(x, y);
                    for (int offsetY = 0; offsetY < scalingFactor; offsetY++)
                    {
                        for (int offsetX = 0; offsetX < scalingFactor; offsetX++)
                        {
                            scaledImage.SetPixel(
                                scalingFactor * x + offsetX,
                                scalingFactor * y + offsetY,
                                color);
                        }
                    }
                }
            }
            return scaledImage;
        }

        private void SetImageToPictureBox()
        {
            psfPictureBox.Image = ShowDiffX ? PsfDiffXImageScaled : PsfImageScaled;
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
            ScaleFactor = (int)scaleFactorNumeric.Value;
            ScaleImages();
            SetImageToPictureBox();
        }
    }
}
