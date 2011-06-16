using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DDA
{
    public partial class Form1 : Form
    {
        Bitmap image;

        Random rnd = new Random();

        public Form1()
        {
            InitializeComponent();
            image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Draw();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Draw();
        }

        private void Draw()
        {
            int width = pictureBox1.Width;
            int height = pictureBox1.Height;

            pictureBox1.Image = null;

            using (Graphics g = Graphics.FromImage(image))
            {
                g.Clear(Color.White);
            }
            for (int i = 0; i < 100; i++)
            {
                Point lineStart = GetRandomPoint(width, height);
                Point lineEnd = GetRandomPoint(width, height);

                DrawLineDDA(lineStart, lineEnd, Color.Black);
            }

            pictureBox1.Image = image;
        }

        private void DrawLineDDA(Point start, Point end, Color color)
        {
            int dx = end.X - start.X;
            int dy = end.Y - start.Y;

            int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));
            float stepsInv = 1.0f / (float)steps;

            float xIncrement = dx * stepsInv;
            float yIncrement = dy * stepsInv;

            image.SetPixel(start.X, start.Y, color);

            float x = start.X;
            float y = start.Y;

            for (int i = 0; i < steps; i++)
            {
                x += xIncrement;
                y += yIncrement;
                image.SetPixel((int)Math.Round(x), (int)Math.Round(y), color);
            }
        }

        private Point GetRandomPoint(int width, int height)
        {
            return new Point(rnd.Next(width - 1), rnd.Next(height - 1));
        }
    }
}
