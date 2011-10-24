namespace BokehLab.RayTracing
{
    using System;
    using BokehLab.FloatMap;
    using BokehLab.Math;
    using OpenTK;

    // TODO: project the ray into the frustum space to match it with the depth map

    public class HeightField : IIntersectable
    {
        // [X,Y,layer]
        // layers ordered from near to far depth
        private FloatMapImage[] depthLayers;

        public int layerCount;

        public float epsilon;

        public HeightField(FloatMapImage[] depthLayers)
        {
            this.epsilon = 0.001f;
            this.depthLayers = depthLayers;
            this.layerCount = depthLayers.Length;
        }

        public float GetDepth(int x, int y, int layer)
        {
            return depthLayers[layer].Image[x, y, 0];
        }

        /// <summary>
        /// Intersects a height-field with a ray.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A height-field is a layered 2D table with depth values. At each
        /// position there can be multiple pixels in layers ordered by
        /// increasing depth.
        /// </para>
        /// <para>
        /// This procedures find the position of intersection in the
        /// height-field along with the depth of the intersected pixel, or
        /// a zero vector in case of no intersection, and indicates if there
        /// was an intersection.
        /// </para>
        /// <para>
        /// Note that there is are only divisions at beginning and the loop
        /// contains only additions, multiplications, Math.Abs() and
        /// height-field table lookups.
        /// </para>
        /// <para>
        /// Based on the line footprint traversal algorithm. See
        /// BokehLab.Math.LineFootprint.
        /// </para>
        /// </remarks>
        /// <param name="ray">incoming ray</param>
        /// <param name="heightfield">height-field to be intersected</param>
        /// <returns>Intersection instance - position of the intersection in
        /// the height-field (only the pixel corner, not the exact position)
        /// - if the was an intersection; null otherwise</returns>
        public Intersection Intersect(Ray ray)
        {
            // 2D ray projection onto the height-field plane
            Vector2 rayEnd = (Vector2)(ray.Origin + ray.Direction).Xy;
            Vector2 dir = (Vector2)ray.Direction.Xy;
            bool rayGoesRatherHorizontal = Math.Abs(dir.X) > Math.Abs(dir.Y);

            Vector2 dirInv = new Vector2((float)(1 / ray.Direction.X), (float)(1 / ray.Direction.Y));
            Vector2 rayDzOverDxy = (float)ray.Direction.Z * dirInv;
            // direction to the nearest corner: [1,1], [1,-1], [-1,1] or [-1,-1]
            Vector2 relDir = new Vector2(Math.Sign(dir.X), Math.Sign(dir.Y));
            // converted to a single pixel: [1,1], [1,0], [0,1] or [0,0]
            Vector2 relCorner = 0.5f * (relDir + new Vector2(1, 1));

            // point where the 2D ray projection enters the current pixel
            Vector3 entry = (Vector3)ray.Origin;
            Vector2 currentPixel = GetPixelCorner(entry.Xy);
            Vector2 endPixel = GetPixelCorner(rayEnd);
            // absolute position of the nearest corner
            Vector2 corner = currentPixel + relCorner;

            while (currentPixel != endPixel)
            {
                // Get a direction of a vector perpendicular to directions
                // from the current position both to the nearest corner and
                // the end of the ray (it is aligned with the Z axis).

                float crossLength = Cross2d(corner - entry.Xy, rayEnd - entry.Xy);
                // it is equavalent to:
                // float crossLength = Vector3.Cross(new Vector3(corner - entry.Xy), new Vector3(rayEnd - entry.Xy)).Z;

                // The direction of the cross vector determines the relative
                // orientation of the two examined vectors:
                // 0 (+- epsilon) -> go across the corner
                // 1 -> go to the clockwise edge
                // -1 -> go to the counter-clockwise edge

                Vector2 nextDir = relDir; // across the corner by default
                if (Math.Abs(crossLength) > epsilon)
                {
                    // add a vector perpendicular in one or another direction
                    // to get a vector 45 degrees apart from the corner
                    Vector2 ccw = new Vector2(-relDir.Y, relDir.X);
                    if (crossLength > 0)
                    {
                        // (relDir + ccw) / 2 -> clockwise edge
                        nextDir += ccw;
                    }
                    else
                    {
                        // (relDir - ccw) / 2 -> counter-clockwise edge
                        nextDir -= ccw;
                    }
                    nextDir *= 0.5f;
                }

                // point where the 2D ray projection exits the current pixel
                Vector3 exit = new Vector3(IntersectPixelEdge2d(entry.Xy, dir, dirInv, corner, nextDir));
                if (rayGoesRatherHorizontal)
                {
                    exit.Z = (float)(ray.Origin.Z + (exit.X - ray.Origin.X) * rayDzOverDxy.X);
                }
                else
                {
                    exit.Z = (float)(ray.Origin.Z + (exit.Y - ray.Origin.Y) * rayDzOverDxy.Y);
                }

                // compute intersection with the height-field pixel (in several layers)
                for (int layer = 0; layer < layerCount; layer++)
                {
                    float pixelZ = GetDepth((int)currentPixel.X, (int)currentPixel.Y, layer);
                    if (IntersectsHeightFieldPixel(entry.Z, exit.Z, pixelZ))
                    {
                        return new Intersection(new Vector3d(currentPixel.X, currentPixel.Y, pixelZ));
                    }
                }

                entry = exit;
                currentPixel += nextDir;
                corner += nextDir;
            }

            // There remained a little bit of the ray which can intersect the
            // current pixel or the ray is perpendicular to the heightfield.
            // For the last fragment of the ray the exit point is the ray end
            // which might not intersect a pixel edge.
            float endZ = (float)(ray.Origin.Z + ray.Direction.Z);
            for (int layer = 0; layer < layerCount; layer++)
            {
                float pixelZ = GetDepth((int)currentPixel.X, (int)currentPixel.Y, layer);
                if (IntersectsHeightFieldPixel(entry.Z, endZ, pixelZ))
                {
                    return new Intersection(new Vector3d(currentPixel.X, currentPixel.Y, pixelZ));
                }
            }

            return null;
        }

        /// <summary>
        /// Tests whether a ray going over a heightfield pixel intersects it or not.
        /// </summary>
        /// <remarks>
        /// There is an intersection if the heightfield pixel depth is between
        /// depths of ray entry and exit points. In case of equality (up to
        /// epsilon) the ray touches the pixel. Otherwise it misses the pixel.
        /// </remarks>
        /// <param name="entryZ"></param>
        /// <param name="exitZ"></param>
        /// <param name="heightfieldZ"></param>
        /// <returns></returns>
        private static bool IntersectsHeightFieldPixel(float entryZ, float exitZ, float heightfieldZ)
        {
            return Math.Sign(entryZ - heightfieldZ) != Math.Sign(exitZ - heightfieldZ);
        }

        private static Vector2 IntersectPixelEdge2d(
            Vector2 rayStart,
            Vector2 dir,
            Vector2 dirInv,
            Vector2 corner,
            Vector2 nextDir)
        {
            if (nextDir.X == 0)
            {
                return rayStart - dir * (rayStart.Y - corner.Y) * dirInv.Y;
            }
            else if (nextDir.Y == 0)
            {
                return rayStart - dir * (rayStart.X - corner.X) * dirInv.X;
            }
            else
            {
                return corner;
            }
        }

        private static Vector2 GetPixelCorner(Vector2 position)
        {
            return new Vector2((float)Math.Floor(position.X), (float)Math.Floor(position.Y));
        }

        private static float Cross2d(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }
    }
}
