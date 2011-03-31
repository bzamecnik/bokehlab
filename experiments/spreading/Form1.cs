using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Drawing.Imaging;
using libpfm;

// TODO:
// - add controls for selecting the blur source (depth map, procedure, constant, ...)

namespace spreading
{
    public partial class Form1 : Form
    {
        protected Bitmap inputLdrImage = null;
        protected Bitmap outputLdrImage = null;

        protected PFMImage inputHdrImage = null;
        protected PFMImage outputHdrImage = null;

        protected PFMImage depthMap = null;

        protected ThinLensDepthMapBlur thinLensBlur = null;

        protected bool ToneMappingEnabled { get; set; }

        private RectangleSpreadingFilter RectangleFilter;
        private PerimeterSpreadingFilter PerimeterFilter;

        public Form1()
        {
            InitializeComponent();
            blurRadiusNumeric.Value = ProceduralBlur.DEFAULT_BLUR_RADIUS;
            imageTypeComboBox.SelectedIndex = 0;
            filterTypeComboBox.SelectedIndex = 0;
            thinLensBlur = new ThinLensDepthMapBlur(50, 20, 100, 1000, 260);
            apertureNumeric.Value = (decimal)thinLensBlur.Aperture;
            focusPlaneNumeric.Value = (decimal)thinLensBlur.FocusPlane;
            toneMappingCheckBox.Checked = false;

            RectangleFilter = new RectangleSpreadingFilter();
            PerimeterFilter = new PerimeterSpreadingFilter();
        }

        private void GeneratePerimeterPSFs(PerimeterSpreadingFilter filter, int maxRadius)
        {
            //PSF.Perimeter.IPSFGenerator psfGen = new PSF.Perimeter.LinearPSFGenerator();
            PSF.Perimeter.IPSFGenerator psfGen = new PSF.Perimeter.CirclePSFGenerator();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            filter.Psf = psfGen.GeneratePSF(maxRadius);
            sw.Stop();
            Console.WriteLine("Generated PSF with max. radius {0} in {1:f}s", maxRadius, 1.0e-3 * sw.ElapsedMilliseconds);
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Title = "Open Image File";
            ofd.Filter = "PNG Files|*.png" +
                "|PFM Files|*.pfm" +
                "|Bitmap Files|*.bmp" +
                "|Gif Files|*.gif" +
                "|JPEG Files|*.jpg" +
                "|TIFF Files|*.tif" +
                "|All Image types|*.png;*.pfm;*.bmp;*.gif;*.jpg;*.tif";

            ofd.FilterIndex = 7;
            ofd.FileName = "";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            if (ofd.FileName.EndsWith(".pfm"))
            {
                if (inputHdrImage != null)
                {
                    inputHdrImage.Dispose();
                }
                inputHdrImage = PFMImage.LoadImage(ofd.FileName);
                ReplaceLdrImage(ref inputLdrImage, inputHdrImage.ToLdr(ToneMappingEnabled));
            }
            else
            {
                ReplaceLdrImage(ref inputLdrImage, (Bitmap)Image.FromFile(ofd.FileName));
                if (inputHdrImage != null)
                {
                    inputHdrImage.Dispose();
                }
                inputHdrImage = PFMImage.FromLdr(inputLdrImage);
            }
            imageTypeComboBox.SelectedIndex = 0; // TODO: select original better
            updatePictureBoxImage();

            if (outputHdrImage != null)
            {
                outputHdrImage.Dispose();
            }
            outputHdrImage = null;
            ReplaceLdrImage(ref outputLdrImage, null);

            imageSizeLabel.Text = String.Format("{0}x{1}", inputHdrImage.Width, inputHdrImage.Height);
        }

        private void ReplaceLdrImage(ref Bitmap existingImage, Bitmap newImage)
        {
            if (existingImage != null)
            {
                existingImage.Dispose();
            }
            existingImage = newImage;
        }

        private void loadDepthMapButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Title = "Open depth map";
            ofd.Filter = "PNG Files|*.png" +
                "|PFM Files|*.pfm" +
                "|Bitmap Files|*.bmp" +
                "|Gif Files|*.gif" +
                "|JPEG Files|*.jpg" +
                "|TIFF Files|*.tif" +
                "|All Image types|*.png;*.pfm;*.bmp;*.gif;*.jpg;*.tif";

