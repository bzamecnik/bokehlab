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
    using BokehLab.Math;
    using BokehLab.RayTracing;
    using BokehLab.RayTracing.Lens;
    using OpenTK;

    public partial class SphereIntersectionForm : Form
    {
        private Sphere sphere;
        private Ray incomingRay;
        private Intersection intersection;
        private Ray refractedRay;
        private Vector3d normal;

        bool initialized = false;

        public SphereIntersectionForm()
        {
            InitializeComponent();
            sphere = new Sphere()
            {
                Radius = 100,
                Center = new Vector3d(0, 0, 0)
            };
            double directionPhi = 0; // Math.PI;
            incomingRay = new Ray(new Vector3d(5, 0, 110), new Vector3d(Math.Sin(directionPhi), 0, Math.Cos(directionPhi)));

            rayDirectionPhiNumeric.Value = (decimal)directionPhi;
            sphereRadiusNumeric.Value = (decimal)sphere.Radius;
            FillVectorToControls(sphere.Center, sphereCenterXNumeric, sphereCenterYNumeric, sphereCenterZNumeric);
            FillVectorToControls(incomingRay.Origin, rayOriginXNumeric, rayOriginYNumeric, rayOriginZNumeric);
            initialized = true;
            Recompute();
        }

        private void Recompute()
        {
            if (!initialized)
            {
                return;
            }

            sphere.Center = GetVectorFromControls(sphereCenterXNumeric, sphereCenterYNumeric, sphereCenterZNumeric);
            incomingRay.Origin = GetVectorFromControls(rayOriginXNumeric, rayOriginYNumeric, rayOriginZNumeric);
            double directionPhi = (double)rayDirectionPhiNumeric.Value;
            incomingRay.Direction = new Vector3d(Math.Sin(directionPhi), 0, Math.Cos(directionPhi));

            intersection = sphere.Intersect(incomingRay);
            refractedRay = null;
            normal = Vector3d.Zero;
            if (intersection != null)
            {
                normal = sphere.GetNormal(intersection.Position);
                double n1, n2;
                double dot = Vector3d.Dot(normal, incomingRay.Direction);
                if (dot < 0)
                {
                    n1 = Materials.Fixed.AIR;
                    n2 = Materials.Fixed.GLASS_CROWN_BK7;
                }
                else
                {
                    n1 = Materials.Fixed.GLASS_CROWN_BK7;
                    n2 = Materials.Fixed.AIR;
                    normal = -normal;
                }
                refractedRay = new Ray(intersection.Position,
                    Ray.Refract(incomingRay.Direction, normal, n1, n2, false));
            }

            drawingPanel.Invalidate();
        }

        private void drawingPanel_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            float panelHalfWidth = e.ClipRectangle.Width / 2.0f;
            float panelHalfHeight = e.ClipRectangle.Height / 2.0f;
            g.TranslateTransform(panelHalfWidth, panelHalfHeight);
            g.ScaleTransform(1.0f, -1.0f);

            // draw X axis
            g.DrawLine(Pens.Black, -panelHalfWidth, 0, panelHalfWidth, 0);

            // draw Y axis
            g.DrawLine(Pens.Black, 0, -panelHalfWidth, 0, panelHalfWidth);

            //g.ScaleTransform(-1.0f, 1.0f);
            //g.TranslateTransform(200.0f, 0.0f);

            PaintScene(g);
        }

        private void PaintScene(Graphics g)
        {
            // draw a circlular lens
            //DrawCircle(g, Pens.Blue, new Point(), (float)Bench.Elements.Radius);

            //g.TranslateTransform((float)-Bench.LensCenter, 0.0f);

            // draw sphere
            DrawCircle(g, Pens.Black, Vector3dToPoint(sphere.Center), (float)sphere.Radius);

            // draw ray
            if (incomingRay.Direction != Vector3d.Zero)
            {
                Point origin = Vector3dToPoint(incomingRay.Origin);
                Point target;
                if (intersection != null)
                {
                    target = Vector3dToPoint(intersection.Position);
                }
                else
                {
                    target = Vector3dToPoint(incomingRay.Origin + 1000 * Vector3d.Normalize(incomingRay.Direction));
                }
                g.DrawLine(Pens.Green, origin, target);
            }

            if ((refractedRay != null) && (refractedRay.Direction != Vector3d.Zero))
            {
                Point origin = Vector3dToPoint(refractedRay.Origin);
                Point target = Vector3dToPoint(refractedRay.Origin + 1000 * Vector3d.Normalize(refractedRay.Direction));
                g.DrawLine(Pens.Orange, origin, target);
            }

            // draw intersection if there is any
            if (intersection != null)
            {
                Point intersectionPos = Vector3dToPoint(intersection.Position);
                FillSquare(g, Brushes.Red, intersectionPos, 3);
                //draw normal
                if (normal != Vector3d.Zero)
                {
                    g.DrawLine(Pens.Purple, intersectionPos, Vector3dToPoint(intersection.Position + 20 * normal));
                }
            }
        }

        private Point Vector3dToPoint(Vector3d vector)
        {
            return new Point((int)vector.Z, (int)vector.X);
        }

        private void DrawCircle(Graphics g, Pen pen, Point center, float radius)
        {
            g.DrawEllipse(pen, center.X - radius, center.Y - radius, 2 * radius, 2 * radius);
        }

        private void FillSquare(Graphics g, Brush brush, Point center, float radius)
        {
            g.FillRectangle(brush, center.X - radius, center.Y - radius, 2 * radius, 2 * radius);
        }

        private Vector3d GetVectorFromControls(
           NumericUpDown xNumeric,
           NumericUpDown yNumeric,
           NumericUpDown zNumeric)
        {
            return new Vector3d(
                (double)xNumeric.Value,
                (double)yNumeric.Value,
                (double)zNumeric.Value);
        }

        private void FillVectorToControls(Vector3d vector,
          NumericUpDown xNumeric,
          NumericUpDown yNumeric,
          NumericUpDown zNumeric)
        {
            xNumeric.Value = (decimal)vector.X;
            yNumeric.Value = (decimal)vector.Y;
            zNumeric.Value = (decimal)vector.Z;
        }

        private void SceneControlsValueChanged(object sender, EventArgs e)
        {
            Recompute();
        }

        private void rayDirectionYNumeric_ValueChanged(object sender, EventArgs e)
        {
            Recompute();
        }

        private void rayDirectionZNumeric_ValueChanged(object sender, EventArgs e)
        {
            Recompute();
        }

        private void rayOriginXNumeric_ValueChanged(object sender, EventArgs e)
        {
            Recompute();
        }

        private void rayOriginYNumeric_ValueChanged(object sender, EventArgs e)
        {
            Recompute();
        }

        private void rayOriginZNumeric_ValueChanged(object sender, EventArgs e)
        {
            Recompute();
        }

        private void SphereIntersectionForm_Resize(object sender, EventArgs e)
        {
            Recompute();
        }

        private void sphereRadiusNumeric_ValueChanged(object sender, EventArgs e)
        {
            sphere.Radius = (double)sphereRadiusNumeric.Value;
            Recompute();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Recompute();
        }

    }
}
