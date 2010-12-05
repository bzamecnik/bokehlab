using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SphericLensGUI
{
    public partial class Form1 : Form
    {
        public SphericLens.OpticalBench Bench;

        public Form1()
        {
            InitializeComponent();
            Bench = PrepareTestingOpticalBench();
            //Bench.Elements.Convex = false;
            Bench.IncidentRay = new SphericLens.Ray(new SphericLens.Point(200, 20), new SphericLens.Vector(-20, 5));
            //this.KeyDown += new KeyEventHandler(pictureResult.KeyPressed);
            rayDirectionPhiNumericUpDown.Value = (decimal)(Bench.IncidentRay.Direction.Phi % (2*Math.PI));
            SphericLens.Vector originAsVector = SphericLens.Vector.FromPoint(Bench.IncidentRay.Origin);
            rayOriginPhiNumericUpDown.Value = (decimal)originAsVector.Phi;
            rayOriginRadiusNumericUpDown.Value = (decimal)originAsVector.Radius;
            sphericalCapRadiusNumericUpDown.Value = (decimal)Bench.Elements[0].Radius;
            sphericalCapApertureNumericUpDown.Value = (decimal)Bench.Elements[0].Aperture;
            sphericalCapConvexCheckBox.Checked = Bench.Elements[0].Convex;
        }

        private SphericLens.OpticalBench PrepareTestingOpticalBench()
        {
            SphericLens.OpticalBench bench = new SphericLens.OpticalBench();
            // bench.LensCenter = -100.0;
            bench.Elements.Add(
                new SphericLens.SphericalCap()
                {
                    Convex = true,
                    Radius = 150.0,
                    Aperture = 120.0,
                    NextRefractiveIndex = SphericLens.RefractiveIndices.CROWN_GLASS,
                    DistanceToNext = 50.0
                }
            );
            bench.Elements.Add(
                new SphericLens.SphericalCap()
                {
                    Convex = true,
                    Radius = 100.0,
                    Aperture = 80.0,
                    NextRefractiveIndex = SphericLens.RefractiveIndices.AIR,
                    DistanceToNext = 50.0
                }
            );
            bench.Elements.Add(
                new SphericLens.SphericalCap()
                {
                    Convex = false,
                    Radius = 100.0,
                    Aperture = 80.0,
                    NextRefractiveIndex = SphericLens.RefractiveIndices.CROWN_GLASS,
                    DistanceToNext = 50.0
                }
            );
            bench.Elements.Add(
                new SphericLens.SphericalCap()
                {
                    Convex = false,
                    Radius = 150.0,
                    Aperture = 120.0,
                    NextRefractiveIndex = SphericLens.RefractiveIndices.AIR,
                    DistanceToNext = 50.0
                }
            );
            return bench;
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

            PaintSphericBench(g);
        }

        private void PaintSphericBench(Graphics g)
        {
            // draw a circlular lens
            //DrawCircle(g, Pens.Blue, new Point(), (float)Bench.Elements.Radius);

            g.TranslateTransform((float)-Bench.LensCenter, 0.0f);
            double translation = 0.0;
            foreach (SphericLens.SphericalCap element in Bench.Elements)
            {
                double signedRadius = (element.Convex ? 1.0 : -1.0) * element.Radius;
                g.TranslateTransform((float)-(translation + signedRadius), 0.0f);
                DrawSphericalCap(g, element);
                g.TranslateTransform((float)(translation + signedRadius), 0.0f);
                translation += element.DistanceToNext;
            }
            g.TranslateTransform((float)Bench.LensCenter, 0.0f);

            SphericLens.Ray lastOutgoingRay = Bench.IncidentRay;

            foreach (SphericLens.OpticalBench.IntersectionResult intersectionResult in Bench.IntersectionResults)
            {
                lastOutgoingRay = intersectionResult.OutgoingRay;
                // draw a ray and its intersection      
                Point intersection = SphericPointToFormsPoint(intersectionResult.Intersection);
                Point origin = SphericPointToFormsPoint(intersectionResult.IncidentRay.Origin);
                g.DrawLine(Pens.Green, origin, intersection);

                // draw normal
                SphericLens.Vector normal = intersectionResult.Normal.Normalize() * 20.0;
                g.DrawLine(Pens.Brown, intersection, SphericPointToFormsPoint(intersectionResult.Intersection + normal));

                FillSquare(g, Brushes.Red, intersection, 3);
            }
            
            g.DrawLine(Pens.Orange,
                        SphericPointToFormsPoint(lastOutgoingRay.Origin),
                        SphericPointToFormsPoint(lastOutgoingRay.Evaluate(100)));
        }

        private void DrawSphericalCap(Graphics g, SphericLens.SphericalCap cap)
        {
            float angle = (float)(cap.Angle * 180.0 / Math.PI);
            float startAngle = (cap.Convex ? 360.0f : 180.0f) - angle;
            float sweepAngle = 2.0f * angle;
            g.DrawArc(Pens.Red, (float)-cap.Radius, (float)-cap.Radius, (float)(2 * cap.Radius), (float)(2 * cap.Radius), startAngle, sweepAngle);
            float x = (float)((cap.Convex ? cap.Radius : -cap.Radius) - (cap.Convex ? cap.Thickness : -cap.Thickness));
            float y = (float)cap.Aperture;
            g.DrawLine(Pens.Black, x, -y, x, y);
        }

        private Point SphericPointToFormsPoint(SphericLens.Point point)
        {
            return new Point((int)point.X, (int)point.Y);
        }

        private void DrawCircle(Graphics g, Pen pen, Point center, float radius)
        {
            g.DrawEllipse(pen, center.X - radius, center.Y - radius, 2 * radius, 2 * radius);
        }

        private void FillSquare(Graphics g, Brush brush, Point center, float radius)
        {
            g.FillRectangle(brush, center.X - radius, center.Y - radius, 2 * radius, 2 * radius);
        }

        private void updateBench()
        {
            Bench.Update();
            drawingPanel.Invalidate();
        }

        private void sphericalCapRadiusNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            double radius = (double)sphericalCapRadiusNumericUpDown.Value;
            if (radius >= Bench.Elements[0].Aperture)
            {
                Bench.Elements[0].Radius = radius;
                updateBench();
            }
            else
            {
                sphericalCapRadiusNumericUpDown.Value = (decimal)Bench.Elements[0].Aperture;
            }
        }

        private void sphericalCapApertureNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            double aperture = (double)sphericalCapApertureNumericUpDown.Value;
            if (aperture <= Bench.Elements[0].Radius)
            {
                Bench.Elements[0].Aperture = Math.Abs(aperture);
                updateBench();
            }
            else
            {
                sphericalCapApertureNumericUpDown.Value = (decimal)Bench.Elements[0].Radius;
            }
        }

        private void rayOriginRadiusNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            SphericLens.Vector originAsVector = SphericLens.Vector.FromPoint(Bench.IncidentRay.Origin);
            originAsVector.Radius = (double)rayOriginRadiusNumericUpDown.Value;
            Bench.IncidentRay.Origin.X = originAsVector.X;
            Bench.IncidentRay.Origin.Y = originAsVector.Y;
            updateBench();
        }

        private void rayOriginPhiNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            SphericLens.Vector originAsVector = SphericLens.Vector.FromPoint(Bench.IncidentRay.Origin);
            originAsVector.Phi = (double)rayOriginPhiNumericUpDown.Value;
            Bench.IncidentRay.Origin.X = originAsVector.X;
            Bench.IncidentRay.Origin.Y = originAsVector.Y;
            updateBench();
        }

        private void rayDirectionPhiNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Bench.IncidentRay.Direction.Phi = (double)rayDirectionPhiNumericUpDown.Value;
            updateBench();
        }

        private void sphericalCapConvexCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Bench.Elements[0].Convex = sphericalCapConvexCheckBox.Checked;
            updateBench();
        }
    }
}
