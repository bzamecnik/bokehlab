namespace BokehLab.RayTracing.Test.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
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
        private IList<Vector3d> intersections;
        double directionPhi;
        // lens position parameter (sample for back surface sampling, 0.0-1.0)
        double lensPosU;

        // directly provide the position on lens back surface or derive it by
        // intersecting the incoming ray with the lens
        bool inputLensPosDirectly = false;

        bool initialized = false;

        public ComplexLensForm()
        {
            InitializeComponent();
            complexLens = CreateLens();
            //directionPhi = Math.PI;
            directionPhi = 1.0;
            incomingRay = new Ray(new Vector3d(25, 0, 300), new Vector3d(Math.Sin(directionPhi), 0, Math.Cos(directionPhi)));

            rayDirectionPhiNumeric.Value = (decimal)directionPhi;
            FillVectorToControls(incomingRay.Origin, rayOriginXNumeric, rayOriginYNumeric, rayOriginZNumeric);
            initialized = true;
            Recompute();
        }

        private ComplexLens CreateLens()
        {
            //double curvatureRadius = 150;
            //double apertureRadius = 100;
            //return ComplexLens.CreateBiconvexLens(curvatureRadius, apertureRadius, 0);
            return ComplexLens.CreateDoubleGaussLens(Materials.Fixed.AIR, 4.0);
            //return ComplexLens.CreatePetzvalLens(Materials.Fixed.AIR, 4.0);
        }

        private void Recompute()
        {
            if (!initialized)
            {
                return;
            }

            directionPhi = (double)rayDirectionPhiNumeric.Value;
            if (inputLensPosDirectly)
            {
                incomingRay.Origin = GetVectorFromControls(rayOriginXNumeric, rayOriginYNumeric, rayOriginZNumeric);
                incomingRay.Direction = new Vector3d(Math.Sin(directionPhi), 0, Math.Cos(directionPhi));
            }
            else
            {
                // compute lens position from lens position parameter
                // (with Y = 0)
                double lensPosV = 0.5;
                lensPosU = (double)lensPosTNumeric.Value;
                if (lensPosU > 1.0)
                {
                    lensPosU = 2.0 - lensPosU;
                    lensPosV = 0.0;
                }
                incomingRay = complexLens.ConvertParametersToBackSurfaceRay(
                    new LensRayTransferFunction.Parameters(
                        lensPosU, lensPosV, directionPhi, 0));
                //Console.WriteLine("IN: {0}", incomingRay);
            }

            intersections = new List<Vector3d>();
            outgoingRay = complexLens.TransferDebug(incomingRay, out intersections, true);
            if (!inputLensPosDirectly)
            {
                backLensPos = incomingRay.Origin;
                //Console.WriteLine("OUT: {0}", outgoingRay);
            }
            else
            {
                if (outgoingRay != null)
                {
                    Intersection backInt = complexLens.Intersect(incomingRay);
                    backLensPos = backInt.Position;
                }
                else
                {
                    backLensPos = Vector3d.Zero;
                }
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
            float scale = 1.0f;
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
                //g.DrawLine(Pens.DarkGreen, Vector3dToPoint(backLensPos), origin);

                //// draw normal
                //g.DrawLine(Pens.Purple, origin, Vector3dToPoint(outgoingRay.Origin + 20 * -complexLens.ElementSurfaces.Last().SurfaceNormalField.GetNormal(outgoingRay.Origin)));

                //// draw normal
                //g.DrawLine(Pens.Purple, Vector3dToPoint(backLensPos), Vector3dToPoint(backLensPos + 20 * -complexLens.ElementSurfaces.First().SurfaceNormalField.GetNormal(backLensPos)));
            }

            // draw ray inside lens
            if (intersections != null)
            {
                for (int i = 0; i < intersections.Count - 1; i++)
                {
                    Point origin = Vector3dToPoint(intersections[i]);
                    Point target = Vector3dToPoint(intersections[i + 1]);
                    g.DrawLine(Pens.Brown, origin, target);
                }
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
            //vector = Vector3d.Transform(vector, Matrix4d.CreateRotationX(1.0));
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            inputLensPosDirectly = checkBox1.Checked;
            rayOriginXNumeric.Enabled = inputLensPosDirectly;
            rayOriginYNumeric.Enabled = inputLensPosDirectly;
            rayOriginZNumeric.Enabled = inputLensPosDirectly;
            lensPosTNumeric.Enabled = !inputLensPosDirectly;
            Recompute();
        }
    }
}
