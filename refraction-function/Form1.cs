using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace refraction_function
{
    public partial class Form1 : Form
    {
        public SphericLens.OpticalBench Bench;

        public Form1()
        {
            Bench = new SphericLens.OpticalBench();
            Bench.Direction = new SphericLens.Vector(0.0, 100.0);
            InitializeComponent();
            //this.KeyDown += new KeyEventHandler(pictureResult.KeyPressed);
            innerMediumNumericUpDown.Value = new Decimal(Bench.RefractiveIndexGlass);
            outerMediumNumericUpDown.Value = new Decimal(Bench.RefractiveIndexAir);
            ComputeRefractionFunction();
        }

        private void ComputeRefractionFunction()
        {
            Rectangle imageSize = pictureBox2.ClientRectangle;
            Bitmap bitmap = new Bitmap(imageSize.Width, imageSize.Height);
            Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.TranslateTransform(0.0f, imageSize.Height - 1);
            g.ScaleTransform(1.0f, -1.0f);
            double maxPhi = 2.0 * Math.PI;
            int steps = imageSize.Height;
            float lastValue = 0.0f;
            for (int x = 0; x < imageSize.Width; x++)
            {
                double phi = maxPhi * x / (double)(steps + 1);
                float value = (float)(RefractionFunction(phi) / maxPhi);
                float currentValue = imageSize.Height * value;
                int intensity = (int)(255 * value);
                Pen colorPen = new Pen(Color.FromArgb(intensity, intensity, intensity), 1.0f);
                g.DrawLine(colorPen, x, 0.0f, x, currentValue);
                g.DrawLine(Pens.Black, x - 1, lastValue, x, currentValue);
                lastValue = currentValue;
            }
            g.DrawLine(Pens.Black, imageSize.Width - 1, lastValue, imageSize.Width - 1, 0.0f);
            pictureBox2.Image = bitmap;
        }

        private double RefractionFunction(double phi)
        {
            Bench.Direction.Phi = phi;
            Bench.Update();
            return Bench.RefractedDirection.Phi;
        }

        private void innerMediumNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Bench.RefractiveIndexGlass = (double)innerMediumNumericUpDown.Value;
            ComputeRefractionFunction();
        }

        private void outerMediumNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Bench.RefractiveIndexAir = (double)outerMediumNumericUpDown.Value;
            ComputeRefractionFunction();
        }

    }
}
