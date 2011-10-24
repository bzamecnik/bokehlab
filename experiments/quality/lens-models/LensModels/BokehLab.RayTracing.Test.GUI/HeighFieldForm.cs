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
        List<FloatMapImage> layers = new List<FloatMapImage>();
        List<Bitmap> layerBitmaps = new List<Bitmap>();

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

        private void addLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (var fileName in openFileDialog.FileNames)
                {
                    Bitmap newLayerBitmap = (Bitmap)Bitmap.FromFile(fileName);
                    layerBitmaps.Add(newLayerBitmap);
                    layers.Add(newLayerBitmap.ToFloatMap());
                }
                heightField = new HeightField(layers.ToArray());
                UpdateHeightfieldPanel();
            }
        }

        private void UpdateHeightfieldPanel()
        {
            heightFieldPanel.Size = new Size(heightField.Width, heightField.Height);
            heightFieldPanel.Invalidate();
            layerCountLabel.Text = heightField.LayerCount.ToString();
        }

        private void clearAllLayersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            layerBitmaps.Clear();
            layers.Clear();
            heightField = new HeightField(new[] { new FloatMapImage(1, 1, PixelFormat.Greyscale) });
            UpdateHeightfieldPanel();
        }

        private void heightFieldPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (layers.Count == 0)
            {
                return;
            }

            if ((e.Location.X < 0) || (e.Location.X > heightField.Width) ||
                (e.Location.Y < 0) || (e.Location.Y > heightField.Height))
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                editingRay = true;
                rayStartPoint = e.Location;
                //rayStart.X = rayStartPoint.X;
                //rayStart.Y = rayStartPoint.Y;
            }
        }

        private void heightFieldPanel_MouseUp(object sender, MouseEventArgs e)
        {
            editingRay = false;
        }

        private void heightFieldPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if ((layerBitmaps == null) || !editingRay)
            {
                return;
            }

            if ((e.Location.X < 0) || (e.Location.X >= heightField.Width) ||
                (e.Location.Y < 0) || (e.Location.Y >= heightField.Height))
            {
                return;
            }

            rayEndPoint = e.Location;
            //rayEnd.X = rayEndPoint.X;
            //rayEnd.Y = rayEndPoint.Y;

            ComputeIntersection();
        }

        private void UpdateRayLabels()
        {
            rayStartXYZLabel.Text = rayStart.ToString();
            rayEndXYZLabel.Text = rayEnd.ToString();
        }

        private void ComputeIntersection()
        {
            rayEnd.X = rayEndPoint.X;
            rayEnd.Y = rayEndPoint.Y;

            rayStart.X = rayStartPoint.X;
            rayStart.Y = rayStartPoint.Y;

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
            if (layerBitmaps.Count > 0)
            {
                g.DrawImage(layerBitmaps[0], 0, 0, heightField.Width, heightField.Height);
                g.DrawLine(Pens.DarkGreen, rayStartPoint, rayEndPoint);
                if (intersection != null)
                {
                    g.DrawRectangle(Pens.Red, (int)intersection.Position.X, (int)intersection.Position.Y, 1, 1);
                }
            }
        }
    }
}
