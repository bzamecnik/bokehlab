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
                return footprintPixels;
            }

            Vector2 relDir = new Vector2(Math.Sign(dir.X), Math.Sign(dir.Y));
            Vector2 relCorner = 0.5f * (relDir + new Vector2(1, 1));

            Vector2 current = rayStart;
            Vector2 currentPixel = GetPixelCorner(current);
            Vector2 endPixel = GetPixelCorner(rayEnd);
            footprintPixels.Add(currentPixel);

            while (currentPixel != endPixel)
            {
                Vector2 corner = currentPixel + relCorner;
                //float crossLength = Vector3.Cross(
                //    new Vector3(corner - current),
                //    new Vector3(rayEnd - current)).Z;
                float crossLength = Cross2d(corner - current, rayEnd - current);

                Vector2 nextDir = relDir;
                if (Math.Abs(crossLength) > Epsilon)
                {
                    Vector2 ccw = new Vector2(-relDir.Y, relDir.X);
                    if (crossLength > 0)
                    {
                        nextDir += ccw;
                    }
                    else
                    {
                        nextDir -= ccw;
                    }
                    nextDir *= 0.5f;
                }
                currentPixel += nextDir;
                footprintPixels.Add(currentPixel);
                current = IntersectPixel(current, rayEnd, corner, relDir, nextDir);
            }
            return footprintPixels;
        }

        private static Vector2 IntersectPixel(
            Vector2 rayStart,
            Vector2 rayEnd,
            Vector2 corner,
            Vector2 relDir,
            Vector2 nextDir)
        {
            Vector2 dir = rayEnd - rayStart;
            if (nextDir.X == 0)
            {
                return rayStart - dir * (rayStart.Y - corner.Y) / dir.Y;
            }
            else if (nextDir.Y == 0)
            {
                return rayStart - dir * (rayStart.X - corner.X) / dir.X;
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
