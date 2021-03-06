﻿namespace BokehLab.ImageBasedRayCasting
{
    using System;
    using System.Drawing;
    using BokehLab.FloatMap;
    using BokehLab.Math;
    using BokehLab.RayTracing;
    using OpenTK;

    public class ImageLayer : IIntersectable
    {
        // For simplicity the scene is a rectangle with center at
        // Position (0,0,z) and normal (0,0,1) aligned with the
        // optical axis.
        //
        // The scene shading is precomputed and colors stored in
        // a float map.

        private Plane Plane { get; set; }

        private double depth;
        public double Depth
        {
            get { return depth; }
            set
            {
                depth = value;
                Plane.Origin = new Vector3d(Plane.Origin.X, Plane.Origin.Y, depth);
                UpdateWidth();
                UpdateObjectToWorld();
            }
        }

        // TODO: fix it to be the correct FOV angle, not just a magic constant
        private double fieldOfView;
        public double FieldOfView
        {
            get { return fieldOfView; }
            set
            {
                fieldOfView = value;
                UpdateWidth();
                UpdateObjectToWorld();
            }
        }

        private FloatMapImage image;
        public FloatMapImage Image
        {
            get { return image; }
            set
            {
                image = value;
                if (image != null)
                {
                    RasterSize = new Size((int)image.Width, (int)image.Height);
                }
                else
                {
                    RasterSize = new Size(1, 1);
                }
                UpdateObjectToWorld();
            }
        }

        private Size RasterSize { get; set; }

        /// <summary>
        /// Senzor width in camera space. Its height is determined by the
        /// aspect ratio.
        /// </summary>
        private double Width { get; set; }

        /// <summary>
        /// Senzor height in camera space. It is determined by the
        /// senzor width and aspect ratio.
        /// </summary>
        private double Height { get { return Width * AspectRatio; } }

        /// <summary>
        /// The ratio of height/width.
        /// </summary>
        private double AspectRatio
        {
            get
            {
                return (double)RasterSize.Height / (double)RasterSize.Width;
            }
        }

        private Matrix4d objectToWorld;
        /// <summary>
        /// Transforms from normalized senzor space to camera space.
        /// Contains information on senzor position and orientation.
        /// </summary>
        public Matrix4d ObjectToWorld
        {
            get { return objectToWorld; }
            set
            {
                objectToWorld = value;
                WorldToObject = Matrix4d.Invert(objectToWorld);
            }
        }

        /// <summary>
        /// Transforms from camera space to normalized senzor space.
        /// Inverse of SenzorToCamera.
        /// </summary>
        public Matrix4d WorldToObject { get; private set; }

        public ImageLayer()
        {
            objectToWorld = Matrix4d.Identity;
            WorldToObject = Matrix4d.Identity;
            Plane = new Plane()
            {
                Origin = new Vector3d(0, 0, -1),
                Normal = new Vector3d(0, 0, 1)
            };
            Depth = -20;
            FieldOfView = 0.5;
            RasterSize = new Size(1, 1);
        }

        #region IIntersectable Members

        public Intersection Intersect(Ray ray)
        {
            // intersect
            Intersection intersection = Plane.Intersect(ray);
            if (intersection == null)
            {
                return null;
            }
            Vector3d intersectionPos = intersection.Position;

            // compute 2D position in image coordinates
            Vector2d intPosImage = WorldToImage(intersectionPos);
            if ((intPosImage.X < 0) || (intPosImage.X >= RasterSize.Width) ||
                (intPosImage.Y < 0) || (intPosImage.Y >= RasterSize.Height))
            {
                return null;
            }

            // retrieve color
            // TODO: possibly to some bilinear interpolation
            int x = (int)intPosImage.X;
            int y = (int)intPosImage.Y;

            float[] color = new float[Image.ColorChannelsCount];
            for (int i = 0; i < Image.ColorChannelsCount; i++)
            {
                color[i] = Image.Image[x, y, i];
            }
            return new Intersection(intersectionPos, color);
        }

        #endregion

        private void UpdateObjectToWorld()
        {
            // - translate by (-0.5, -aspectRatio * 0.5)
            // - scale by (width, height, 1)
            // - translate by origin
            var matrix = Matrix4d.CreateTranslation(-0.5, AspectRatio * -0.5, 0);
            matrix = Matrix4d.Mult(matrix, Matrix4d.Scale(Width, Width, 1));
            matrix = Matrix4d.Mult(matrix, Matrix4d.CreateTranslation(Plane.Origin));
            ObjectToWorld = matrix;
        }

        private void UpdateWidth()
        {
            Width = FieldOfView * Math.Abs(Depth);
        }


        public Vector3d ImageToWorld(Vector2d imagePos)
        {
            Vector3d senzorPos3 = new Vector3d(RasterToNormalized(imagePos));
            return Vector3d.Transform(senzorPos3, ObjectToWorld);
        }

        public Vector2d WorldToImage(Vector3d cameraPos)
        {
            return NormalizedToRaster(Vector3d.Transform(cameraPos, WorldToObject).Xy);
        }

        /// <summary>
        /// Transforms from image raster space to normalized space.
        /// </summary>
        /// <param name="senzorPos"></param>
        /// <returns></returns>
        public Vector2d RasterToNormalized(Vector2d rasterPos)
        {
            return rasterPos / (double)RasterSize.Width;
        }

        public Vector2d NormalizedToRaster(Vector2d rasterPos)
        {
            return rasterPos * RasterSize.Width;
        }
    }
}
