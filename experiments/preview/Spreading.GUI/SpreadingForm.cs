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
using BokehLab.FloatMap;

// TODO:
// - add controls for selecting the blur source (depth map, procedure, constant, ...)

namespace BokehLab.Spreading.GUI
{
    public partial class SpreadingForm : Form
    {
        private static readonly int MAX_GENERATED_PSF_RADIUS = 50;

        protected Bitmap inputLdrImage = null;
        protected Bitmap outputLdrImage = null;

        protected FloatMapImage inputHdrImage = null;
        protected FloatMapImage outputHdrImage = null;

        protected FloatMapImage depthMap = null;

        protected ThinLensDepthMapBlur thinLensBlur = null;

        protected bool ToneMappingEnabled { get; set; }

        private RectangleSpreadingFilter RectangleFilter;
        private PerimeterSpreadingFilter PerimeterFilter;
        private HybridSpreadingFilter HybridFilter;

        public SpreadingForm()
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
            HybridFilter = new HybridSpreadingFilter(RectangleFilter, PerimeterFilter);
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
                inputHdrImage = PortableFloatMap.LoadImage(ofd.FileName);
                ReplaceLdrImage(ref inputLdrImage, inputHdrImage.ToBitmap(ToneMappingEnabled));
            }
            else
            {
                ReplaceLdrImage(ref inputLdrImage, (Bitmap)Image.FromFile(ofd.FileName));
                if (inputHdrImage != null)
                {
                    inputHdrImage.Dispose();
                }
                inputHdrImage = inputLdrImage.ToFloatMap();
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
                depthMap = PortableFloatMap.LoadImage(ofd.FileName);
            }
            else
            {
                Bitmap depthMapLdr = (Bitmap)Image.FromFile(ofd.FileName);
                depthMap = depthMapLdr.ToFloatMap();
                depthMapLdr.Dispose();
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            FloatMapImage hdrImageToSave = outputHdrImage;
            Bitmap ldrImageToSave = outputLdrImage;
            if ((outputLdrImage == null) || (outputHdrImage == null))
            {
                hdrImageToSave = inputHdrImage;
                ldrImageToSave = inputLdrImage;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save output file";
            sfd.Filter = "PNG Files|*.png|PFM Files|*.pfm|JPEG Files|*.jpg";
            sfd.AddExtension = true;
            sfd.FileName = "";
            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            if (sfd.FileName.EndsWith(".pfm"))
            {
                PortableFloatMap.SaveImage(hdrImageToSave, sfd.FileName);
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
                //filter.SpreadOneRoundedPSF = true;
                outputHdrImage = filter.FilterImage(inputHdrImage, outputHdrImage);
                ReplaceLdrImage(ref outputLdrImage, outputHdrImage.ToBitmap(ToneMappingEnabled));
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
                PreparePerimeterFilter();
                return PerimeterFilter;
            }
            else if (filterName == "hybrid")
            {
                PreparePerimeterFilter();
                HybridFilter.MaxRadiusForQualityFilter = PerimeterFilter.Psf.MaxRadius;
                return HybridFilter;
            }
            else
            {
                throw new ArgumentException("Unknown filter name: {0}", filterName);
            }
        }

        private void PreparePerimeterFilter()
        {
            PerimeterFilter.ForceMaxRadius = (int)blurRadiusNumeric.Value;
            if (PerimeterFilter.Psf == null)
            {
                int maxGeneratedPsfRadius = MAX_GENERATED_PSF_RADIUS;
                Console.WriteLine("Generating perimeter PSFs up to radius {0}.", maxGeneratedPsfRadius);
                GeneratePerimeterPSFs(PerimeterFilter, maxGeneratedPsfRadius);
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
                    pictureBox1.Image = (depthMap != null) ? depthMap.ToBitmap(ToneMappingEnabled) : null;
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

        private BlurMap CreateBlurFunction(FloatMapImage depthMap)
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
                ReplaceLdrImage(ref outputLdrImage, outputHdrImage.ToBitmap(ToneMappingEnabled));
            }
            if (inputHdrImage != null)
            {
                ReplaceLdrImage(ref inputLdrImage, inputHdrImage.ToBitmap(ToneMappingEnabled));
            }
            updatePictureBoxImage();
        }
    }
}
