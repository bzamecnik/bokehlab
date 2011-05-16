namespace BokehLab.ImageBasedRayCasting
{
    using System.Drawing;
    using BokehLab.FloatMap;
    using BokehLab.Math;
    using OpenTK;

    class RayTracer
    {
        public Scene Scene { get; set; }
        public Camera Camera { get; set; }

        public int SampleCount;

        public RayTracer()
        {
            Scene = new Scene();
            Camera = new Camera();
            SampleCount = 1;
        }

        public FloatMapImage RenderImage(Size imageSize) {
            int height = imageSize.Height;
            int width = imageSize.Width;
            FloatMapImage outputImage = new FloatMapImage((uint)width, (uint)height);

            Camera.Sensor.RasterSize = imageSize;

            float[] color = new float[outputImage.ColorChannelsCount];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int sampleIndex = 0; sampleIndex < SampleCount; sampleIndex++)
                    {
                        // generate a ray from the senzor and lens towards the scene
                        Vector2d imagePos = GenerateImageSample(new Point(x, y));
                        Ray outgoingRay = Camera.GenerateRay(imagePos);
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
                            color[i] += intersection.color[i];
                        }
                    }
                    for (int i = 0; i < outputImage.ColorChannelsCount; i++)
                    {
                        outputImage.Image[x, y, i] = color[i] / (float)SampleCount;
                        color[i] = 0;
                    }
                }
            }
            return outputImage;
        }

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
