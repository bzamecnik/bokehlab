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

        private float epsilonForCorners;
        public float EpsilonForCorners { get { return epsilonForCorners; } set { epsilonForCorners = value; } }

        // If the ray goes through a hole in subsequent pixels which is
        // tighter in depth that this epsilon we can consider it to be an
        // intersection.
        private float epsilonForClosePixelDepth;
        public float EpsilonForClosePixelDepth { get { return epsilonForClosePixelDepth; } set { epsilonForClosePixelDepth = value; } }

        public AmanatidesIntersector(HeightField heightField)
            : base(heightField)
        {
            this.epsilonForRayDir = 0.001f;
            this.epsilonForCorners = 0.0001f;
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
            Vector3 dir = end - start;
            Vector2 rayDir = dir.Xy;

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

            Vector2 currentPixel = GetPixelCorner(start.Xy, step);
            Vector2 endPixel = GetPixelCorner(start.Xy + rayDir, -step);

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

            int maxIterations = (int)(2 * rayDir.Length);
            int iterations = 0;
            debugInfo.EntryPoints.Add(rayStart);
            Vector3 entry = start;
            while (currentPixel != endPixel)
            {
                if (collectDebugInfo)
                {
                    debugInfo.VisitedPixels.Add(currentPixel);
                    //debugInfo.EntryPoints.Add(rayStart + Math.Min(tMax.X, tMax.Y) * rayDir);
                    debugInfo.EntryPoints.Add(entry.Xy);
                }

                Vector3 exit;
                if (tMax.X < tMax.Y)
                {
                    exit = start + tMax.X * dir;
                    tMax.X += tDelta.X;
                    currentPixel.X += step.X;
                }
                else
                {
                    exit = start + tMax.Y * dir;
                    tMax.Y += tDelta.Y;
                    currentPixel.Y += step.Y;
                }

                // height field intersection
                for (int layer = 0; layer < this.HeightField.LayerCount; layer++)
                {
                    float layerZ = this.HeightField.GetDepth((int)currentPixel.X, (int)currentPixel.Y, layer);
                    // we could compare:
                    // (1) sign(entry.Z - hf[pixel.xy]) != sign(exit.Z - hf[pixel.xy])
                    // (2) sign(entry.Z - hf[entry.xy]) != sign(exit.Z - hf[exit.xy])
                    // (3) sign(entry.Z - hf[middle.xy]) != sign(exit.Z - hf[middle.xy])
                    //Vector2 middle = 0.5f * (exit.Xy - entry.Xy);
                    //float layerZ = this.HeightField.GetDepth((int)middle.X, (int)middle.Y, 0);
                    if ((Math.Sign(layerZ - entry.Z) != Math.Sign(layerZ - exit.Z)))
                    {
                        if (collectDebugInfo)
                        {
                            debugInfo.LayerOfIntersection = layer;
                        }
                        return new Intersection(new Vector3d(currentPixel.X, currentPixel.Y, layerZ));
                    }
                }

                entry = exit;

                if (iterations == maxIterations) break;
                iterations++;
            }
            debugInfo.VisitedPixels.Add(currentPixel);
            debugInfo.EntryPoints.Add(rayEnd);

            #endregion

            return null;
        }

        private Vector2 GetPixelCorner(Vector2 position, Vector2 relDir)
        {
            Vector2 corner = new Vector2((float)Math.Floor(position.X), (float)Math.Floor(position.Y));
            if ((relDir.X < 0) && (position.X - corner.X < epsilonForCorners))
            {
                corner.X -= 1;
            }
            if ((relDir.Y < 0) && (position.Y - corner.Y < epsilonForCorners))
            {
                corner.Y -= 1;
            }
            return corner;
        }
    }
}
