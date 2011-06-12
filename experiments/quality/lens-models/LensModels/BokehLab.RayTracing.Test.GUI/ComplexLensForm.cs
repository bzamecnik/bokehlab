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
        private LensRayTransferFunction lrtf;
        private LensRayTransferFunction.Table3d lrtfTable;
        private Ray incomingRay;
        private Ray outgoingRay;
        private Vector3d backLensPos;
        private IList<Vector3d> intersections;
        double directionPhi;

        // directly provide the position on lens back surface or derive it by
        // intersecting the incoming ray with the lens
        bool inputLensPosDirectly = false;

        bool initialized = false;

        public ComplexLensForm()
        {
            InitializeComponent();
            complexLens = CreateLens();
            PrepareLrtf(complexLens, 128);
            //directionPhi = Math.PI;
            directionPhi = 1.0;
            incomingRay = new Ray(new Vector3d(25, 0, 300), new Vector3d(Math.Sin(directionPhi), 0, Math.Cos(directionPhi)));

            rayDirectionPhiNumeric.Value = (decimal)directionPhi;
            FillVectorToControls(incomingRay.Origin, rayOriginXNumeric, rayOriginYNumeric, rayOriginZNumeric);
            initialized = true;
            Recompute();
        }

        private void PrepareLrtf(ComplexLens lens, int sampleCount)
        {
            lrtf = new LensRayTransferFunction(lens);
            // load precomputed LRTF from a file or compute it and save to file
            string filename = string.Format(@"..\..\..\lrtf_double_gauss_{0}.bin", sampleCount);
            lrtfTable = lrtf.SampleLrtf3DCached(sampleCount, filename);
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
                //// compute lens position from lens position parameter
                //// (with Y = 0)
                //double lensPosV = 0.5;
                //lensPosU = (double)lensPosTNumeric.Value;
                //if (lensPosU > 1.0)
                //{
                //    lensPosU = 2.0 - lensPosU;
                //    lensPosV = 0.0;
                //}
                //incomingRay = complexLens.ConvertParametersToBackSurfaceRay(
                //    new LensRayTransferFunction.Parameters(
                //        lensPosU, lensPosV, directionPhi, 0));
                ////Console.WriteLine("IN: {0}", incomingRay);
                var incomingParams = GetIncomingParams();
                incomingRay = complexLens.ConvertParametersToBackSurfaceRay(incomingParams);
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
            drawingYProjectionPanel.Invalidate();
            drawingXProjectionPanel.Invalidate();
            drawingZProjectionPanel.Invalidate();
        }

        private LensRayTransferFunction.Parameters GetIncomingParams()
        {
            double posTheta = (double)positionThetaNumeric.Value;
            double posPhi = (double)positionPhiNumeric.Value;
            double dirTheta = (double)directionThetaNumeric.Value;
            double dirPhi = (double)directionPhiNumeric.Value;
            return new LensRayTransferFunction.Parameters(
                posTheta, posPhi, dirTheta, dirPhi);
        }

        private void drawingYProjectionPanel_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.TranslateTransform(-e.ClipRectangle.Width * 0.25f, 0);
            PrepareDrawingPanel(e);

            PaintScene(g, Vector3dToPointY);
        }


        private void drawingXProjectionPanel_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.TranslateTransform(-e.ClipRectangle.Width * 0.25f, 0);
            PrepareDrawingPanel(e);

            PaintScene(g, Vector3dToPointX);
        }

        private void drawingZProjectionPanel_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            PrepareDrawingPanel(e);

            ComplexLens.ElementSurface backSurface = complexLens.ElementSurfaces.First();
            float radius = (float)backSurface.ApertureRadius;

            DrawCircle(g, Pens.Blue, new Point(), radius);

            if (incomingRay.Direction != Vector3d.Zero)
            {
                Point origin = Vector3dToPointZ(incomingRay.Origin);
                Point target = Vector3dToPointZ(incomingRay.Origin - 100 * Vector3d.Normalize(incomingRay.Direction));
                g.DrawLine(Pens.Green, origin, target);
            }

            if (backLensPos != Vector3d.Zero)
            {
                Vector3d normalLocal = backSurface.SurfaceNormalField.GetNormal(backLensPos);
                Point origin = Vector3dToPointZ(backLensPos);
                Point target = Vector3dToPointZ(backLensPos + 100 * Vector3d.Normalize(normalLocal));
                g.DrawLine(Pens.Crimson, origin, target);

                FillSquare(g, Brushes.Red, Vector3dToPointZ(backLensPos), 3);
            }
        }

        private static Graphics PrepareDrawingPanel(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            float panelHalfWidth = e.ClipRectangle.Width * 0.5f;
            float panelHalfHeight = e.ClipRectangle.Height * 0.5f;
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
            return g;
        }

        private void PaintScene(Graphics g, Func<Vector3d, Point> projFunc)
        {
            DrawComplexLens(g, projFunc, complexLens);
            DrawRays(g, projFunc);
        }

        private void DrawRays(Graphics g, Func<Vector3d, Point> projFunc)
        {
            // draw incoming ray
            if (incomingRay.Direction != Vector3d.Zero)
            {
                Point origin = projFunc(incomingRay.Origin);
                Point target;
                if (inputLensPosDirectly)
                {
                    if (outgoingRay != null)
                    {
                        target = projFunc(backLensPos);
                    }
                    else
                    {
                        target = projFunc(incomingRay.Origin + 1000 * Vector3d.Normalize(incomingRay.Direction));
                    }
                }
                else
                {
                    target = projFunc(incomingRay.Origin - 100 * Vector3d.Normalize(incomingRay.Direction));

                }
                g.DrawLine(Pens.Green, origin, target);

                //var parameters = complexLens.ConvertBackSurfaceRayToParameters(incomingRay);
                //var recoveredRay = complexLens.ConvertParametersToBackSurfaceRay(parameters);
                //if (recoveredRay.Direction != Vector3d.Zero)
                //{
                //    origin = projFunc(recoveredRay.Origin);
                //    target = projFunc(recoveredRay.Origin + 1000 * Vector3d.Normalize(recoveredRay.Direction));
                //    //g.DrawLine(Pens.Orange, origin, target);
                //}
            }

            if (backLensPos != Vector3d.Zero)
            {
                FillSquare(g, Brushes.Red, projFunc(backLensPos), 3);
            }

            // draw outgoing ray
            if ((outgoingRay != null) && (outgoingRay.Direction != Vector3d.Zero))
            {
                Point origin = projFunc(outgoingRay.Origin);
                Point target = projFunc(outgoingRay.Origin + 1000 * Vector3d.Normalize(outgoingRay.Direction));
                g.DrawLine(Pens.Brown, origin, target);
                //g.DrawLine(Pens.DarkGreen, projFunc(backLensPos), origin);

                //// draw normal
                //g.DrawLine(Pens.Purple, origin, projFunc(outgoingRay.Origin + 20 * -complexLens.ElementSurfaces.Last().SurfaceNormalField.GetNormal(outgoingRay.Origin)));

                //// draw normal
                //g.DrawLine(Pens.Purple, projFunc(backLensPos), projFunc(backLensPos + 20 * -complexLens.ElementSurfaces.First().SurfaceNormalField.GetNormal(backLensPos)));

                // test ray->parameters->ray:
                //var parameters = complexLens.ConvertFrontSurfaceRayToParameters(outgoingRay);
                //var recoveredRay = complexLens.ConvertParametersToFrontSurfaceRay(parameters);
                //if (recoveredRay.Direction != Vector3d.Zero)
                //{
                //    origin = projFunc(recoveredRay.Origin);
                //    target = projFunc(recoveredRay.Origin + 1000 * Vector3d.Normalize(recoveredRay.Direction));
                //    g.DrawLine(Pens.Orange, origin, target);
                //}

                // test LRTF without rotation:

                //var outgoingParams = lrtf.ComputeLrtf(GetIncomingParams());
                //var outgoingRayFromLrtf = complexLens.ConvertParametersToFrontSurfaceRay(outgoingParams);
                //if (outgoingRayFromLrtf.Direction != Vector3d.Zero)
                //{
                //    origin = projFunc(outgoingRayFromLrtf.Origin);
                //    target = projFunc(outgoingRayFromLrtf.Origin + 1000 * Vector3d.Normalize(outgoingRayFromLrtf.Direction));
                //    g.DrawLine(Pens.Orange, origin, target);
                //}

                var incomingParams = GetIncomingParams();

                // test LRTF with rotation:
                //var positionPhi = incomingParams.PositionPhi;
                //incomingParams.PositionPhi = 0;

                //var outgoingParams = lrtf.ComputeLrtf(incomingParams);

                //outgoingParams.PositionPhi += positionPhi;

                // test LRTF evaluated from a precomputed table
                var outgoingParams = lrtfTable.EvaluateLrtf3D(incomingParams);

                var outgoingRayFromLrtf = complexLens.ConvertParametersToFrontSurfaceRay(outgoingParams);
                if (outgoingRayFromLrtf.Direction != Vector3d.Zero)
                {
                    origin = projFunc(outgoingRayFromLrtf.Origin);
                    target = projFunc(outgoingRayFromLrtf.Origin + 1000 * Vector3d.Normalize(outgoingRayFromLrtf.Direction));
                    g.DrawLine(Pens.Orange, origin, target);
                }
            }

            // draw ray inside lens
            if (intersections != null)
            {
                for (int i = 0; i < intersections.Count - 1; i++)
                {
                    Point origin = projFunc(intersections[i]);
                    Point target = projFunc(intersections[i + 1]);
                    g.DrawLine(Pens.Brown, origin, target);
                }
            }
        }

        private void DrawComplexLens(Graphics g, Func<Vector3d, Point> projFunc, ComplexLens lens)
        {
            float maxApertureRadius = (float)lens.ElementSurfaces.Select(
                surface => surface.ApertureRadius).Max();
            // draw spheres
            foreach (var surface in lens.ElementSurfaces)
            {
                if (surface.Surface is Sphere)
                {
                    Sphere sphere = (Sphere)surface.Surface;
                    //DrawCircle(g, Pens.Blue, projFunc(sphere.Center), (float)sphere.Radius);
                    //FillSquare(g, Brushes.Aquamarine, projFunc(sphere.Center), 3);
                    DrawSphericalCap(g, sphere, surface.ApertureRadius, surface.Convex);
                }
                else if (surface.Surface is Circle)
                {
                    Circle circle = (Circle)surface.Surface;
                    DrawCircularStop(g, Pens.Violet, projFunc(
                        new Vector3d(0, 0, circle.Z)), (float)circle.Radius, maxApertureRadius);
                }
            }
        }

        private Point Vector3dToPointX(Vector3d vector)
        {
            return new Point((int)vector.Z, (int)vector.Y);
        }

        private Point Vector3dToPointY(Vector3d vector)
        {
            return new Point((int)vector.Z, (int)vector.X);
        }

        private Point Vector3dToPointZ(Vector3d vector)
        {
            return new Point((int)vector.X, (int)vector.Y);
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
