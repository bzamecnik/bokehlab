namespace LightTracing
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;
    using OpenTK;
    using BokehLab.FloatMap;

    public partial class LightTracingForm : Form
    {
        private LightTracer lightTracer;

        public LightTracingForm()
        {
            InitializeComponent();
            lightTracer = new LightTracer();
            FillDefaultValues();
            Recompute();
        }

        private void FillDefaultValues()
        {
            lightXNumeric.Value = (decimal)lightTracer.LightSourcePosition.X;
            lightYNumeric.Value = (decimal)lightTracer.LightSourcePosition.Y;
            lightZNumeric.Value = (decimal)lightTracer.LightSourcePosition.Z;
            senzorCenterZNumeric.Value = (decimal)lightTracer.Sensor.Shift.Z;
            lensFocalLengthNumeric.Value = (decimal)lightTracer.ThinLens.FocalLength;
            lensApertureNumeric.Value = (decimal)lightTracer.ThinLens.ApertureRadius;
            lightIntensityNumeric.Value = (decimal)lightTracer.LightIntensity;
            sampleCountNumeric.Value = (decimal)lightTracer.SampleCount;
        }

        private void Recompute()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            pictureBox1.Image = lightTracer.RenderImage(pictureBox1.Size).ToBitmap();
            stopwatch.Stop();
            elapsedTimeStatusLabel.Text = string.Format("{0} ms", stopwatch.ElapsedMilliseconds);
        }

        private void lightXNumeric_ValueChanged(object sender, EventArgs e)
        {
            lightTracer.LightSourcePosition = new Vector3d(
                (double)lightXNumeric.Value,
                lightTracer.LightSourcePosition.Y,
                lightTracer.LightSourcePosition.Z);
            Recompute();
        }

        private void lightYNumeric_ValueChanged(object sender, EventArgs e)
        {
            lightTracer.LightSourcePosition = new Vector3d(
                lightTracer.LightSourcePosition.X,
                (double)lightYNumeric.Value,
                lightTracer.LightSourcePosition.Z);
            Recompute();
        }

        private void lightZNumeric_ValueChanged(object sender, EventArgs e)
        {
            lightTracer.LightSourcePosition = new Vector3d(
                lightTracer.LightSourcePosition.X,
                lightTracer.LightSourcePosition.Y,
                (double)lightZNumeric.Value);
            Recompute();
        }

        private void senzorCenterZNumeric_ValueChanged(object sender, EventArgs e)
        {
            lightTracer.Sensor.Shift = new Vector3d(
                lightTracer.Sensor.Shift.X,
                lightTracer.Sensor.Shift.Y,
                (double)senzorCenterZNumeric.Value);
            Recompute();
        }

        private void lensFocalLengthNumeric_ValueChanged(object sender, EventArgs e)
        {
            lightTracer.ThinLens.FocalLength = (double)lensFocalLengthNumeric.Value;
            Recompute();
        }

        private void lensApertureNumeric_ValueChanged(object sender, EventArgs e)
        {
            lightTracer.ThinLens.ApertureRadius = (double)lensApertureNumeric.Value;
            Recompute();
        }

        private void lightIntensityNumeric_ValueChanged(object sender, EventArgs e)
        {
            lightTracer.LightIntensity = (float)lightIntensityNumeric.Value;
            Recompute();
        }

        private void sampleCountNumeric_ValueChanged(object sender, EventArgs e)
        {
            lightTracer.SampleCount = (int)sampleCountNumeric.Value;
            Recompute();
        }
    }
}
