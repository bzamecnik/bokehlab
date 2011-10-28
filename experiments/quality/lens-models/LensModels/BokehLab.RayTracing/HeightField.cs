namespace BokehLab.RayTracing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BokehLab.FloatMap;
    using BokehLab.Math;
    using OpenTK;
    using System.Diagnostics;

    // TODO: project the ray into the frustum space to match it with the depth map

    // Assume the height field depth values are in range [0.0; 1.0] ~ [near; far].
    // Value 1.0 also mean no data.
    // Values in layers within each pixel are assumed to be ordered by increasing
    // depth - the input data are assumed to be obtained by depth peeling.

    public class HeightField : IIntersectable
    {
        // [X,Y,layer]
        // layers ordered from near to far depth
        private FloatMapImage[] depthLayers;

        private int width = 0;
        private int height = 0;

        public int Width { get { return width; } internal set { width = value; } }
        public int Height { get { return height; } internal set { height = value; } }

        private int layerCount;
        public int LayerCount { get { return layerCount; } }

        private float epsilonForCorners;
        public float EpsilonForCorners { get { return epsilonForCorners; } set { epsilonForCorners = value; } }

        // If the ray goes through a hole in subsequent pixels which is
        // tighter in depth that this epsilon we can consider it to be an
        // intersection.
        private float epsilonForClosePixelDepth;
        public float EpsilonForClosePixelDepth { get { return epsilonForClosePixelDepth; } set { epsilonForClosePixelDepth = value; } }

        public HeightField(int width, int height)
            : this(new FloatMapImage[] { })
        {
            this.width = width;
            this.height = height;
        }

        public HeightField(IEnumerable<FloatMapImage> depthLayers)
        {
            this.epsilonForCorners = 0.001f;
            this.epsilonForClosePixelDepth = 0.05f;
            Debug.Assert(depthLayers != null);
            //Debug.Assert(depthLayers.Length > 0);
            this.depthLayers = depthLayers.ToArray();
            this.layerCount = this.depthLayers.Length;
            if (layerCount > 0)
            {
                this.width = (int)this.depthLayers[0].Width;
                this.height = (int)this.depthLayers[0].Height;
            }
        }

        public float GetDepth(int x, int y, int layer)
        {
            Debug.Assert(layer < layerCount);
            Debug.Assert(x >= 0);
            Debug.Assert(y >= 0);
            Debug.Assert(x < width);
            Debug.Assert(y < height);
            return depthLayers[layer].Image[x, y, 0];
        }

        /// <summary>
        /// Intersects a height field with a ray.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A height field is a layered 2D table with depth values. At each
        /// position there can be multiple pixels in layers ordered by
        /// increasing depth.
        /// </para>
        /// <para>
        /// This procedures find the position of intersection in the
        /// height field along with the depth of the intersected pixel, or
        /// a zero vector in case of no intersection, and indicates if there
        /// was an intersection.
        /// </para>
        /// <para>
        /// Note that there is are only divisions at beginning and the loop
        /// contains only additions, multiplications, Math.Abs() and
        /// height field table lookups.
        /// </para>
        /// <para>
        /// Based on the line footprint traversal algorithm. See
        /// BokehLab.Math.LineFootprint.
        /// </para>
        /// </remarks>
        /// <param name="ray">incoming ray</param>
        /// <param name="heightfield">height field to be intersected</param>
        /// <returns>Intersection instance - position of the intersection in
        /// the height field (only the pixel corner, not the exact position)
        /// - if the was an intersection; null otherwise</returns>
        public Intersection Intersect(Ray ray)
        {
            FootprintDebugInfo debugInfo = null;
            return Intersect(ray, ref debugInfo);
        }

        internal Intersection Intersect(
            Ray ray,
            ref FootprintDebugInfo debugInfo)
        {
            bool collectDebugInfo = debugInfo != null;

            if (Math.Abs(ray.Direction.Z) < epsilonForCorners)
            {
                return null;
            }
            bool rayGoesFromDepth = ray.Direction.Z < 0;

            Vector3 rayOrigin = (Vector3)ray.Origin;
            // 2D ray projection onto the height field plane
            Vector2 rayEnd = (Vector2)(ray.Origin + ray.Direction).Xy;
            Vector2 dir = (Vector2)ray.Direction.Xy;
            bool rayGoesRatherHorizontal = Math.Abs(dir.X) > Math.Abs(dir.Y);

            Vector2 dirInv = new Vector2((float)(1 / ray.Direction.X), (float)(1 / ray.Direction.Y));
            Vector2 rayDzOverDxy = (float)ray.Direction.Z * dirInv;
            // direction to the nearest corner: [1,1], [1,-1], [-1,1] or [-1,-1]
            Vector2 relDir = new Vector2(Math.Sign(dir.X), Math.Sign(dir.Y));
            bool isDirectionAxisAligned = relDir.X * relDir.Y == 0;
            // relDir converted to a single pixel: [1,1], [1,0], [0,1] or [0,0]
            Vector2 relCorner = isDirectionAxisAligned ? relDir : (0.5f * (relDir + new Vector2(1, 1)));

            // point where the 2D ray projection enters the current pixel
            Vector3 entry = (Vector3)ray.Origin;
            Vector2 entryXY = entry.Xy;

            Vector2 currentPixel = GetPixelCorner(entryXY, relDir);
            Vector2 endPixel = GetPixelCorner(rayEnd, -relDir);

            if (collectDebugInfo)
            {
                debugInfo.StartPixel = currentPixel;
                debugInfo.EndPixel = endPixel;
            }

            // absolute position of the nearest corner
            Vector2 corner = currentPixel + relCorner;

            // depth of the last square in the previous pixel which was
            // completely in front of the ray
            float? previousLastZ = null;

            while (currentPixel != endPixel)
            {
                if (collectDebugInfo)
                {
                    debugInfo.VisitedPixels.Add(currentPixel);
                    debugInfo.EntryPoints.Add(entryXY);
                }

                if ((currentPixel.X < 0) || (currentPixel.X >= width) ||
                    (currentPixel.Y < 0) || (currentPixel.Y >= height))
                {
                    return null;
                }

                // We can only decide the orientation to nearest corner only
                // if the ray direction in the XY plane is axis aligned.
                // Otherwise we have to use the original relative direction.
                // Also this direction is used if traversing directly across
                // the corner.

                Vector2 nextDir = relDir; // across the corner by default
                if (!isDirectionAxisAligned)
                {
                    // Get a direction of a vector perpendicular to directions
                    // from the current position both to the nearest corner and
                    // the end of the ray (it is aligned with the Z axis).

                    float crossLength = (corner.X - entryXY.X) * (rayEnd.Y - entryXY.Y)
                        - (corner.Y - entryXY.Y) * (rayEnd.X - entryXY.X);
                    //   it is equavalent to:
                    // float crossLength = Cross2d(corner - entryXY, rayEnd - entryXY);
                    //   or to:
                    // float crossLength = Vector3.Cross(new Vector3(corner - entryXY), new Vector3(rayEnd - entryXY)).Z;

                    // The direction of the cross vector determines the relative
                    // orientation of the two examined vectors:
                    // 0 (+- epsilon) -> go across the corner
                    // > 0 -> go to the clockwise edge
                    // < 0 -> go to the counter-clockwise edge

                    if (Math.Abs(crossLength) > epsilonForCorners)
                    {
                        // add a vector perpendicular in one or another direction
                        // to get a vector 45 degrees apart from the corner
                        Vector2 cw = new Vector2(-relDir.Y, relDir.X);
                        if (crossLength > 0)
                        {
                            // (relDir + ccw) / 2 -> clockwise edge
                            nextDir += cw;
                        }
                        else
                        {
                            // (relDir - ccw) / 2 -> counter-clockwise edge
                            nextDir -= cw;
                        }
                        nextDir *= 0.5f;
                    }
                }

                // point where the 2D ray projection exits the current pixel
                Vector3 exit = new Vector3(IntersectPixelEdge2d(entryXY, dir, dirInv, corner, nextDir));
                if (rayGoesRatherHorizontal)
                {
                    exit.Z = rayOrigin.Z + (exit.X - rayOrigin.X) * rayDzOverDxy.X;
                }
                else
                {
                    exit.Z = rayOrigin.Z + (exit.Y - rayOrigin.Y) * rayDzOverDxy.Y;
                }

                // compute intersection with the height field pixel (in several layers)
                Intersection isec = IntersectLayerAtPixel(currentPixel, entry.Z, exit.Z, rayGoesFromDepth, collectDebugInfo, debugInfo, ref previousLastZ);
                if (isec != null)
                {
                    return isec;
                }

                entry = exit;
                entryXY = entry.Xy;
                currentPixel += nextDir;
                corner += nextDir;
            }

            // tail of the ray (the end pixel)
            if (collectDebugInfo)
            {
                debugInfo.VisitedPixels.Add(currentPixel);
                debugInfo.EntryPoints.Add(entryXY);
            }
            float endZ = (float)(ray.Origin.Z + ray.Direction.Z);
            return IntersectLayerAtPixel(currentPixel, entry.Z, endZ, rayGoesFromDepth, collectDebugInfo, debugInfo, ref previousLastZ);
        }

        private Intersection IntersectLayerAtPixel(
            Vector2 currentPixel,
            float entryZ,
            float exitZ,
            bool rayGoesFromDepth,
            bool collectDebugInfo,
            FootprintDebugInfo debugInfo,
            ref float? previousLastZ)
        {
            // depth of the last square in the current pixel which was
            // completely in front of the ray
            float? currentLastZ = null;
            int layer;
            for (layer = 0; layer < layerCount; layer++)
            {
                // Tests whether a ray going over a height field pixel intersects it or not.
                //
                // There is an intersection if the heightfield pixel depth is between
                // depths of ray entry and exit points. In case of equality (up to
                // epsilon) the ray touches the pixel. Otherwise it misses the pixel.
                float layerZ = GetDepth((int)currentPixel.X, (int)currentPixel.Y, layer);
                if (layerZ == 1)
                {
                    // early termination - no data in the height field
                    break;
                }
                if (layer == 0)
                {
                    currentLastZ = layerZ;
                }
                if (Math.Sign(entryZ - layerZ) != Math.Sign(exitZ - layerZ))
                {
                    // ray crosses the square, proper intersection
                    if (collectDebugInfo)
                    {
                        debugInfo.LayerOfIntersection = layer;
                    }
                    return new Intersection(new Vector3d(currentPixel.X, currentPixel.Y, layerZ));
                }
                else if ((layerZ > exitZ) ^ rayGoesFromDepth)
                {
                    // termination since the rest of layers is also behind
                    break;
                }
                // else: ray misses the square in front of it
                currentLastZ = layerZ;
            }
            // ray misses the square behind it
            if (previousLastZ.HasValue && currentLastZ.HasValue)
            {
                //float diff = (rayGoesFromDepth ? -1 : 1) *
                //    (previousLastZ.Value - currentLastZ.Value);
                //if ((diff > 0) && (diff < epsilonForClosePixelDepth))
                //{
                float diff = previousLastZ.Value - currentLastZ.Value;
                if ((Math.Sign(currentLastZ.Value - entryZ) != Math.Sign(previousLastZ.Value - entryZ)) &&
                    (Math.Abs(diff) < EpsilonForClosePixelDepth))
                {
                    // in case the squares of subsequent pixels are too
                    // close and the ray goes between them we can consider
                    // it an intersection
                    if (collectDebugInfo)
                    {
                        debugInfo.LayerOfIntersection = layer;
                    }
                    return new Intersection(new Vector3d(currentPixel.X, currentPixel.Y, 0.5f * diff));
                }
            }
            previousLastZ = currentLastZ;
            return null;
        }

        private static Vector2 IntersectPixelEdge2d(
            Vector2 entry,
            Vector2 dir,
            Vector2 dirInv,
            Vector2 corner,
            Vector2 nextDir)
        {
            if (nextDir.X == 0)
            {
                //return entry - dir * (entry.Y - corner.Y) * dirInv.Y;
                float a = (entry.Y - corner.Y) * dirInv.Y;
                return new Vector2(entry.X - dir.X * a, entry.Y - dir.Y * a);
            }
            else if (nextDir.Y == 0)
            {
                //return entry - dir * (entry.X - corner.X) * dirInv.X;
                float a = (entry.X - corner.X) * dirInv.X;
                return new Vector2(entry.X - dir.X * a, entry.Y - dir.Y * a);
            }
            else
            {
                return corner;
            }
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

        private static float Cross2d(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        internal class FootprintDebugInfo
        {
            public List<Vector2> VisitedPixels;
            public List<Vector2> EntryPoints;
            public Vector2 StartPixel;
            public Vector2 EndPixel;
            public int LayerOfIntersection;

            public FootprintDebugInfo()
            {
                VisitedPixels = new List<Vector2>();
                EntryPoints = new List<Vector2>();
            }
        }
    }
}
