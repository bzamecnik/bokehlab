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
        float scale = 32.0f;

        private static readonly Vector2 HalfPixel = new Vector2(0.5f, 0.5f);
        private static readonly float Epsilon = 10e-6f;

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

                DrawGrid(g);

                float scaledWidth = (int)(width / scale);
                float scaledHeight = (int)(height / scale);
                //Point lineStart = GetRandomPoint((int)scaledWidth, (int)scaledHeight);
                //Point lineEnd = GetRandomPoint((int)scaledWidth, (int)scaledHeight);

                //Vector2 lineStart = GetRandomVector2Int((int)scaledWidth, (int)scaledHeight);
                //Vector2 lineEnd = GetRandomVector2Int((int)scaledWidth, (int)scaledHeight);
                //lineStart += HalfPixel;
                //lineEnd += HalfPixel;

                Vector2 lineStart = GetRandomVector2(scaledWidth, scaledHeight);
                Vector2 lineEnd = GetRandomVector2(scaledWidth, scaledHeight);

                //Vector2 lineStart = new Vector2(8.5f, 8.5f);
                //Vector2 lineEnd = new Vector2(3.5f, 8.5f);

                //Vector2 lineStart = new Vector2(10.14768f, 4.474112f);
                //Vector2 lineEnd = new Vector2(7.743159f, 6.267795f);
                //Vector2 lineStart = new Vector2(10.1f, 4.5f);
                //Vector2 lineEnd = new Vector2(8.1f, 6.5f);
                //Vector2 lineStart = new Vector2(0f, 3f);
                //Vector2 lineEnd = new Vector2(13f, 3f);

                Console.WriteLine("lineStart: {0}, lineEnd: {1}", lineStart, lineEnd);

                //DrawOrigScale(lineStart, lineEnd, g);
                DrawVariableScale(lineStart, lineEnd, g);

                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.DrawLine(new Pen(Color.FromArgb(128, 255, 255, 255), 1),
                    lineStart.X * scale, lineStart.Y * scale,
                    lineEnd.X * scale, lineEnd.Y * scale);
            }

            pictureBox1.Image = image;
        }

        private void DrawGrid(Graphics g)
        {
            float xLines = image.Width / scale;
            float yLines = image.Height / scale;

            for (int y = 0; y < yLines; y++)
            {
                g.DrawLine(Pens.DarkOliveGreen, 0, y * scale, image.Width - 1, y * scale);
            }
            for (int x = 0; x < xLines; x++)
            {
                g.DrawLine(Pens.DarkOliveGreen, x * scale, 0, x * scale, image.Height - 1);
            }
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
        private void DrawLineFullDDAOrigScaleVector2(Vector2 start, Vector2 end, Color color)
        {
            // distances between ray intersections with neighbor pixel
            // in axes X and Y
            Vector2 d = new Vector2(
                (end.X - start.X) / Math.Abs(end.Y - start.Y),
                (end.Y - start.Y) / Math.Abs(end.X - start.X));

            // distances between X and Y grid intersections along the ray
            Vector2 dist = new Vector2(
                (float)Math.Sqrt(1 + d.Y * d.Y),
                (float)Math.Sqrt(1 + d.X * d.X));

            // distance from current point to the nearest grid intersection
            Vector2 distToNext;
            {
                float distToNextX = dist.X * (float)(Math.Ceiling(start.X) - start.X);
                distToNextX = (distToNextX > 0) ? distToNextX : dist.X;
                float distToNextY = dist.Y * (float)(Math.Ceiling(start.Y) - start.Y);
                distToNextY = (distToNextY > 0) ? distToNextY : dist.Y;
                distToNext = new Vector2(distToNextX, distToNextY);
            }

            // current grid cell (pixel) coordinates
            int x = (int)Math.Floor(start.X);
            int y = (int)Math.Floor(start.Y);

            image.SetPixel(x, y, color);

            int endX = (int)Math.Ceiling(end.X);
            int endY = (int)Math.Ceiling(end.Y);

            int stepX = Math.Sign(d.X);
            int stepY = Math.Sign(d.Y);

            while ((x != endX) && (y != endY))
            {
                if (distToNext.X < distToNext.Y)
                {
                    distToNext.Y -= distToNext.X;
                    distToNext.X = dist.X;
                    x += stepX;
                }
                else
                {
                    distToNext.X -= distToNext.Y;
                    distToNext.Y = dist.Y;
                    y += stepY;
                }
                if ((x < 0) || (x >= image.Width) ||
                    (y < 0) || (y >= image.Height))
                {
                    // NOTE: if end point is inside the image
                    // this condition is useless
                    break;
                }
                image.SetPixel(x, y, color);
            }
        }

        private void DrawVariableScale(PointF lineStart, PointF lineEnd, Graphics g)
        {
            Vector2 start = new Vector2(lineStart.X, lineStart.Y);
            Vector2 end = new Vector2(lineEnd.X, lineEnd.Y);
            DrawVariableScale(start, end, g);
        }

        private void DrawVariableScale(Vector2 lineStart, Vector2 lineEnd, Graphics g)
        {
            //DrawLineDDAVariableScale(lineStart, lineEnd, Brushes.Green, g, scale);
            DrawLineFullDDAVariableScale(lineStart, lineEnd, Brushes.Green, g, scale);
        }

        private void DrawLineDDAVariableScale(
            Vector2 start, Vector2 end,
            Brush brush, Graphics g, float scale)
        {
            Vector2 d = end - start;

            int steps = (int)Math.Max(Math.Abs(d.X), Math.Abs(d.Y));
            float stepsInv = 1.0f / (float)steps;

            Vector2 increment = d * stepsInv;

            Vector2 pos = scale * new Vector2((float)Math.Round(start.X - 0.499999f), (float)Math.Round(start.Y - 0.499999f));

            float squareSize = scale;
            float squareRadius = scale / 2.0f;

            g.FillRectangle(Brushes.MediumSpringGreen, pos.X, pos.Y, squareSize, squareSize);
            g.FillRectangle(Brushes.Red, start.X * scale, start.Y * scale, 1, 1);

            pos = start;

            for (int i = 0; i < steps; i++)
            {
                pos += increment;
                float x = (float)Math.Round(pos.X - 0.499999f) * scale;
                float y = (float)Math.Round(pos.Y - 0.499999f) * scale;
                g.FillRectangle(brush, x, y, squareSize, squareSize);
                g.FillRectangle(Brushes.Red, scale * pos.X, scale * pos.Y, 1, 1);
            }
        }

        // TODO:
        // - try to simplify the code (esp. for singular cases)
        private void DrawLineFullDDAVariableScale(
            Vector2 start, Vector2 end,
            Brush brush, Graphics g, float scale)
        {
            float squareSize = scale;
            float squareRadius = scale / 2.0f;

            Vector2 lineDir = end - start;
            // singular case: zero length line segment
            if (lineDir.LengthSquared < Epsilon)
            {
                return;
            }
            // singular cases: line segment starts or end at a gridline
            if ((Math.Floor(start.X) == start.X) || (Math.Floor(start.Y) == start.Y))
            {
                start += lineDir * Epsilon;
            }
            if ((Math.Floor(end.X) == end.X) || (Math.Floor(end.Y) == end.Y))
            {
                end -= lineDir * Epsilon;
            }

            // distances between ray intersections with neighbor pixel
            // in axes X and Y
            // singular cases: line parallel to X or Y axis
            Vector2 d = new Vector2(
                (lineDir.Y != 0) ? (lineDir.X / Math.Abs(lineDir.Y)) : Math.Sign(lineDir.X),
                (lineDir.X != 0) ? (lineDir.Y / Math.Abs(lineDir.X)) : Math.Sign(lineDir.Y));

            // ray parameter span of segment of a single pixel length
            float dt = 1 / lineDir.Length;

            // distances between X and Y grid intersections along the ray
            // singular cases: line parallel to X or Y axis
            Vector2 dist = new Vector2();
            if (Math.Abs(lineDir.Y) < Epsilon) // y != 0
            {
                dist.X = Math.Abs(d.X);
                dist.Y = float.PositiveInfinity;
            }
            else if (Math.Abs(lineDir.X) < Epsilon) // x != 0
            {
                dist.X = float.PositiveInfinity;
                dist.Y = Math.Abs(d.Y);
            }
            else
            {
                dist.X = (float)Math.Sqrt(1 + d.Y * d.Y);
                dist.Y = (float)Math.Sqrt(1 + d.X * d.X);
            }

            // distance from current point to the nearest grid intersection
            Vector2 distToNext;
            {
                // NOTE: when start.X or Y is 0 it has to be rounded up to 1, not to 0
                float distToNextX = (float)(Math.Ceiling(start.X) - start.X);
                //distToNextX += ((distToNextX < 0) && (lineDir.X > 0)) ? 0 : 1;
                distToNextX = (distToNextX > 0) ? distToNextX : 1;
                distToNextX = (lineDir.X >= 0) ? distToNextX : 1 - distToNextX;
                float distToNextY = (float)(Math.Ceiling(start.Y) - start.Y);
                //distToNextY += ((distToNextX < 0) && (lineDir.Y > 0)) ? 0 : 1;
                distToNextY = (distToNextY > 0) ? distToNextY : 1;
                distToNextY = (lineDir.Y >= 0) ? distToNextY : 1 - distToNextY;
                distToNext = new Vector2(distToNextX * dist.X, distToNextY * dist.Y);
            }

            // current grid cell (pixel) coordinates
            int x = (int)Math.Floor(start.X);
            int y = (int)Math.Floor(start.Y);

            // TODO: this is not needed for pixel computation
            // it is useful only for visualization
            Vector2 pos = start;

            g.FillRectangle(Brushes.DarkGreen, x * scale, y * scale, squareSize, squareSize);
            g.FillRectangle(Brushes.Red, start.X * scale, start.Y * scale, 2, 2);

            int endX = (int)Math.Floor(end.X);
            int endY = (int)Math.Floor(end.Y);

            int stepX = Math.Sign(lineDir.X);
            int stepY = Math.Sign(lineDir.Y);

            bool finished = false;
            int i = 0;
            int maxIterations = (int)(2 * lineDir.Length);
            while (!finished)
            {
                float currentDistToNext;
                bool lastCameVertically = false;
                if (distToNext.X < distToNext.Y)
                {
                    currentDistToNext = distToNext.X;
                    distToNext.Y -= currentDistToNext;
                    distToNext.X = dist.X;
                    x += stepX;
                }
                else
                {
                    currentDistToNext = distToNext.Y;
                    distToNext.X -= currentDistToNext;
                    distToNext.Y = dist.Y;
                    y += stepY;
                    lastCameVertically = true;
                }
                pos += lineDir * currentDistToNext * dt;
                //Console.WriteLine("pos: {0}", pos);

                g.FillRectangle(brush, x * scale, y * scale, squareSize, squareSize);
                g.FillRectangle(lastCameVertically ? Brushes.Red : Brushes.Cyan,
                    pos.X * scale, pos.Y * scale, 2, 2);
                finished = (x == endX) && (y == endY);
                // TODO: this loop condition should be useless
                if (i >= maxIterations)
                {
                    Console.WriteLine("Too long loop");
                    break;
                }
                i++;
            }
            g.FillRectangle(Brushes.Yellow, end.X * scale, end.Y * scale, 2, 2);
            //g.FillRectangle(Brushes.HotPink, endX * scale, endY * scale, 2, 2);
        }

        private Point GetRandomPoint(int width, int height)
        {
            return new Point(rnd.Next(width - 1), rnd.Next(height - 1));
        }

        private Vector2 GetRandomVector2Int(int width, int height)
        {
            return new Vector2(rnd.Next(width - 1), rnd.Next(height - 1));
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
