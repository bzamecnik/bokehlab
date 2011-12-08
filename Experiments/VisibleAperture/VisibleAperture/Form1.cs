using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VisibleAperture
{
    public partial class Form1 : Form
    {
        Bitmap image;
        Random rnd = new Random();

        public Form1()
        {
            InitializeComponent();
            image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = image;
            Draw();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Draw();
        }

        private void Draw()
        {
            pictureBox1.Image = null;
            using (Graphics g = Graphics.FromImage(image))
            {
                g.Clear(Color.Black);
            }

            int width = image.Width;
            int height = image.Height;

            //for (int y = 0; y < height; y++)
            //{
            //    for (int x = 0; x < width; x++)
            //    {
            //        if (ApertureFunction(x, y) == 1)
            //        {
            //            image.SetPixel(x, y, Color.Black);
            //        }
            //    }
            //}

            for (int i = 0; i < 100; i++)
            {
                float x = (float)(width * rnd.NextDouble());
                float y = (float)(height * rnd.NextDouble());
                bool inside = (ApertureFunction(x, y) == 1);
                image.SetPixel((int)x, (int)y, inside ? Color.Red : Color.Green);
            }


            pictureBox1.Image = image;
        }

        private float ApertureFunction(float x, float y)
        {
            x -= 0.5f * image.Width;
            y -= 0.5f * image.Height;
            float radius = 20;
            bool inside = x * x + y * y < radius * radius;
            return inside ? 1 : 0;
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = image;
            Draw();
        }
    }
}
