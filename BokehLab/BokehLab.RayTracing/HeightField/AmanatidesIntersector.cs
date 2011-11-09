namespace BokehLab.RayTracing.HeightField
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using OpenTK;
    using BokehLab.Math;

    /// <summary>
    /// "A Fast Voxel Traversal Algorithm for Ray Tracing", John Amanatides and Andrew Woo
    /// [amanatides1999]
    /// </summary>
    class AmanatidesIntersector : AbstractIntersector
    {
        private float epsilonForRayDir;
        public float EpsilonForRayDir { get { return epsilonForRayDir; } set { epsilonForRayDir = value; } }

        // If the ray goes through a hole in subsequent pixels which is
        // tighter in depth that this epsilon we can consider it to be an
        // intersection.
        private float epsilonForClosePixelDepth;
        public float EpsilonForClosePixelDepth { get { return epsilonForClosePixelDepth; } set { epsilonForClosePixelDepth = value; } }

        public AmanatidesIntersector(HeightField heightField)
            : base(heightField)
        {
            this.epsilonForRayDir = 0.001f;
            this.epsilonForClosePixelDepth = 0.05f;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        internal override Intersection Intersect(
            Vector3 start, Vector3 end,
            ref FootprintDebugInfo debugInfo)
        {
            bool collectDebugInfo = debugInfo != null;

            #region initialization
            Vector2 rayStart = start.Xy;
            Vector2 rayEnd = end.Xy;
            Vector2 rayDir = rayEnd - rayStart;

            // make sure the denominator in tMax and tDelta is not zero

            if (Math.Abs(rayDir.X) < epsilonForRayDir)
            {
                rayDir.X = epsilonForRayDir;
            }
            if (Math.Abs(rayDir.Y) < epsilonForRayDir)
            {
                rayDir.Y = epsilonForRayDir;
            }

            Vector2 step = new Vector2(Math.Sign(rayDir.X), Math.Sign(rayDir.Y));

            Vector2 currentPixel = new Vector2((float)Math.Floor(start.X), (float)Math.Floor(start.Y));
            Vector2 endPixel = new Vector2((float)Math.Floor(end.X), (float)Math.Floor(end.Y));

            Vector2 boundary = new Vector2(
                currentPixel.X + ((step.X > 0) ? 1 : 0),
                currentPixel.Y + ((step.Y > 0) ? 1 : 0));

            Vector2 boundaryToStart = boundary - rayStart;

            Vector2 rayDirInv = new Vector2(1 / rayDir.X, 1 / rayDir.Y);

            Vector2 tMax = new Vector2(boundaryToStart.X * rayDirInv.X, boundaryToStart.Y * rayDirInv.Y);
            Vector2 tDelta = new Vector2(step.X * rayDirInv.X, step.Y * rayDirInv.Y);

            if (collectDebugInfo)
            {
                debugInfo.StartPixel = currentPixel;
                debugInfo.EndPixel = endPixel;
            }

            #endregion

            #region traversal

            List<Vector2> footprintPixels = new List<Vector2>();

            //int maxIterations = (int)(2 * rayDir.Length);
            //int iterations = 0;
            debugInfo.EntryPoints.Add(rayStart);
            Vector2 entry = rayStart;
            while (currentPixel != endPixel)
            {
                #region visit current pixel - implementation dependent code

                if (collectDebugInfo)
                {
                    debugInfo.VisitedPixels.Add(currentPixel);
                    //debugInfo.EntryPoints.Add(rayStart + Math.Min(tMax.X, tMax.Y) * rayDir);
                }

                #endregion

                if (tMax.X < tMax.Y)
                {
                    entry = rayStart + tMax.X * rayDir;
                    tMax.X += tDelta.X;
                    currentPixel.X += step.X;
                }
                else
                {
                    entry = rayStart + tMax.Y * rayDir;
                    tMax.Y += tDelta.Y;
                    currentPixel.Y += step.Y;
                }
                debugInfo.EntryPoints.Add(entry);
                //if (iterations == maxIterations) break;
                //iterations++;
            }
            debugInfo.EntryPoints.Add(rayEnd);

            #endregion

            return null;
        }
    }
}
