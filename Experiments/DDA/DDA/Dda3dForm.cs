using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;

namespace DDA
{
    public partial class Dda3dForm : Form
    {
        Bitmap[] image = new Bitmap[3];
        Random rnd = new Random();

        int width;
        int height;
        int depth;

        public Dda3dForm()
        {
            InitializeComponent();

            width = 300;
            height = 200;
            depth = 100;

            xPictureBox.Size = new Size(width, depth);
            yPictureBox.Size = new Size(height, depth);
            zPictureBox.Size = new Size(width, height);

            Draw();
        }

        private void Draw()
        {
            xPictureBox.Image = null;
            yPictureBox.Image = null;
            zPictureBox.Image = null;

            image[0] = PrepareClearImage(width, depth, image[0]);
            image[1] = PrepareClearImage(height, depth, image[1]);
            image[2] = PrepareClearImage(width, height, image[2]);

            //for (int i = 0; i < 2; i++)
            //{
            //    Vector3 lineStart = GetRandomPoint(width, height, depth);
            //    Vector3 lineEnd = GetRandomPoint(width, height, depth);

            //    DrawLineDDA(lineStart, lineEnd, Color.FromArgb(
            //        rnd.Next(255), rnd.Next(255), rnd.Next(255)));
            //}

            //DrawLineDDA(new Vector3(0, 0, 0), new Vector3(width - 1, height - 1, depth - 1), Color.Black);
            DrawLineDDA(new Vector3(0, 0, 0), new Vector3(width - 1, 0, depth - 1), Color.Black);

            xPictureBox.Image = image[0];
            yPictureBox.Image = image[1];
            zPictureBox.Image = image[2];
        }

        private Bitmap PrepareClearImage(int width, int height, Bitmap image)
        {
            if ((image == null) || (image.Width != width) || (image.Height != height))
            {
                image = new Bitmap(width, height);
            }

            using (Graphics g = Graphics.FromImage(image))
            {
                g.Clear(Color.White);
            }
            return image;
        }

        private void DrawLineDDA(Vector3 start, Vector3 end, Color color)
        {
            Vector3 d = end - start;

            float maxDiffComp = Math.Max(Math.Abs(d.X), Math.Abs(d.Y));
            maxDiffComp = Math.Max(maxDiffComp, Math.Abs(d.Z));
            int steps = (int)maxDiffComp;

            float stepsInv = 1.0f / (float)steps;

            Vector3 increment = stepsInv * d;

            image[0].SetPixel((int)start.X, (int)start.Z, color);
            image[1].SetPixel((int)start.Y, (int)start.Z, color);
            image[2].SetPixel((int)start.X, (int)start.Y, color);

            Vector3 pos = start;

            for (int i = 0; i < steps; i++)
            {
                pos += increment;
                image[0].SetPixel((int)Math.Round(pos.X), (int)Math.Round(pos.Z), color);
                image[1].SetPixel((int)Math.Round(pos.Y), (int)Math.Round(pos.Z), color);
                image[2].SetPixel((int)Math.Round(pos.X), (int)Math.Round(pos.Y), color);
            }
        }

        private Vector3 GetRandomPoint(int width, int height, int depth)
        {
            return new Vector3(
                rnd.Next(width - 1),
                rnd.Next(height - 1),
                rnd.Next(depth - 1));
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            Draw();
        }
    }
}
