namespace BokehLab.ImageBasedRayCasting
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;
    using BokehLab.FloatMap;
    using BokehLab.RayTracing;
    using BokehLab.RayTracing.Lens;
    using OpenTK;

    public partial class IbrtForm : Form
    {
        RayTracer rayTracer = new RayTracer();
        ThinLens thinLens = new ThinLens();
        BiconvexLens biconvexLens = new BiconvexLens()
        {
            ApertureRadius = 1.8,
            CurvatureRadius = 10
        };

        ComplexLens complexLens;

        FloatMapImage layerImage;
        FloatMapImage outputImage;

        bool initialized = false;

        public IbrtForm()
        {
            InitializeComponent();

            //complexLens = ComplexLens.CreateBiconvexLens(10, 1, 0);
            //complexLens = ComplexLens.CreateDoubleGaussLens(Materials.Fixed.AIR, 2.0);
            complexLens = ComplexLens.CreatePetzvalLens(Materials.Fixed.AIR, 2.0);

            thinLens.FocalLength = 10;

            //Size outputImageSize = pictureBox1.Size;
            Size outputImageSize = new Size(450, 300);

            //Size outputImageSize = new Size(10, 10);
            outputSizeXNumeric.Value = outputImageSize.Width;
            outputSizeYNumeric.Value = outputImageSize.Height;
            specificOutputSizeCheckBox.Checked = true;

            //rayTracer.Camera.Lens = thinLens;
            //rayTracer.Camera.Lens = new PinholeLens();
            //rayTracer.Camera.Lens = new LensWithTwoStops() { Lens = thinLens };
            //rayTracer.Camera.Lens = biconvexLens;
            //rayTracer.Scene.Layer.Depth = -biconvexLens.FocalLength;
            rayTracer.Camera.Lens = complexLens;

            //rayTracer.Camera.Sensor.Tilt = new Vector3d(0, -0.25, 0);

            OpenLayerImage("TestData/testImage.jpg");

            //rayTracer.Scene.Layer.Plane.Normal = new Vector3d(1, 1, 1);

            sampleCountNumeric.Value = (decimal)rayTracer.SampleCount;

            //double senzorZ = biconvexLens.FocalLength;
            double senzorZ = 317.50;
            rayTracer.Camera.Sensor.Shift = new Vector3d(0, 0, senzorZ);
            senzorShiftZNumeric.Value = (decimal)rayTracer.Camera.Sensor.Shift.Z;
            rayTracer.Camera.Sensor.Width = 120;
            senzorWidthNumeric.Value = (decimal)rayTracer.Camera.Sensor.Width;

            rayTracer.Scene.Layer.Depth = -1000;
            //rayTracer.Scene.Layer.Depth = -2.18;
            layerZNumeric.Value = (decimal)rayTracer.Scene.Layer.Depth;

            lensFocalLengthNumeric.Value = (decimal)thinLens.FocalLength;
            lensApertureNumeric.Value = (decimal)thinLens.ApertureRadius;

            initialized = true;
        }

        private void RenderPreview()
        {
            RenderImage(true);
        }

        private void RenderImage(bool preview)
        {
            if (!initialized)
            {
                return;
            }

            rayTracer.SampleCount = (int)sampleCountNumeric.Value;
            rayTracer.Camera.Sensor.Width = (double)senzorWidthNumeric.Value;

            Cursor = Cursors.WaitCursor;
            Stopwatch stopwatch = Stopwatch.StartNew();

            Size outputImageSize = (specificOutputSizeCheckBox.Checked)
                ? new Size((int)outputSizeXNumeric.Value,
                    (int)outputSizeYNumeric.Value)
                : pictureBox1.Size;

            if (preview)
            {
                Size previewSize = new Size(outputImageSize.Width / 4, outputImageSize.Height / 4);
                int sampleCount = rayTracer.SampleCount;
                outputImage = rayTracer.RenderImagePreview(outputImageSize, new Rectangle(
                    new Point(
                        (outputImageSize.Width - previewSize.Width) / 2,
                        (outputImageSize.Height - previewSize.Height) / 2),
                    previewSize),
                    null);

                rayTracer.SampleCount = 64;
                Size focusWindowSize = new Size(40, 40);
                outputImage = rayTracer.RenderImagePreview(outputImageSize, new Rectangle(
                    new Point(
                        (outputImageSize.Width - focusWindowSize.Width) / 2,
                        (outputImageSize.Height - focusWindowSize.Height) / 2),
                    focusWindowSize),
                    outputImage);
                rayTracer.SampleCount = sampleCount;
            }
            else
            {
                outputImage = rayTracer.RenderImage(outputImageSize);
            }

            stopwatch.Stop();

            ShowImage();

            elapsedTimeToolStripStatusLabel.Text = string.Format("{0} ms",
                stopwatch.ElapsedMilliseconds);

            Cursor = Cursors.Default;
        }

        private void ShowImage()
        {
            pictureBox1.Image = outputImage.ToBitmap(tonemapOutputCheckBox.Checked);
            pictureBox1.Invalidate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RenderImage(false);
        }

        private void renderButton_Click(object sender, EventArgs e)
        {
            Vector3d senzorShift = rayTracer.Camera.Sensor.Shift;
            senzorShift.Z = (double)senzorShiftZNumeric.Value;
            rayTracer.Camera.Sensor.Shift = senzorShift;

            rayTracer.Scene.Layer.Depth = (double)layerZNumeric.Value;
            RenderImage(false);
        }

        private void layerZNumeric_ValueChanged(object sender, EventArgs e)
        {
            rayTracer.Scene.Layer.Depth = (double)layerZNumeric.Value;
            RenderPreview();
        }

        private void sampleCountNumeric_ValueChanged(object sender, EventArgs e)
        {
            rayTracer.SampleCount = (int)sampleCountNumeric.Value;
            //RenderImage();
        }

        private void lensApertureNumeric_ValueChanged(object sender, EventArgs e)
        {
            double aperture = (double)lensApertureNumeric.Value;
            thinLens.ApertureRadius = aperture;
            biconvexLens.ApertureRadius = aperture;
            RenderPreview();
        }

        private void lensFocalLengthNumeric_ValueChanged(object sender, EventArgs e)
        {
            thinLens.FocalLength = (double)lensFocalLengthNumeric.Value;
            RenderPreview();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = openLayerFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                OpenLayerImage(openLayerFileDialog.FileName);
            }
        }

        private void OpenLayerImage(string fileName)
        {
            if (fileName.EndsWith(".pfm"))
            {
                layerImage = PortableFloatMap.LoadImage(fileName);
            }
            else
            {
                Bitmap layerBitmap = (Bitmap)Bitmap.FromFile(fileName);
                layerImage = layerBitmap.ToFloatMap();
            }
            rayTracer.Scene.Layer.Image = layerImage;
        }

        private void renderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenderImage(false);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (outputImage == null)
            {
                return;
            }
            DialogResult result = saveRenderedFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                SaveOutputImage(saveRenderedFileDialog.FileName);
            }
        }

        private void SaveOutputImage(string fileName)
        {
            if (outputImage == null)
            {
                return;
            }
            if (fileName.EndsWith(".pfm"))
            {
                outputImage.SaveImage(fileName);
            }
            else
            {
                Bitmap bitmap = outputImage.ToBitmap(tonemapOutputCheckBox.Checked);
                bitmap.Save(fileName);
            }
        }

        private void specificOutputSizeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool specificOutputSize = specificOutputSizeCheckBox.Checked;
            outputSizeXNumeric.Enabled = specificOutputSize;
            outputSizeYNumeric.Enabled = specificOutputSize;
        }

        private void tonemapOutputCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ShowImage();
        }

        private void senzorTiltXNumeric_ValueChanged(object sender, EventArgs e)
        {
            rayTracer.Camera.Sensor.Tilt =
                GetVectorFromControls(senzorTiltXNumeric, senzorTiltYNumeric, senzorTiltZNumeric);

            RenderPreview();
        }

        private void senzorTiltYNumeric_ValueChanged(object sender, EventArgs e)
        {
            rayTracer.Camera.Sensor.Tilt =
                GetVectorFromControls(senzorTiltXNumeric, senzorTiltYNumeric, senzorTiltZNumeric);

            RenderPreview();
        }


        private void senzorTiltZNumeric_ValueChanged(object sender, EventArgs e)
        {
            rayTracer.Camera.Sensor.Tilt =
                GetVectorFromControls(senzorTiltXNumeric, senzorTiltYNumeric, senzorTiltZNumeric);

            RenderPreview();
        }

        private void senzorShiftXNumeric_ValueChanged(object sender, EventArgs e)
        {
            rayTracer.Camera.Sensor.Shift =
                GetVectorFromControls(senzorShiftXNumeric, senzorShiftYNumeric, senzorShiftZNumeric);

            RenderPreview();
        }

        private void senzorShiftYNumeric_ValueChanged(object sender, EventArgs e)
        {
            rayTracer.Camera.Sensor.Shift =
                GetVectorFromControls(senzorShiftXNumeric, senzorShiftYNumeric, senzorShiftZNumeric);

            RenderPreview();
        }

        private void senzorShiftZNumeric_ValueChanged(object sender, EventArgs e)
        {
            rayTracer.Camera.Sensor.Shift =
                GetVectorFromControls(senzorShiftXNumeric, senzorShiftYNumeric, senzorShiftZNumeric);

            RenderPreview();
        }

        private Vector3d GetVectorFromControls(
            NumericUpDown xNumeric,
            NumericUpDown yNumeric,
            NumericUpDown zNumeric)
        {
            return new Vector3d(
                (double)xNumeric.Value,
                (double)yNumeric.Value,
                (double)zNumeric.Value);
        }

        private void senzorWidthNumeric_ValueChanged(object sender, EventArgs e)
        {
            rayTracer.Camera.Sensor.Width = (double)senzorWidthNumeric.Value;
        }

        private void renderPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenderPreview();
        }
    }
}
