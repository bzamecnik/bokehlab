namespace LightTracing
{
    using System;
    using System.Drawing;
    using BokehLab.FloatMap;
    using BokehLab.RayTracing.Lens;
    using BokehLab.Math;
    using BokehLab.RayTracing;
    using OpenTK;

    public class LightTracer : IRenderer
    {
        public Vector3d LightSourcePosition { get; set; }

        public Sensor Sensor { get; set; }

        public ThinLens ThinLens { get; set; }
        public ILens Lens { get; set; }

        public float LightIntensity { get; set; }

        public LightTracer()
        {
            LightSourcePosition = new Vector3d(0, 0, -20);
            Sensor = new Sensor()
            {
                Shift = new Vector3d(0, 0, 10),
                Width = 4,
                Tilt = new Vector3d(0, 0, 0)
            };
            SampleCount = 1000;
            ThinLens = new ThinLens(10, 1);
            //Lens = ThinLens;
            LensWithTwoStops vignettingLens = new LensWithTwoStops() { Lens = ThinLens };
            vignettingLens.BackStop.Z = -1;
            vignettingLens.FrontStop.Z = 1;
            Lens = vignettingLens;
            LightIntensity = 0.5f;
        }


        #region IRenderer Members

        public int SampleCount { get; set; }

        public FloatMapImage RenderImage(Size imageSize)
        {
            int height = imageSize.Height;
            int width = imageSize.Width;
            FloatMapImage outputImage = new FloatMapImage((uint)width, (uint)height, PixelFormat.Greyscale);
            Sensor.RasterSize = imageSize;

            Sampler sampler = new Sampler();
            int SqrtSampleCount = (int)Math.Sqrt(SampleCount);
            foreach (Vector2d sample in sampler.GenerateJitteredSamples(SqrtSampleCount))
            {
                // generate a sample at the lens surface
                Vector3d lensPos = Lens.GetBackSurfaceSample(sample);
                lensPos.Z = 0;
                // make an incoming ray from the light source to the lens sample and
                // transfer the incoming ray through the lens creating the outgoing ray
                Ray outgoingRay = Lens.Transfer(LightSourcePosition, lensPos);
                if (outgoingRay == null)
                {
                    continue;
                }
                // intersect the senzor with the outgoing ray
                Intersection intersection = Sensor.Intersect(outgoingRay);
                if (intersection == null)
                {
                    continue;
                }
                Vector3d intersectionPoint = intersection.Position;
                Vector2d intersectionPixelPoint = Sensor.CameraToImage(intersectionPoint);
                // put a splat on the senzor at the intersection
                Splat(outputImage, LightIntensity, intersectionPixelPoint);
            }

            return outputImage;
        }

        #endregion

        private void Splat(FloatMapImage senzor, float lightIntensity, Vector2d intersectionPixelPoint)
        {
            int x = (int)intersectionPixelPoint.X;
            int y = (int)intersectionPixelPoint.Y;
            if ((x >= 0) && (x < senzor.Width) && (y >= 0) && (y < senzor.Height))
            {
                senzor.Image[x, y, 0] += lightIntensity;
            }
        }
    }
}
