﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BokehLab.Lens;
using OpenTK;

namespace BokehLab.ImageBasedRayCasting
{
    class Camera
    {
        public Sensor Sensor { get; set; }

        public ThinLens Lens { get; set; }

        private Matrix4d cameraToWorld;
        /// <summary>
        /// Camera-to-world transformation. Contains information on camera
        /// position and orientation.
        /// </summary>
        public Matrix4d CameraToWorld
        {
            get
            {
                return cameraToWorld;
            }
            set
            {
                cameraToWorld = value;
                WorldToCamera = Matrix4d.Invert(cameraToWorld);
            }
        }

        /// <summary>
        /// World-to-camera transformation. Inverse of CameraToWorld.
        /// </summary>
        public Matrix4d WorldToCamera { get; private set; }

        public Camera()
        {
            cameraToWorld = Matrix4d.Identity;
            WorldToCamera = Matrix4d.Identity;
            Sensor = new Sensor();
            Lens = new ThinLens();
        }

        /// <summary>
        /// Generates a ray going from camera lens towards the scene
        /// (in camera space).
        /// </summary>
        /// <param name="senzorPoint">Position on output image (in image raster
        /// space)</param>
        /// <returns>Outgoint ray or null if the ray is absorbed inside lens.
        /// </returns>
        public Ray GenerateRay(Vector2d imagePos)
        {
            // for a position on the sensor generate a position on the lens

            // Generates a ray from senzor to lens back pupil given position on
            // image which corresponds to the senzor position.

            Vector3d senzorPos = Sensor.ImageToCamera(imagePos);
            Vector3d lensPos = LensToCamera(GenerateLensPosition());
            Ray outgoingRay = Lens.Transfer(senzorPos, lensPos);
            return outgoingRay;
        }

        private Vector3d LensToCamera(Vector3d lensPos)
        {
            // NOTE: a more complex transform can be there
            return lensPos;
        }

        private Vector3d GenerateLensPosition()
        {
            return new Vector3d(Lens.GenerateLensPositionSample());
        }
    }
}