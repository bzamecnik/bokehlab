namespace BokehLab.RayTracing.Test.GUI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using BokehLab.FloatMap;
    using BokehLab.Math;
    using OpenTK;

    public partial class HeighFieldForm : Form
    {
        HeightField heightField = new HeightField(new[] { new FloatMapImage(1, 1, PixelFormat.Greyscale) });
        FloatMapImage layer;
        Bitmap layerBitmap;

        Point rayStartPoint;
        Point rayEndPoint;
        Vector3d rayStart;
        Vector3d rayEnd;

        bool editingRay = false;

        Intersection intersection;

        public HeighFieldForm()
        {
            InitializeComponent();

            SetDoubleBuffered(heightFieldPanel);

            rayStart = new Vector3d(20.5, 30.5, 0);
            rayEnd = new Vector3d(25.5, 31.5, 0);
            rayStart.Z = (double)rayStartZNumeric.Value;
            rayEnd.Z = (double)rayEndZNumeric.Value;
        }

        public static void SetDoubleBuffered(System.Windows.Forms.Control c)
        {
            // source: http://stackoverflow.com/questions/76993/how-to-double-buffer-net-controls-on-a-form

            //Taxes: Remote Desktop Connection and painting
            //http://blogs.msdn.com/oldnewthing/archive/2006/01/03/508694.aspx
            if (System.Windows.Forms.SystemInformation.TerminalServerSession)
                return;

            System.Reflection.PropertyInfo aProp =
                  typeof(System.Windows.Forms.Control).GetProperty(
                        "DoubleBuffered",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

            aProp.SetValue(c, true, null);
        }

        private void openHeightfieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                layerBitmap = (Bitmap)Bitmap.FromFile(openFileDialog.FileName);
                layer = layerBitmap.ToFloatMap();
                heightField = new HeightField(new FloatMapImage[] { layer });
                heightFieldPanel.Invalidate();
            }
        }

        private void heightFieldPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (layerBitmap == null)
            {
                return;
            }

            if ((e.Location.X < 0) || (e.Location.X > layerBitmap.Width) ||
                (e.Location.Y < 0) || (e.Location.Y > layerBitmap.Height))
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                editingRay = true;
                rayStartPoint = e.Location;
                rayStart.X = rayStartPoint.X;
                rayStart.Y = rayStartPoint.Y;
            }
        }

        private void heightFieldPanel_MouseUp(object sender, MouseEventArgs e)
        {
            editingRay = false;
        }

        private void heightFieldPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if ((layerBitmap == null) || !editingRay)
            {
                return;
            }

            if ((e.Location.X < 0) || (e.Location.X > layerBitmap.Width) ||
                (e.Location.Y < 0) || (e.Location.Y > layerBitmap.Height))
            {
                return;
            }

            rayEndPoint = e.Location;
            rayEnd.X = rayEndPoint.X;
            rayEnd.Y = rayEndPoint.Y;

            ComputeIntersection();
        }

        private void UpdateRayLabels()
        {
            rayStartXYZLabel.Text = rayStart.ToString();
            rayEndXYZLabel.Text = rayEnd.ToString();
        }

        private void ComputeIntersection()
        {
            intersection = heightField.Intersect(new Ray(rayStart, rayEnd - rayStart));
            intersectionLabel.Text = (intersection != null) ? intersection.Position.ToString() : "none";

            UpdateRayLabels();
            heightFieldPanel.Invalidate();
        }

        private void rayStartZNumeric_ValueChanged(object sender, EventArgs e)
        {
            rayStart.Z = (float)rayStartZNumeric.Value;
            ComputeIntersection();
        }

        private void rayEndZNumeric_ValueChanged(object sender, EventArgs e)
        {
            rayEnd.Z = (float)rayEndZNumeric.Value;
            ComputeIntersection();
        }

        private void heightFieldPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            if (layerBitmap != null)
            {
                g.DrawImage(layerBitmap, 0, 0, layerBitmap.Width, layerBitmap.Height);
            }
            g.DrawLine(Pens.DarkGreen, rayStartPoint, rayEndPoint);

            if (intersection != null)
            {
                g.DrawRectangle(Pens.Red, (int)intersection.Position.X, (int)intersection.Position.Y, 1, 1);
            }
        }
    }
}
