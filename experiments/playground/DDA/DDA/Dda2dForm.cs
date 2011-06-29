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
    public partial class Dda2dForm : Form
    {
        Bitmap image;
        float scale = 2.0f;

        Random rnd = new Random();

        public Dda2dForm()
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
            pictureBox1.Image = null;

            int width = pictureBox1.Width;
            int height = pictureBox1.Height;

            if ((image == null) || (image.Width != width) || (image.Height != height))
            {
                image = new Bitmap(width, height);
            }

            using (Graphics g = Graphics.FromImage(image))
            {
                g.Clear(Color.Black);

                //Point lineStart = new Point(200, 100);
                //Point lineEnd = new Point(0, 0);
                //for (int i = 0; i < 100; i++)
                //{
                Point lineStart = GetRandomPoint(width, height);
                Point lineEnd = GetRandomPoint(width, height);

                DrawOrigScale(lineStart, lineEnd, g);
                //DrawVariableScale(lineStart, lineEnd, g);

                //}
            }

            pictureBox1.Image = image;
        }

        private void DrawOrigScale(Point lineStart, Point lineEnd, Graphics g)
        {
            DrawLineFullDDAOrigScale(lineStart, lineEnd, Color.Red);
            //DrawLineDDAOrigScale(lineStart, lineEnd, Color.White);
            g.DrawLine(Pens.Green, lineStart, lineEnd);
        }

        private void DrawLineDDAOrigScale(Point start, Point end, Color color)
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

        // DDA visiting all pixels covered by the line
        private void DrawLineFullDDAOrigScale(PointF start, PointF end, Color color)
        {
            // distances between ray intersections with neighbor pixel
            // in axes X and Y
            float dx = (end.X - start.X) / Math.Abs(end.Y - start.Y);
            float dy = (end.Y - start.Y) / Math.Abs(end.X - start.X);

            // distances between X and Y grid intersections along the ray
            float distY = (float)Math.Sqrt(1 + dx * dx);
            float distX = (float)Math.Sqrt(1 + dy * dy);

            // distance from current point to the nearest grid intersection
            float distToNextX = distX * (float)(Math.Ceiling(start.X) - start.X);
            distToNextX = (distToNextX > 0) ? distToNextX : distX;
            float distToNextY = distY * (float)(Math.Ceiling(start.Y) - start.Y);
            distToNextY = (distToNextY > 0) ? distToNextY : distY;

            // current grid cell (pixel) coordinates
            int x = (int)Math.Floor(start.X);
            int y = (int)Math.Floor(start.Y);

            image.SetPixel(x, y, color);

            int endX = (int)Math.Ceiling(end.X);
            int endY = (int)Math.Ceiling(end.Y);

            int stepX = Math.Sign(dx);
            int stepY = Math.Sign(dy);

            while ((x != endX) && (y != endY))
            {
                if (distToNextX < distToNextY)
                {
                    distToNextY -= distToNextX;
                    distToNextX = distX;
                    x += stepX;
                }
                else
                {
                    distToNextX -= distToNextY;
                    distToNextY = distY;
                    y += stepY;
                }
                if ((x < 0) || (x >= image.Width) ||
                    (y < 0) || (y >= image.Height))
                {
                    break;
                }
                image.SetPixel(x, y, color);
            }
        }

        private void DrawVariableScale(PointF lineStart, PointF lineEnd, Graphics g)
        {
            Vector2 start = new Vector2(lineStart.X, lineStart.Y);
            Vector2 end = new Vector2(lineEnd.X, lineEnd.Y);
            DrawLineDDAVariableScale(start, end, Brushes.Black, g, scale);
        }

        private void DrawLineDDAVariableScale(
            Vector2 start, Vector2 end,
            Brush brush, Graphics g, float scale)
        {
            Vector2 d = end - start;

            float steps = Math.Max(Math.Abs(d.X), Math.Abs(d.Y));
            float stepsInv = 1.0f / (float)steps;

            Vector2 increment = d * stepsInv;

            Vector2 pos = scale * new Vector2((float)Math.Round(start.X), (float)Math.Round(start.Y));

            g.FillRectangle(Brushes.Red, pos.X, pos.Y, 1, 1);
            g.FillRectangle(brush, pos.X, pos.Y, scale, scale);

            pos = start;

            for (int i = 0; i < steps; i++)
            {
                pos += increment;
                float x = (float)Math.Round(pos.X) * scale;
                float y = (float)Math.Round(pos.Y) * scale;
                g.FillRectangle(Brushes.Red, pos.X, pos.Y, 1, 1);
                g.FillRectangle(brush, x, y, scale, scale);
            }
        }

        private Point GetRandomPoint(int width, int height)
        {
            return new Point(rnd.Next(width - 1), rnd.Next(height - 1));
        }

        private Vector2 GetRandomVector2(float width, float height)
        {
            return new Vector2((float)rnd.NextDouble() * width, (float)rnd.NextDouble() * height);
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            Draw();
        }
    }
}
