namespace BokehLab.Math
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using OpenTK;

    public class LineFootprint
    {
        private static readonly float Epsilon = 0.001f;

        /// <summary>
        /// Traverses the footprint of a 2D ray, ie. all pixels in a grid
        /// intersected by the ray in the order of ray direction.
        /// </summary>
        /// <remarks>
        /// <para>
        /// In each pixel it looks for the direction to the next pixel.
        /// </para>
        /// <para>
        /// We'd like to traverse a footprint of a ray on a pixel grid, ie.
        /// visit each pixel the ray intersect in the order of the ray direction.
        /// <para>
        /// Note that in each step we can continue to one of three adjacent
        /// pixels depending which pixel edge (or corner) the ray intersects.
        /// The idea is to compare the direction of the ray with the direction
        /// to the corner between the two potential edges. We can compute the
        /// direction of the cross product of the two directions from the
        /// source point and decide where to go based on the sign of its z
        /// component:
        /// 
        /// 0 (+- epsilon) - go across the corner
        /// 1 - go to the clockwise edge
        /// -1 - go to the counter-clockwise edge
        /// </para>
        /// <para>
        /// If the next pixel is across the corner we can just add the
        /// relative corner direction to the original pixel position.
        /// </para>
        /// <para>
        /// The algorithm assumes that X coordinate goes rightward a Y upwards
        /// for the CW and CCW names, however it work even for Y pointing the
        /// other direction.
        /// </para>
        /// </remarks>
        /// <param name="rayStart">ray start point (in pixel grid space)</param>
        /// <param name="rayEnd">ray end point (in pixel grid space)</param>
        /// <returns>the list of pixels under the ray footprint</returns>
        public static IList<Vector2> TraverseFootprint(Vector2 rayStart, Vector2 rayEnd)
        {
            Vector2 dir = rayEnd - rayStart;

            List<Vector2> footprintPixels = new List<Vector2>();
            if (dir.LengthSquared < Epsilon)
            {
                // the ray is of almost zero length and can be discarded
                return footprintPixels;
            }

            Vector2 dirInv = new Vector2(1 / dir.X, 1 / dir.Y);
            // direction to the nearest corner: [1,1], [1,-1], [-1,1] or [-1,-1]
            Vector2 relDir = new Vector2(Math.Sign(dir.X), Math.Sign(dir.Y));
            // converted to a single pixel: [1,1], [1,0], [0,1] or [0,0]
            Vector2 relCorner = 0.5f * (relDir + new Vector2(1, 1));

            Vector2 current = rayStart;
            Vector2 currentPixel = GetPixelCorner(current);
            Vector2 endPixel = GetPixelCorner(rayEnd);
            footprintPixels.Add(currentPixel);

            while (currentPixel != endPixel)
            {
                // absolute position of the nearest corner
                Vector2 corner = currentPixel + relCorner;

                // Get a direction of a vector perpendicular to directions
                // from the current position both to the nearest corner and
                // the end of the ray (it is aligned with the Z axis).

                float crossLength = Cross2d(corner - current, rayEnd - current);
                // it is equavalent to:
                // float crossLength = Vector3.Cross(new Vector3(corner - current), new Vector3(rayEnd - current)).Z;

                // The direction of the cross vector determines the relative
                // orientation of the two examined vectors:
                // 0 (+- epsilon) -> go across the corner
                // 1 -> go to the clockwise edge
                // -1 -> go to the counter-clockwise edge

                Vector2 nextDir = relDir; // across the corner by default
                if (Math.Abs(crossLength) > Epsilon)
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
                currentPixel += nextDir;
                footprintPixels.Add(currentPixel);
                current = IntersectPixelEdge2d(current, dir, dirInv, corner, nextDir);
            }
            return footprintPixels;
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
        /// </remarks>
        /// <param name="ray">incoming ray</param>
        /// <param name="heightfield">height-field to be intersected</param>
        /// <param name="intersection">position of the intersection in the
        /// height-field (only the pixel corner, not the exact position)
        /// </param>
        /// <returns>true if the was an intersection; false otherwise</returns>
        public static bool IntersectHeightField(Ray ray, HeightField heightfield, out Vector3 intersection)
        {
            intersection = Vector3.Zero;

            Vector2 rayStart = (Vector2)ray.Origin.Xy;
            Vector2 rayEnd = (Vector2)(ray.Origin + ray.Direction).Xy;
            Vector2 dir = (Vector2)ray.Direction.Xy;
            bool rayGoesRatherHorizontal = Math.Abs(dir.X) > Math.Abs(dir.Y);

            Vector2 dirInv = new Vector2((float)(1 / ray.Direction.X), (float)(1 / ray.Direction.Y));
            Vector2 rayDzOverDxy = (float)ray.Direction.Z * dirInv;
            // direction to the nearest corner: [1,1], [1,-1], [-1,1] or [-1,-1]
            Vector2 relDir = new Vector2(Math.Sign(dir.X), Math.Sign(dir.Y));
            // converted to a single pixel: [1,1], [1,0], [0,1] or [0,0]
            Vector2 relCorner = 0.5f * (relDir + new Vector2(1, 1));

            Vector3 entry = (Vector3)ray.Origin;
            Vector2 currentPixel = GetPixelCorner(entry.Xy);
            Vector2 endPixel = GetPixelCorner(rayEnd);
            // absolute position of the nearest corner
            Vector2 corner = currentPixel + relCorner;

            int layerCount = heightfield.LayerCount;

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
                if (Math.Abs(crossLength) > Epsilon)
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
                    float pixelZ = heightfield.Data[(int)currentPixel.X, (int)currentPixel.Y, layer];
                    if (IntersectsHeightfieldPixel(entry.Z, exit.Z, pixelZ))
                    {
                        intersection = new Vector3(currentPixel.X, currentPixel.Y, pixelZ);
                        return true;
                    }
                }

                entry = exit;
                currentPixel += nextDir;
                corner += nextDir;
            }

            // There remained a little bit of the ray which can intersect the
            // current pixel or the ray is perpendicular to the heightfield.
            float endZ = (float)(ray.Origin.Z + ray.Direction.Z);
            for (int layer = 0; layer < layerCount; layer++)
            {
                float pixelZ = heightfield.Data[(int)currentPixel.X, (int)currentPixel.Y, layer];
                if (IntersectsHeightfieldPixel(entry.Z, endZ, pixelZ))
                {
                    intersection = new Vector3(currentPixel.X, currentPixel.Y, pixelZ);
                    return true;
                }
            }

            return false;
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
        private static bool IntersectsHeightfieldPixel(float entryZ, float exitZ, float heightfieldZ)
        {
            return Math.Sign(entryZ - heightfieldZ) != Math.Sign(exitZ - heightfieldZ);
        }

        public class HeightField
        {
            // [X,Y,layer]
            // layers ordered from near to far depth
            public float[, ,] Data { get; set; }
            public int LayerCount { get { return Data.GetLength(2); } }
        }
    }
}
