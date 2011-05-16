namespace LightTracing
{
    using System;
    using System.Drawing;
    using BokehLab.FloatMap;
    using BokehLab.Lens;
    using BokehLab.Math;
    using OpenTK;

    public class LightTracer
    {
        public Vector3d LightSourcePosition { get; set; }

        public Vector3d SenzorCenter { get; set; }

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

        public ThinLens Lens { get; set; }

        public int SampleCount { get; set; }

        public float LightIntensity { get; set; }

        private FloatMapImage senzorFloatMap;

        public LightTracer()
        {
            LightSourcePosition = new Vector3d(0, 0, 20);
            SenzorCenter = new Vector3d(0, 0, -10);
            SensorSize = new Vector2d(4, 4);
            RasterSize = new Size(500, 500);
            SampleCount = 1000;
            Lens = new ThinLens(10, 1);
            LightIntensity = 0.5f;
            senzorFloatMap = new FloatMapImage((uint)RasterSize.Width, (uint)RasterSize.Height, PixelFormat.Greyscale);
        }

        // TODO:
        // - use a FloatMap as the senzor, then tone-map it to a Bitmap

        public Bitmap TraceLight()
        {
            for (int y = 0; y < senzorFloatMap.Height; y++)
            {
                for (int x = 0; x < senzorFloatMap.Width; x++)
                {
                    senzorFloatMap.Image[x, y, 0] = 0;
                }
            }

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
                // intersect the senzor with the outgoing ray
                double t = (SenzorCenter.Z - outgoingRay.Origin.Z) / outgoingRay.Direction.Z;
                Vector3d intersectionPoint = outgoingRay.Origin + t * outgoingRay.Direction;
                Vector2d intersectionPixelPoint = SenzorToRaster(intersectionPoint.Xy);
                // put a splat on the senzor at the intersection
                Splat(senzorFloatMap, LightIntensity, intersectionPixelPoint);
            }

            return senzorFloatMap.ToBitmap();
        }

        private void Splat(FloatMapImage senzor, float lightIntensity, Vector2d intersectionPixelPoint)
        {
            int x = (int)intersectionPixelPoint.X;
            int y = (int)intersectionPixelPoint.Y;
            if ((x >= 0) && (x < RasterSize.Width) && (y >= 0) && (y < RasterSize.Height))
            {
                senzor.Image[x, y, 0] += lightIntensity;
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
