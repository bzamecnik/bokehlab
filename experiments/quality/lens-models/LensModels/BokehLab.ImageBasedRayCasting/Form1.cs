using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BokehLab.FloatMap;
using OpenTK;

namespace BokehLab.ImageBasedRayCasting
{
    public partial class Form1 : Form
    {
        RayTracer rayTracer = new RayTracer();

        public Form1()
        {
            InitializeComponent();
        }

        private void RenderImage()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            FloatMapImage outputImage = rayTracer.RenderImage(pictureBox1.Size);

            stopwatch.Stop();

            pictureBox1.Image = outputImage.ToBitmap();
            pictureBox1.Invalidate();

            elapsedTimeToolStripStatusLabel.Text = string.Format("{0} ms",
                stopwatch.ElapsedMilliseconds);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            rayTracer.Camera.Lens.FocalLength = 10;
            Bitmap sceneImage = (Bitmap)Bitmap.FromFile("TestData/testImage.jpg");
            rayTracer.Scene.Layer.Image = sceneImage.ToFloatMap();

            rayTracer.Scene.Layer.Normal = new Vector3d(1, 1, 1);

            sampleCountNumeric.Value = (decimal)rayTracer.SampleCount;

            rayTracer.Camera.Sensor.Width = 10;
            rayTracer.Camera.Sensor.Shift = new Vector3d(0, 0, 20);
            senzorShiftZNumeric.Value = (decimal)rayTracer.Camera.Sensor.Shift.Z;

            rayTracer.Scene.Layer.Origin = new Vector3d(0, 0, -20);
            layerZNumeric.Value = (decimal)rayTracer.Scene.Layer.Origin.Z;

            RenderImage();
        }

        private void renderButton_Click(object sender, EventArgs e)
        {
            Vector3d senzorShift = rayTracer.Camera.Sensor.Shift;
            senzorShift.Z = (double)senzorShiftZNumeric.Value;
            rayTracer.Camera.Sensor.Shift = senzorShift;

            Vector3d sceneOrigin = rayTracer.Scene.Layer.Origin;
            sceneOrigin.Z = (double)layerZNumeric.Value;
            rayTracer.Scene.Layer.Origin = sceneOrigin;

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
            Vector3d sceneOrigin = rayTracer.Scene.Layer.Origin;
            sceneOrigin.Z = (double)layerZNumeric.Value;
            rayTracer.Scene.Layer.Origin = sceneOrigin;

            RenderImage();
        }

        private void sampleCountNumeric_ValueChanged(object sender, EventArgs e)
        {
            rayTracer.SampleCount = (int)sampleCountNumeric.Value;

            RenderImage();
        }
    }
}
