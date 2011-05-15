﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using BokehLab.FloatMap;
using BokehLab.Lens;
using OpenTK;

namespace BokehLab.ImageBasedRayCasting
{
    class RayTracer
    {
        public Scene Scene { get; set; }
        public Camera Camera { get; set; }

        public RayTracer()
        {
            Scene = new Scene();
            Camera = new Camera();
        }

        public FloatMapImage RenderImage(Size imageSize) {
            int height = imageSize.Height;
            int width = imageSize.Width;
            FloatMapImage outputImage = new FloatMapImage((uint)width, (uint)height);

            Camera.Sensor.RasterSize = imageSize;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
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
                        outputImage.Image[x, y, i] = intersection.color[i];
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
