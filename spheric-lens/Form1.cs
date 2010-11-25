﻿using System;
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

    public Form1 ()
    {
      Bench = new SphericLens.OpticalBench();
      Bench.Sphere.Radius = 100.0;
      Bench.Ray = new SphericLens.Ray(new SphericLens.Point(-20, 0), new SphericLens.Vector(10, 5));
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

        // draw X axis
        g.DrawLine(Pens.Black, -panelHalfWidth, 0, panelHalfWidth, 0);
        
        // draw Y axis
        g.DrawLine(Pens.Black, 0, -panelHalfWidth, 0, panelHalfWidth);

        // draw a circlular lens
        DrawCircle(g, Pens.Blue, new Point(), (float)Bench.Sphere.Radius);

        // draw a ray and possibly its intersection
        if (Bench.RayIntersects)
        {
            Point intersection = SphericPointToFormsPoint(Bench.Intersection);
            g.DrawLine(Pens.Green, SphericPointToFormsPoint(Bench.Ray.Origin), intersection);

            // draw normal
            g.DrawLine(Pens.Brown, new Point(), SphericPointToFormsPoint(Bench.Intersection));

            // draw refracted ray
            g.DrawLine(Pens.Yellow,
                SphericPointToFormsPoint(Bench.RefractedRay.Origin),
                SphericPointToFormsPoint(Bench.RefractedRay.Evaluate(100)));

            FillSquare(g, Brushes.Red, intersection, 5);
        }
        else
        {
            g.DrawLine(Pens.Orange,
                SphericPointToFormsPoint(Bench.Ray.Origin),
                SphericPointToFormsPoint(Bench.Ray.Evaluate(100)));
        }
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

      private void sphereRadiusNumericUpDown_ValueChanged(object sender, EventArgs e)
      {
          Bench.Sphere.Radius = (double)sphereRadiusNumericUpDown.Value;
          updateBench();
      }

      private void rayOriginXNumericUpDown_ValueChanged(object sender, EventArgs e)
      {
          Bench.Ray.Origin.X = (double)rayOriginXNumericUpDown.Value;
          updateBench();
      }

      private void rayOriginYNumericUpDown_ValueChanged(object sender, EventArgs e)
      {
          Bench.Ray.Origin.Y = (double)rayOriginYNumericUpDown.Value;
          updateBench();
      }

      private void rayDirectionXNumericUpDown_ValueChanged(object sender, EventArgs e)
      {
          Bench.Ray.Direction.X = (double)rayDirectionXNumericUpDown.Value;
          updateBench();
      }

      private void rayDirectionYNumericUpDown_ValueChanged(object sender, EventArgs e)
      {
          Bench.Ray.Direction.Y = (double)rayDirectionYNumericUpDown.Value;
          updateBench();
      }
  }
}