            ofd.FilterIndex = 7;
            ofd.FileName = "";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            if (depthMap != null)
            {
                depthMap.Dispose();
            }
            if (ofd.FileName.EndsWith(".pfm"))
            {
                depthMap = PFMImage.LoadImage(ofd.FileName);
            }
            else
            {
                Bitmap depthMapLdr = (Bitmap)Image.FromFile(ofd.FileName);
                depthMap = PFMImage.FromLdr(depthMapLdr);
                depthMapLdr.Dispose();
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            PFMImage hdrImageToSave = outputHdrImage;
            Bitmap ldrImageToSave = outputLdrImage;
            if ((outputLdrImage == null) || (outputHdrImage == null))
            {
                hdrImageToSave = inputHdrImage;
                ldrImageToSave = inputLdrImage;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save output file";
            sfd.Filter = "JPEG Files|*.jpg|PNG Files|*.png|PFM Files|*.pfm";
            sfd.AddExtension = true;
            sfd.FileName = "";
            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            if (sfd.FileName.EndsWith(".pfm"))
            {
                hdrImageToSave.SaveImage(sfd.FileName);
            }
            else if (sfd.FileName.EndsWith(".png"))
            {
                ldrImageToSave.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
            else if (sfd.FileName.EndsWith(".jpg"))
            {
                ldrImageToSave.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }

        private void buttonFilter_Click(object sender, EventArgs e)
        {
            filterImage();
        }

        private void blurRadiusNumeric_ValueChanged(object sender, EventArgs e)
        {
            filterImage();
        }

        private void filterImage()
        {
            if ((inputLdrImage == null) || (inputLdrImage == null)) return;
            Cursor.Current = Cursors.WaitCursor;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                uint width = inputHdrImage.Width;
                uint height = inputHdrImage.Height;
                if ((depthMap != null) &&
                    ((depthMap.Width != width) ||
                    (depthMap.Height != height)))
                {
                    throw new ArgumentException(String.Format(
                        "Depth map must have the same dimensions as the input image"
                        + " {0}x{1}, but it's size was {2}x{3}.", width, height, depthMap.Width, depthMap.Height));
                }
                AbstractSpreadingFilter filter = GetSpreadingFilter();
                filter.Blur = CreateBlurFunction(depthMap);
                outputHdrImage = filter.FilterImage(inputHdrImage, outputHdrImage);
                ReplaceLdrImage(ref outputLdrImage, outputHdrImage.ToLdr(ToneMappingEnabled));
                imageTypeComboBox.SelectedIndex = 1; // TODO: select the filtered image better
                updatePictureBoxImage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error");
            }

            sw.Stop();
            labelElapsed.Text = String.Format("Elapsed time: {0:f}s", 1.0e-3 * sw.ElapsedMilliseconds);

            Cursor.Current = Cursors.Default;
        }

        private AbstractSpreadingFilter GetSpreadingFilter()
        {
            string filterName = filterTypeComboBox.SelectedItem.ToString();
            if (filterName == "rectangle") {
                return RectangleFilter;
            }
            else if (filterName == "perimeter")
            {
                PerimeterFilter.ForceMaxRadius = (int)blurRadiusNumeric.Value;
                if (PerimeterFilter.Psf == null)
                {
                    int maxGeneratedPsfRadius = 100;
                    Console.WriteLine("Generating perimeter PSFs up to radius {0}.", maxGeneratedPsfRadius);
                    GeneratePerimeterPSFs(PerimeterFilter, maxGeneratedPsfRadius);
                }
                return PerimeterFilter;
            }
            else
            {
                throw new ArgumentException("Unknown filter name: {0}", filterName);
            }
        }

        private void imageTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            updatePictureBoxImage();
        }

        private void updatePictureBoxImage()
        {
            // TODO: use enum
            switch (imageTypeComboBox.SelectedItem.ToString())
            {
                case "Original":
                    pictureBox1.Image = inputLdrImage;
                    break;
                case "Filtered":
                    pictureBox1.Image = outputLdrImage;
                    break;
                case "Depth map":
                    pictureBox1.Image = (depthMap != null) ? depthMap.ToLdr(ToneMappingEnabled) : null;
                    break;
            }
        }

        private void imageSizeOrigButton_Click(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox1.Dock = DockStyle.None;
        }

        private void imageSizeStretchButton_Click(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Dock = DockStyle.Fill;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (inputLdrImage != null)
            {
                inputLdrImage.Dispose();
                inputLdrImage = null;
            }
            if (outputLdrImage != null)
            {
                outputLdrImage.Dispose();
                outputLdrImage = null;
            }
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
            }
        }

        private void clearDepthmapButton_Click(object sender, EventArgs e)
        {
            if (depthMap != null)
            {
                depthMap.Dispose();
                depthMap = null;
            }
            if (imageTypeComboBox.SelectedItem.ToString() == "Depth map")
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
        }

        private BlurMap CreateBlurFunction(PFMImage depthMap)
        {
            BlurMap blur;
            if (depthMap != null)
            {
                thinLensBlur.DepthMap = depthMap;
                blur = thinLensBlur;
            }
            else
            {
                int maxBlurRadius = (int)blurRadiusNumeric.Value;
                blur = new ConstantBlur(maxBlurRadius - 1);
            }
            return blur;
        }

        private void focusPlaneNumeric_ValueChanged(object sender, EventArgs e)
        {
            thinLensBlur.FocusPlane = (float)focusPlaneNumeric.Value;
            filterImage();
        }

        private void apertureNumeric_ValueChanged(object sender, EventArgs e)
        {
            thinLensBlur.Aperture = (float)apertureNumeric.Value;
            filterImage();
        }

        private void toneMappingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ToneMappingEnabled = toneMappingCheckBox.Checked;
            if (outputHdrImage != null)
            {
                ReplaceLdrImage(ref outputLdrImage, outputHdrImage.ToLdr(ToneMappingEnabled));
            }
            if (inputHdrImage != null)
            {
                ReplaceLdrImage(ref inputLdrImage, inputHdrImage.ToLdr(ToneMappingEnabled));
            }
            updatePictureBoxImage();
        }
    }
}
