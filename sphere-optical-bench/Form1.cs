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
            Bench = new SphericLens.OpticalBench();
            Bench.Sphere.Radius = 100.0;
            Bench.IncidentRay = new SphericLens.Ray(new SphericLens.Point(200, 20), new SphericLens.Vector(-20, 5));
            //this.KeyDown += new KeyEventHandler(pictureResult.KeyPressed);
            rayDirectionPhiNumericUpDown.Value = (decimal)(Bench.IncidentRay.Direction.Phi % (2*Math.PI));
            SphericLens.Vector originAsVector = SphericLens.Vector.FromPoint(Bench.IncidentRay.Origin);
            rayOriginPhiNumericUpDown.Value = (decimal)(originAsVector.Phi % (2*Math.PI));
            rayOriginRadiusNumericUpDown.Value = (decimal)originAsVector.Radius;
            sphereRadiusNumericUpDown.Value = (decimal)Bench.Sphere.Radius;
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
            DrawCircle(g, Pens.Blue, new Point(), (float)Bench.Sphere.Radius);

            SphericLens.Ray lastOutgoingRay = Bench.IncidentRay;

            foreach (SphericLens.OpticalBench.IntersectionResult intersectionResult in Bench.IntersectionResults)
            {
                lastOutgoingRay = intersectionResult.OutgoingRay;
                // draw a ray and its intersection      
                Point intersection = SphericPointToFormsPoint(intersectionResult.Intersection);
                Point origin = SphericPointToFormsPoint(intersectionResult.IncidentRay.Origin);
                g.DrawLine(Pens.Green, origin, intersection);

                // draw normal
                g.DrawLine(Pens.Brown, new Point(), intersection);

                FillSquare(g, Brushes.Red, intersection, 5);
            }
            
            g.DrawLine(Pens.Orange,
                        SphericPointToFormsPoint(lastOutgoingRay.Origin),
                        SphericPointToFormsPoint(lastOutgoingRay.Evaluate(100)));
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

        private void sphereRadiusNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Bench.Sphere.Radius = (double)sphereRadiusNumericUpDown.Value;
            updateBench();
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
    }
}
