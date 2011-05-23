﻿namespace BokehLab.RayTracing.Test.GUI
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

    public partial class ComplexLensForm : Form
    {
        private ComplexLens complexLens;
        private Ray incomingRay;
        private Ray outgoingRay;
        private Vector3d backLensPos;

        bool initialized = false;

        public ComplexLensForm()
        {
            InitializeComponent();
            complexLens = CreateLens();
            double directionPhi = Math.PI;
            incomingRay = new Ray(new Vector3d(25.2, 0, 150), new Vector3d(Math.Sin(directionPhi), 0, Math.Cos(directionPhi)));

            rayDirectionPhiNumeric.Value = (decimal)directionPhi;
            FillVectorToControls(incomingRay.Origin, rayOriginXNumeric, rayOriginYNumeric, rayOriginZNumeric);
            initialized = true;
            Recompute();
        }

        private ComplexLens CreateLens()
        {
            //double curvatureRadius = 150;
            //double apertureRadius = 100;
            //return ComplexLens.CreateBiconvexLens(curvatureRadius, apertureRadius, 100);

            //var surfaces = new List<ComplexLens.SphericalElementSurfaceDefinition>();
            //surfaces.Add(new ComplexLens.SphericalElementSurfaceDefinition()
            //{
            //    CurvatureRadius = null,//58.950,
            //    Thickness = 5.520,
            //    NextRefractiveIndex = 1.670,
            //    ApertureDiameter = 50.4,
            //});
            //surfaces.Add(new ComplexLens.SphericalElementSurfaceDefinition()
            //{
            //    CurvatureRadius = null,//169.660,
            //    Thickness = 12.240,
            //    NextRefractiveIndex = Materials.Fixed.AIR,
            //    ApertureDiameter = 50.4,
            //});
            //surfaces.Add(new ComplexLens.SphericalElementSurfaceDefinition()
            //{
            //    CurvatureRadius = null,//38.550,
            //    Thickness = 18.050,
            //    NextRefractiveIndex = 1.670,
            //    ApertureDiameter = 46.0,
            //});
            //return ComplexLens.Create(surfaces, Materials.Fixed.AIR);

            return ComplexLens.CreateDoubleGaussLens();
        }

        private void Recompute()
        {
            if (!initialized)
            {
                return;
            }

            incomingRay.Origin = GetVectorFromControls(rayOriginXNumeric, rayOriginYNumeric, rayOriginZNumeric);
            double directionPhi = (double)rayDirectionPhiNumeric.Value;
            incomingRay.Direction = new Vector3d(Math.Sin(directionPhi), 0, Math.Cos(directionPhi));

            Intersection backInt = complexLens.Intersect(incomingRay);
            if (backInt != null)
            {
                outgoingRay = complexLens.Transfer(incomingRay.Origin, backInt.Position);
                backLensPos = backInt.Position;
            }
            else
            {
                outgoingRay = null;
                backLensPos = Vector3d.Zero;
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
            float scale = 2.0f;
            g.ScaleTransform(scale, scale);

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
            DrawComplexLens(g, complexLens);
            DrawRays(g);
        }

        private void DrawRays(Graphics g)
        {
            // draw incoming ray
            if (incomingRay.Direction != Vector3d.Zero)
            {
                Point origin = Vector3dToPoint(incomingRay.Origin);
                Point target;
                if (outgoingRay != null)
                {
                    target = Vector3dToPoint(backLensPos);
                }
                else
                {
                    target = Vector3dToPoint(incomingRay.Origin + 1000 * Vector3d.Normalize(incomingRay.Direction));
                }
                g.DrawLine(Pens.Green, origin, target);
            }

            if (backLensPos != Vector3d.Zero)
            {
                FillSquare(g, Brushes.Red, Vector3dToPoint(backLensPos), 3);
            }

            // draw outgoing ray
            if ((outgoingRay != null) && (outgoingRay.Direction != Vector3d.Zero))
            {
                Point origin = Vector3dToPoint(outgoingRay.Origin);
                Point target = Vector3dToPoint(outgoingRay.Origin + 1000 * Vector3d.Normalize(outgoingRay.Direction));
                g.DrawLine(Pens.Brown, origin, target);
                g.DrawLine(Pens.DarkGreen, Vector3dToPoint(backLensPos), origin);

                //// draw normal
                //g.DrawLine(Pens.Purple, origin, Vector3dToPoint(outgoingRay.Origin + 20 * -complexLens.ElementSurfaces.Last().SurfaceNormalField.GetNormal(outgoingRay.Origin)));

                //// draw normal
                //g.DrawLine(Pens.Purple, Vector3dToPoint(backLensPos), Vector3dToPoint(backLensPos + 20 * -complexLens.ElementSurfaces.First().SurfaceNormalField.GetNormal(backLensPos)));
            }
        }

        private void DrawComplexLens(Graphics g, ComplexLens lens)
        {
            float maxApertureRadius = (float)lens.ElementSurfaces.Select(
                surface => surface.ApertureRadius).Max();
            // draw spheres
            foreach (var surface in lens.ElementSurfaces)
            {
                if (surface.Surface is Sphere)
                {
                    Sphere sphere = (Sphere)surface.Surface;
                    //DrawCircle(g, Pens.Blue, Vector3dToPoint(sphere.Center), (float)sphere.Radius);
                    //FillSquare(g, Brushes.Aquamarine, Vector3dToPoint(sphere.Center), 3);
                    DrawSphericalCap(g, sphere, surface.ApertureRadius, surface.Convex);
                }
                else if (surface.Surface is Circle)
                {
                    Circle circle = (Circle)surface.Surface;
                    DrawCircularStop(g, Pens.Violet, Vector3dToPoint(
                        new Vector3d(0, 0, circle.Z)), (float)circle.Radius, maxApertureRadius);
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

        private void DrawPlanarSurface(Graphics g, Pen pen, Point center, float radius)
        {
            g.DrawLine(pen, center.X, center.Y - radius, center.X, center.Y + radius);
        }

        private void DrawCircularStop(Graphics g, Pen pen, Point center, float innerRadius, float outerRadius)
        {
            g.DrawLine(pen, center.X, center.Y + innerRadius, center.X, center.Y + (innerRadius + outerRadius));
            g.DrawLine(pen, center.X, center.Y - innerRadius, center.X, center.Y - (innerRadius + outerRadius));
        }

        private void FillSquare(Graphics g, Brush brush, Point center, float radius)
        {
            g.FillRectangle(brush, center.X - radius, center.Y - radius, 2 * radius, 2 * radius);
        }

        /// <summary>
        /// Draws a spherical cap (in 2D).
        /// </summary>
        /// <param name="g">graphics</param>
        /// <param name="angle">Elevation (?) angle (in radians).</param>
        /// <param name="baseRadius">Radius of the sphere.
        /// </param>
        /// <param name="convex">Indicates whether the cap is convex when viewed
        /// from back.</param>
        private void DrawSphericalCap(Graphics g, Sphere sphere, double apertureRadius, bool convex)
        {
            double angle = Math.Asin(sphere.GetCapElevationAngleSine(apertureRadius));
            float angleDeg = (float)(angle * 180.0 / Math.PI);
            angleDeg = 90 - angleDeg;
            float startAngle = (convex ? 360.0f : 180.0f) - angleDeg;
            float sweepAngle = 2.0f * angleDeg;
            float radius = (float)sphere.Radius;
            g.TranslateTransform((float)sphere.Center.Z, 0.0f);
            g.DrawArc(Pens.Blue, -radius, -radius, 2 * radius, 2 * radius, startAngle, sweepAngle);
            g.TranslateTransform((float)-sphere.Center.Z, 0.0f);

            //float x = (float)((cap.Convex ? cap.Radius : -cap.Radius) - (cap.Convex ? cap.Thickness : -cap.Thickness));
            //float y = (float)cap.Aperture;
            //g.DrawLine(Pens.Black, x, -y, x, y);
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

        private void button1_Click(object sender, EventArgs e)
        {
            Recompute();
        }

        //private void curvatureRadiusNumeric_ValueChanged(object sender, EventArgs e)
        //{
        //    complexLens.CurvatureRadius = (double)curvatureRadiusNumeric.Value;
        //    Recompute();
        //}

        //private void apertureRadiusNumeric_ValueChanged(object sender, EventArgs e)
        //{
        //    complexLens.ApertureRadius = (double)apertureRadiusNumeric.Value;
        //    Recompute();
        //}

        private void complexLensForm_Resize(object sender, EventArgs e)
        {
            Recompute();
        }
    }
}