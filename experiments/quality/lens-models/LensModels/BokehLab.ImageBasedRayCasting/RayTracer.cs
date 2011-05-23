namespace BokehLab.ImageBasedRayCasting
{
    using System;
    using System.Drawing;
    using BokehLab.FloatMap;
    using BokehLab.Math;
    using BokehLab.RayTracing;
    using OpenTK;

    public class RayTracer : IRenderer
    {
        public Scene Scene { get; set; }
        public Camera Camera { get; set; }

        public RayTracer()
        {
            Scene = new Scene();
            Camera = new Camera();
            SampleCount = 1;
        }

        #region IRenderer Members

        public int SampleCount { get; set; }

        public FloatMapImage RenderImage(Size imageSize)
        {
            return RenderImagePreview(imageSize, new Rectangle(Point.Empty, imageSize), null);
        }

        public FloatMapImage RenderImagePreview(Size imageSize, Rectangle preview, FloatMapImage outputImage)
        {
            int height = imageSize.Height;
            int width = imageSize.Width;
            if ((outputImage == null) ||
                (outputImage.Width != width) ||
                (outputImage.Height != height))
            {
                outputImage = new FloatMapImage((uint)width, (uint)height);
            }

            Camera.Sensor.RasterSize = imageSize;

            Sampler sampler = new Sampler();
            int sqrtSampleCount = (int)Math.Sqrt(SampleCount);
            int totalSampleCount = sqrtSampleCount * sqrtSampleCount;

            // NOTE: It is not useful to directly use the same lens samples
            // for each pixel in the whole image. This leads to ugly artifacts
            // and surprisigly there is only an insignificant performance
            // benefit.

            int minX = preview.Left;
            int maxX = preview.Right;
            int minY = preview.Top;
            int maxY = preview.Bottom;
            float[] color = new float[outputImage.ColorChannelsCount];
            for (int y = minY; y < maxY; y++)
            {
                for (int x = minX; x < maxX; x++)
                {
                    foreach (Vector2d sample in sampler.GenerateJitteredSamples(sqrtSampleCount))
                    {
                        // generate a ray from the senzor and lens towards the scene
                        Vector2d imagePos = GenerateImageSample(new Point(x, y));
                        Ray outgoingRay = Camera.GenerateRay(imagePos, sample);
                        if (outgoingRay == null)
                        {
                            continue;
                        }
                        Ray outgoingRayWorld = outgoingRay.Transform(Camera.CameraToWorld);

                        // intersect the scene

                        Intersection intersection = Scene.Intersect(outgoingRayWorld);

                        if (intersection == null)
                        {
                            continue;
                        }

                        // get the color stored in the scene point

                        for (int i = 0; i < outputImage.ColorChannelsCount; i++)
                        {
                            color[i] += intersection.Color[i];
                        }
                    }
                    for (int i = 0; i < outputImage.ColorChannelsCount; i++)
                    {
                        outputImage.Image[x, y, i] = color[i] / (float)totalSampleCount;
                        color[i] = 0;
                    }
                }
            }
            return outputImage;
        }

        #endregion

        /// <summary>
        /// Generates a sample within the pixel rectangle
        /// [X; X + 1) x [Y; Y + 1) (in image raster coordinates).
        /// </summary>
        /// <param name="pixel"></param>
        /// <returns></returns>
        private Vector2d GenerateImageSample(Point pixel)
        {
            // TODO: there should be some [psudo-]random sampling
            // (probably with jittering).
            return new Vector2d(pixel.X + 0.5, pixel.Y + 0.5);
        }
    }
}
