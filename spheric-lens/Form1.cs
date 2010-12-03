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
      double circleRadius = 100.0;

    public Form1 ()
    {
        Bench = new SphericLens.OpticalBench();
        Bench.Direction = new SphericLens.Vector(0.0, circleRadius);
        InitializeComponent();
        //this.KeyDown += new KeyEventHandler(pictureResult.KeyPressed);
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

        //// draw X axis
        //g.DrawLine(Pens.Black, -panelHalfWidth, 0, panelHalfWidth, 0);

        //// draw Y axis
        //g.DrawLine(Pens.Black, 0, -panelHalfWidth, 0, panelHalfWidth);

        PaintLinearBench(g);
    }

    private void PaintLinearBench(Graphics g)
    {
        int radius = (int) circleRadius;
        DrawCircle(g, Pens.Blue, new Point(), radius);

        // draw the optical border
        g.DrawLine(new Pen(Color.Black, 4.0f), new Point(-radius, 0), new Point(radius, 0));


        double eta = Bench.RefractiveIndexAir / Bench.RefractiveIndexGlass;
        double criticalAngle = (eta > 1.0) ? Math.PI + Math.Asin(1 / eta) : Math.Asin(eta);
        SphericLens.Vector criticalAngleVector = new SphericLens.Vector() { Phi = criticalAngle, Radius = radius };
        Point criticalAnglePoint = SphericPointToFormsPoint(SphericLens.Point.FromVector(-criticalAngleVector));
        g.DrawLine(Pens.Red, new Point(), criticalAnglePoint);
        criticalAnglePoint.X *= -1;
        g.DrawLine(Pens.Red, new Point(), criticalAnglePoint);

        // draw the normal
        g.DrawLine(Pens.Brown, new Point(), new Point(0, (int)Bench.Normal.Radius));

        // draw the incomint ray
        g.DrawLine(Pens.Green, new Point(), SphericPointToFormsPoint(SphericLens.Point.FromVector(Bench.Direction)));

        // draw the refracted ray
        g.DrawLine(Pens.Blue, new Point(), SphericPointToFormsPoint(SphericLens.Point.FromVector(Bench.RefractedDirection)));
    }

      private Point SphericPointToFormsPoint(SphericLens.Point point) {
          return new Point((int)point.X, (int)point.Y);
      }

      private void DrawCircle(Graphics g, Pen pen, Point center, float radius) {
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
      private void rayDirectionPhiNumericUpDown_ValueChanged(object sender, EventArgs e)
      {
          Bench.Direction.Phi = (double)rayDirectionPhiNumericUpDown.Value;
          updateBench();
      }
  }
}
