using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;

namespace LightTracing
{
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
            senzorCenterZNumeric.Value = (decimal)lightTracer.SenzorCenter.Z;
            lensFocalLengthNumeric.Value = (decimal)lightTracer.Lens.FocalLength;
            lensApertureNumeric.Value = (decimal)lightTracer.Lens.ApertureRadius;
            lightIntensityNumeric.Value = (decimal)lightTracer.LightIntensity;
            sampleCountNumeric.Value = (decimal)lightTracer.SampleCount;
        }

        private void Recompute()
        {
            pictureBox1.Image = lightTracer.TraceLight();
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
            lightTracer.SenzorCenter = new Vector3d(
                lightTracer.SenzorCenter.X,
                lightTracer.SenzorCenter.Y,
                (double)senzorCenterZNumeric.Value);
            Recompute();
        }

        private void lensFocalLengthNumeric_ValueChanged(object sender, EventArgs e)
        {
            lightTracer.Lens.FocalLength = (double)lensFocalLengthNumeric.Value;
            Recompute();
        }

        private void lensApertureNumeric_ValueChanged(object sender, EventArgs e)
        {
            lightTracer.Lens.ApertureRadius = (double)lensApertureNumeric.Value;
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
