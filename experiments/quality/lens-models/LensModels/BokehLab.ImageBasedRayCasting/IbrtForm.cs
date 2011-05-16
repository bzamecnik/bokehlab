﻿namespace BokehLab.ImageBasedRayCasting
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;
    using BokehLab.FloatMap;
    using BokehLab.Lens;
    using OpenTK;

    public partial class IbrtForm : Form
    {
        RayTracer rayTracer = new RayTracer();
        ThinLens thinLens = new ThinLens();

        FloatMapImage layerImage;
        FloatMapImage outputImage;

        public IbrtForm()
        {
            InitializeComponent();
            thinLens.FocalLength = 10;
        }

        private void RenderImage()
        {
            Cursor = Cursors.WaitCursor;
            Stopwatch stopwatch = Stopwatch.StartNew();

            Size outputImageSize = (specificOutputSizeCheckBox.Checked)
                ? new Size((int)outputSizeXNumeric.Value,
                    (int)outputSizeYNumeric.Value)
                : pictureBox1.Size;

            outputImage = rayTracer.RenderImage(outputImageSize);

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
            Size outputImageSize = pictureBox1.Size;
            outputSizeXNumeric.Value = outputImageSize.Width;
            outputSizeYNumeric.Value = outputImageSize.Height;

            rayTracer.Camera.Lens = thinLens;
            //rayTracer.Camera.Lens = new PinholeLens();

            rayTracer.Camera.Sensor.Tilt = new Vector3d(0, -0.25, 0);

            OpenLayerImage("TestData/testImage.jpg");

            //rayTracer.Scene.Layer.Plane.Normal = new Vector3d(1, 1, 1);

            sampleCountNumeric.Value = (decimal)rayTracer.SampleCount;

            rayTracer.Camera.Sensor.Width = 10;
            rayTracer.Camera.Sensor.Shift = new Vector3d(0, 0, 20);
            senzorShiftZNumeric.Value = (decimal)rayTracer.Camera.Sensor.Shift.Z;

            rayTracer.Scene.Layer.Plane.Origin = new Vector3d(0, 0, -20);
            layerZNumeric.Value = (decimal)rayTracer.Scene.Layer.Plane.Origin.Z;

            lensFocalLengthNumeric.Value = (decimal)thinLens.FocalLength;
            lensApertureNumeric.Value = (decimal)thinLens.ApertureRadius;

            RenderImage();
        }

        private void renderButton_Click(object sender, EventArgs e)
        {
            Vector3d senzorShift = rayTracer.Camera.Sensor.Shift;
            senzorShift.Z = (double)senzorShiftZNumeric.Value;
            rayTracer.Camera.Sensor.Shift = senzorShift;

            Vector3d sceneOrigin = rayTracer.Scene.Layer.Plane.Origin;
            sceneOrigin.Z = (double)layerZNumeric.Value;
            rayTracer.Scene.Layer.Plane.Origin = sceneOrigin;

            RenderImage();
        }

        private void senzorShiftZNumeric_ValueChanged(object sender, EventArgs e)
        {
            Vector3d senzorShift = rayTracer.Camera.Sensor.Shift;
            senzorShift.Z = (double)senzorShiftZNumeric.Value;
            rayTracer.Camera.Sensor.Shift = senzorShift;

            RenderImage();
        }

        private void layerZNumeric_ValueChanged(object sender, EventArgs e)
        {
            Vector3d sceneOrigin = rayTracer.Scene.Layer.Plane.Origin;
            sceneOrigin.Z = (double)layerZNumeric.Value;
            rayTracer.Scene.Layer.Plane.Origin = sceneOrigin;

            RenderImage();
        }

        private void sampleCountNumeric_ValueChanged(object sender, EventArgs e)
        {
            rayTracer.SampleCount = (int)sampleCountNumeric.Value;

            RenderImage();
        }


        private void lensApertureNumeric_ValueChanged(object sender, EventArgs e)
        {
            thinLens.ApertureRadius = (double)lensApertureNumeric.Value;
            RenderImage();
        }

        private void lensFocalLengthNumeric_ValueChanged(object sender, EventArgs e)
        {
            thinLens.FocalLength = (double)lensFocalLengthNumeric.Value;
            RenderImage();
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
            RenderImage();
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
    }
}
