using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK;
using BokehLab.Lens;

namespace LightTracing
{
    public class LightTracer
    {
        Vector3d LightSourcePosition { get; set; }

        Vector3d SenzorCenter { get; set; }

        /// <summary>
        /// Senzor spans from [-SensorSize.X/2; -SensorSize.Y/2] to
        /// [SensorSize.X/2; SensorSize.Y/2]. [0;0] is at the center
        /// corresponding to SenzorPosition.
        /// </summary>
        Vector2d SensorSize { get; set; }

        /// <summary>
        /// Raster spans from [0; 0] to [RasterSize.X; Raster.Y].
        /// Left to right, top to bottom.
        /// </summary>
        Size RasterSize { get; set; }

        ThinLens Lens { get; set; }

        int SampleCount { get; set; }

        float LightIntensity { get; set; }

        public LightTracer()
        {
            LightSourcePosition = new Vector3d(0, 0, 20);
            SenzorCenter = new Vector3d(0, 0, -10);
            SensorSize = new Vector2d(4, 4);
            RasterSize = new Size(500, 500);
            SampleCount = 1000;
            Lens = new ThinLens(10, 1);
            LightIntensity = 1;
        }

        // TODO:
        // - use a FloatMap as the senzor, then tone-map it to a Bitmap

        public Bitmap TraceLight()
        {
            Bitmap senzor = new Bitmap(RasterSize.Width, RasterSize.Height);
            using (Graphics g = Graphics.FromImage(senzor))
            {
                g.FillRectangle(Brushes.Black, 0, 0, RasterSize.Width, RasterSize.Height);
            }

            int pixelIntensity = (int)(255 * LightIntensity);
            Color lightColor = Color.FromArgb(pixelIntensity, pixelIntensity, pixelIntensity);

            for (int i = 0; i < SampleCount; i++)
            {
                // generate a sample at the lens surface
                Vector2d lensPosCameraSpace = Lens.GenerateLensPositionSample();
                Vector3d lensPos = new Vector3d(lensPosCameraSpace);
                lensPos.Z = 0;
                // make an incoming ray from the light source to the lens sample and
                // transfer the incoming ray through the lens creating the outgoing ray
                Ray outgoingRay = Lens.Transfer(LightSourcePosition, lensPos);
                // intersect the senzor with the outgoing ray
                double t = (SenzorCenter.Z - outgoingRay.Origin.Z) / outgoingRay.Direction.Z;
                Vector3d intersectionPoint = outgoingRay.Origin + t * outgoingRay.Direction;
                Vector2d intersectionPixelPoint = SenzorToRaster(intersectionPoint.Xy);
                // put a splat on the senzor at the intersection
                Splat(senzor, lightColor, intersectionPixelPoint);
            }

            return senzor;
        }

        private void Splat(Bitmap senzor, Color lightColor, Vector2d intersectionPixelPoint)
        {
            int x = (int)intersectionPixelPoint.X;
            int y = (int)intersectionPixelPoint.Y;
            if ((x >= 0) && (x < RasterSize.Width) && (y >= 0) && (y < RasterSize.Height))
            {
                senzor.SetPixel(x, y, lightColor);
            }
        }

        /// <summary>
        /// Transform from position in senzor space to raster space.
        /// </summary>
        /// <param name="senzorPos"></param>
        /// <returns></returns>
        private Vector2d SenzorToRaster(Vector2d senzorPos)
        {
            return new Vector2d(
                (senzorPos.X / (1 * SensorSize.X) + 0.5) * RasterSize.Width,
                (1 - (senzorPos.Y / (1 * SensorSize.Y) + 0.5)) * RasterSize.Height
                );
        }
    }
}
