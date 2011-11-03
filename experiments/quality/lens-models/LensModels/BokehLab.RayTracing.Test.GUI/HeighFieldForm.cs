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
        static readonly HeightField emptyHeightField = new HeightField(new FloatMapImage[] { }) { Width = 30, Height = 20 };
        HeightField heightField = emptyHeightField;
        List<FloatMapImage> layers = new List<FloatMapImage>();
        List<Bitmap> layerBitmaps = new List<Bitmap>();

        Point rayStartPoint;
        Point rayEndPoint;
        Vector3d rayStart;
        Vector3d rayEnd;

        bool editingRay = false;

        bool updatingGuiVectors = false;

        Intersection intersection;
        HeightField.FootprintDebugInfo footprintDebugInfo;

        float footprintScale = 32.0f;

        int cocFootprintRadius = 5;
        int cocRadius = 20;
        Vector2 cocCenter;
        int lightSourceLayer = 0;

        bool editingCocCenter = false;

        Sampler sampler = new Sampler();

        public HeighFieldForm()
        {
            InitializeComponent();

            SetDoubleBuffered(heightFieldPanel);
            SetDoubleBuffered(footprintTraversalPanel);
            SetDoubleBuffered(cocClippingPanel);

            rayStart.Z = 0;
            rayEnd.Z = 1;

            cocFootprintRadiusNumeric.Value = (decimal)cocFootprintRadius;
            cocRadiusNumeric.Value = (decimal)cocRadius;

            epsilonForCloseDepthNumeric.Value = (decimal)heightField.EpsilonForClosePixelDepth;

            UpdateHeightfieldPanel();
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

            footprintTraversalPanel.Size = new Size((int)(footprintScale * heightField.Width) + 1, (int)(footprintScale * heightField.Height) + 1);
            footprintTraversalPanel.Invalidate();

            cocClippingPanel.Size = new Size(heightField.Width, heightField.Height);
            cocCenter = new Vector2(heightField.Width / 2, heightField.Height / 2);
            lightSourceLayerNumeric.Maximum = Math.Max(heightField.LayerCount - 1, 0);
            cocClippingPanel.Invalidate();

            layerCountLabel.Text = heightField.LayerCount.ToString();
        }

        private void clearAllLayersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            layerBitmaps.Clear();
            layers.Clear();
            heightField = emptyHeightField;
            UpdateHeightfieldPanel();
        }

        private void heightFieldPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                panelMouseDown(e.Location, 1);
            }
        }

        private void footprintTraversalPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                panelMouseDown(e.Location, footprintScale);
            }
        }

        private void panelMouseDown(Point location, float scale)
        {
            if ((location.X < 0) || (location.X >= heightField.Width * scale) ||
                (location.Y < 0) || (location.Y >= heightField.Height * scale))
            {
                return;
            }

            editingRay = true;
            rayStartPoint = location;
            rayStart.X = rayStartPoint.X / scale;
            rayStart.Y = rayStartPoint.Y / scale;

            UpdateGui();
        }

        private void heightFieldPanel_MouseUp(object sender, MouseEventArgs e)
        {
            editingRay = false;
        }

        private void footprintTraversalPanel_MouseUp(object sender, MouseEventArgs e)
        {
            editingRay = false;
        }

        private void heightFieldPanel_MouseMove(object sender, MouseEventArgs e)
        {
            //panelMouseMove(e.Location, 1, false);
            panelMouseMove(e.Location, 1, true);
        }

        private void footprintTraversalPanel_MouseMove(object sender, MouseEventArgs e)
        {
            panelMouseMove(e.Location, footprintScale, true);
        }

        private void panelMouseMove(Point location, float scale, bool computeDebugInfo)
        {
            if (!editingRay)
            {
                return;
            }

            if ((location.X < 0) || (location.X >= heightField.Width * scale) ||
                (location.Y < 0) || (location.Y >= heightField.Height * scale))
            {
                return;
            }

            rayEndPoint = location;
            rayEnd.X = rayEndPoint.X / scale;
            rayEnd.Y = rayEndPoint.Y / scale;

            UpdateGui();

            ComputeIntersection(computeDebugInfo);
        }

        private void UpdateGui()
        {
            rayStartXYZLabel.Text = rayStart.ToString();
            rayEndXYZLabel.Text = rayEnd.ToString();

            updatingGuiVectors = true;

            rayStartXNumeric.Value = (decimal)rayStart.X;
            rayStartYNumeric.Value = (decimal)rayStart.Y;
            rayStartZNumeric.Value = (decimal)rayStart.Z;

            rayEndXNumeric.Value = (decimal)rayEnd.X;
            rayEndYNumeric.Value = (decimal)rayEnd.Y;
            rayEndZNumeric.Value = (decimal)rayEnd.Z;

            updatingGuiVectors = false;
        }

        private void ComputeIntersection(bool withDebugInfo)
        {
            rayStartPoint = new Point((int)rayStart.X, (int)rayStart.Y);
            rayEndPoint = new Point((int)rayEnd.X, (int)rayEnd.Y);
            if (withDebugInfo)
            {
                footprintDebugInfo = new HeightField.FootprintDebugInfo();
            }
            else
            {
                footprintDebugInfo = null;
            }
            intersection = heightField.Intersect(new Ray(rayStart, rayEnd - rayStart), ref footprintDebugInfo);
            intersectionLabel.Text = (intersection != null) ? intersection.Position.ToString() : "none";
            isecLayerLabel.Text = (footprintDebugInfo != null) && (intersection != null) ? footprintDebugInfo.LayerOfIntersection.ToString() : "";

            heightFieldPanel.Invalidate();
            footprintTraversalPanel.Invalidate();
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

        private void rayStartXNumeric_ValueChanged(object sender, EventArgs e)
        {
            if (!updatingGuiVectors)
            {
                rayStart.X = (float)rayStartXNumeric.Value;
            }
        }

        private void rayStartYNumeric_ValueChanged(object sender, EventArgs e)
        {
            if (!updatingGuiVectors)
            {
                rayStart.Y = (float)rayStartYNumeric.Value;
            }
        }

        private void rayStartZNumeric_ValueChanged(object sender, EventArgs e)
        {
            if (!updatingGuiVectors)
            {
                rayStart.Z = (float)rayStartZNumeric.Value;
            }
        }

        private void rayEndXNumeric_ValueChanged(object sender, EventArgs e)
        {
            if (!updatingGuiVectors)
            {
                rayEnd.X = (float)rayEndXNumeric.Value;
            }
        }

        private void rayEndYNumeric_ValueChanged(object sender, EventArgs e)
        {
            if (!updatingGuiVectors)
            {
                rayEnd.Y = (float)rayEndYNumeric.Value;
            }
        }

        private void rayEndZNumeric_ValueChanged(object sender, EventArgs e)
        {
            if (!updatingGuiVectors)
            {
                rayEnd.Z = (float)rayEndZNumeric.Value;
            }
        }

        private void recomputeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComputeIntersection(true);
        }

        private void DrawGrid(Graphics g, Size size, float scale)
        {
            float width = size.Width * scale;
            float height = size.Height * scale;

            for (int y = 0; y <= size.Height; y++)
            {
                g.DrawLine(Pens.DarkOliveGreen, 0, y * scale, width - 1, y * scale);
            }
            for (int x = 0; x <= size.Width; x++)
            {
                g.DrawLine(Pens.DarkOliveGreen, x * scale, 0, x * scale, height - 1);
            }
        }

        private void footprintTraversalPanel_Paint(object sender, PaintEventArgs e)
        {
            float scale = footprintScale;
            var g = e.Graphics;
            g.Clear(Color.Black);
            DrawGrid(g, new Size(heightField.Width, heightField.Height), scale);

            Brush brush = new SolidBrush(Color.FromArgb(128, 0, 100, 0));
            Brush brushStart = new SolidBrush(Color.FromArgb(128, 100, 0, 0));
            Brush brushEnd = new SolidBrush(Color.FromArgb(128, 0, 0, 100));

            if (footprintDebugInfo != null)
            {
                foreach (var pixel in footprintDebugInfo.VisitedPixels)
                {
                    g.FillRectangle(brush, scale * pixel.X + 1, scale * pixel.Y + 1, scale - 1, scale - 1);
                }
                g.FillRectangle(brushStart, scale * footprintDebugInfo.StartPixel.X + 1, scale * footprintDebugInfo.StartPixel.Y + 1, scale - 1, scale - 1);
                g.FillRectangle(brushEnd, scale * footprintDebugInfo.EndPixel.X + 1, scale * footprintDebugInfo.EndPixel.Y + 1, scale - 1, scale - 1);
                int i = 1;
                float totalInv = 1f / footprintDebugInfo.EntryPoints.Count;
                foreach (var entryPoint in footprintDebugInfo.EntryPoints)
                {
                    float ratio = 255 * i * totalInv;
                    Brush b = new SolidBrush(Color.FromArgb(255, (int)ratio, 128, (int)(255 - ratio)));
                    g.FillRectangle(b, entryPoint.X * scale - 1, entryPoint.Y * scale - 1, 3, 3);
                    i++;
                }
            }

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.DrawLine(new Pen(Color.FromArgb(128, 255, 255, 255), 1),
                (float)rayStart.X * scale, (float)rayStart.Y * scale,
                (float)rayEnd.X * scale, (float)rayEnd.Y * scale);
        }

        private void cocClippingPanel_Paint(object sender, PaintEventArgs e)
        {
            if (layerBitmaps.Count <= 0)
            {
                return;
            }

            var g = e.Graphics;

            g.DrawImage(layerBitmaps[lightSourceLayer], 0, 0, heightField.Width, heightField.Height);

            int cocSize = 2 * cocRadius + 1;
            // The light source cannot be at the pixel edge as it will not be
            // easy to test if there was only a single intersection - with the
            // light source. If the light source is within the pixel it is easier.
            Vector3d lightSource = new Vector3d(cocCenter.X + 0.5f, cocCenter.Y + 0.5f,
                heightField.GetDepth((int)cocCenter.X, (int)cocCenter.Y, lightSourceLayer));
            if (lightSource.Z == 1)
            {
                return;
            }
            Rectangle coc = new Rectangle(
                (int)cocCenter.X - cocRadius,
                (int)cocCenter.Y - cocRadius,
                cocSize,
                cocSize);
            g.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(200, 0, 200, 0))), coc);
            Brush yellowBrush = new SolidBrush(Color.FromArgb(200, 255, 255, 0));
            Brush blueBrush = new SolidBrush(Color.FromArgb(200, 0, 0, 100));
            float ratio = cocFootprintRadius / (float)cocRadius;
            for (int y = -cocRadius; y <= cocRadius; y++)
            {
                for (int x = -cocRadius; x <= cocRadius; x++)
                {
                    Vector2d[] jitter = sampler.GenerateJitteredSamples(2).ToArray();

                    int visibleSamples = 0;
                    for (int i = 0; i < jitter.Length; i++)
                    {
                        Vector3d origin = new Vector3d(
                            cocCenter.X + ratio * (x + jitter[i].X),
                            cocCenter.Y + ratio * (y + jitter[i].Y), 0);
                        if ((origin.X >= 0) && (origin.X < heightField.Width) &&
                            (origin.Y >= 0) && (origin.Y < heightField.Height))
                        {
                            Intersection isec = selectedIntersector.Intersect(new Ray(origin, lightSource - origin));
                            if ((isec == null) || (isec.Position.Z >= lightSource.Z - 0.1f))
                            {
                                visibleSamples++;
                            }
                        }
                    }
                    float visibilityRatio = visibleSamples / (float)jitter.Length;
                    int intensity = (int)(255 * visibilityRatio);
                    g.FillRectangle(new SolidBrush(Color.FromArgb(200, intensity, intensity, 255 - intensity)), cocCenter.X + x, cocCenter.Y + y, 1, 1);
                }
            }
        }

        private void lightSourceNumeric_ValueChanged(object sender, EventArgs e)
        {
            lightSourceLayer = (int)lightSourceLayerNumeric.Value;
            if (lightSourceLayer < heightField.LayerCount)
            {
                cocClippingPanel.Invalidate();
            }
        }

        private void cocFootprintRadiusNumeric_ValueChanged(object sender, EventArgs e)
        {
            cocFootprintRadius = (int)cocFootprintRadiusNumeric.Value;
            cocClippingPanel.Invalidate();
        }

        private void cocClippingPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                editingCocCenter = true;
            }
        }

        private void cocClippingPanel_MouseUp(object sender, MouseEventArgs e)
        {
            editingCocCenter = false;
        }

        private void cocClippingPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (editingCocCenter)
            {
                if ((e.Location.X < 0) || (e.Location.X >= heightField.Width) ||
                    (e.Location.Y < 0) || (e.Location.Y >= heightField.Height))
                {
                    return;
                }

                cocCenter = new Vector2(e.Location.X, e.Location.Y);
                cocClippingPanel.Invalidate();
            }
        }

        private void cocRadiusNumeric_ValueChanged(object sender, EventArgs e)
        {
            cocRadius = (int)cocRadiusNumeric.Value;
            cocClippingPanel.Invalidate();
        }

        private void epsilonForCloseDepthNumeric_ValueChanged(object sender, EventArgs e)
        {
            heightField.EpsilonForClosePixelDepth = (float)epsilonForCloseDepthNumeric.Value;
            heightFieldPanel.Invalidate();
            cocClippingPanel.Invalidate();
        }
    }
}
