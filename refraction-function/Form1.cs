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
            Rectangle imageSize = pictureBox1.ClientRectangle;
            Bitmap bitmap = new Bitmap(imageSize.Width, imageSize.Height);
            double maxPhi = 2 * Math.PI;
            int steps = imageSize.Height;
            for (int y = 0; y < imageSize.Height; y++)
            {
                double phi = maxPhi * y / (double) (steps + 1);
                int intensity = (int) (255 * RefractionFunction(phi) / maxPhi);
                Color color = Color.FromArgb(intensity, intensity, intensity);
                for (int x = 0; x < imageSize.Width; x++)
                {
                    bitmap.SetPixel(x, y, color);
                }
            }
            pictureBox1.Image = bitmap;
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
