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
            senzorTiltXNumeric.Value = (decimal)lightTracer.Sensor.Tilt.X;
            senzorTiltYNumeric.Value = (decimal)lightTracer.Sensor.Tilt.Y;
            senzorTiltZNumeric.Value = (decimal)lightTracer.Sensor.Tilt.Z;
            senzorShiftXNumeric.Value = (decimal)lightTracer.Sensor.Shift.X;
            senzorShiftYNumeric.Value = (decimal)lightTracer.Sensor.Shift.Y;
            senzorShiftZNumeric.Value = (decimal)lightTracer.Sensor.Shift.Z;
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
            lightTracer.LightSourcePosition =
                GetVectorFromControls(lightXNumeric, lightYNumeric, lightZNumeric);
            Recompute();
        }

        private void lightYNumeric_ValueChanged(object sender, EventArgs e)
        {
            lightTracer.LightSourcePosition =
                GetVectorFromControls(lightXNumeric, lightYNumeric, lightZNumeric);
            Recompute();
        }

        private void lightZNumeric_ValueChanged(object sender, EventArgs e)
        {
            lightTracer.LightSourcePosition =
                GetVectorFromControls(lightXNumeric, lightYNumeric, lightZNumeric);
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

        private void senzorTiltXNumeric_ValueChanged(object sender, EventArgs e)
        {
            lightTracer.Sensor.Tilt =
                GetVectorFromControls(senzorTiltXNumeric, senzorTiltYNumeric, senzorTiltZNumeric);
            Recompute();
        }

        private void senzorTiltYNumeric_ValueChanged(object sender, EventArgs e)
        {
            lightTracer.Sensor.Tilt =
                GetVectorFromControls(senzorTiltXNumeric, senzorTiltYNumeric, senzorTiltZNumeric);
            Recompute();
        }

        private void senzorTiltZNumeric_ValueChanged(object sender, EventArgs e)
        {
            lightTracer.Sensor.Tilt =
                GetVectorFromControls(senzorTiltXNumeric, senzorTiltYNumeric, senzorTiltZNumeric);
            Recompute();
        }

        private void senzorShiftXNumeric_ValueChanged(object sender, EventArgs e)
        {
            lightTracer.Sensor.Shift =
                GetVectorFromControls(senzorShiftXNumeric, senzorShiftYNumeric, senzorShiftZNumeric);
            Recompute();
        }

        private void senzorShiftYNumeric_ValueChanged(object sender, EventArgs e)
        {
            lightTracer.Sensor.Shift =
                GetVectorFromControls(senzorShiftXNumeric, senzorShiftYNumeric, senzorShiftZNumeric);
            Recompute();
        }

        private void senzorShiftZNumeric_ValueChanged(object sender, EventArgs e)
        {
            lightTracer.Sensor.Shift =
                GetVectorFromControls(senzorShiftXNumeric, senzorShiftYNumeric, senzorShiftZNumeric);
            Recompute();
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
    }
}
