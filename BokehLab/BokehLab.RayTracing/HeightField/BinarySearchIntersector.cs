namespace BokehLab.RayTracing.HeightField
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using OpenTK;
    using BokehLab.Math;

    class BinarySearchIntersector : AbstractIntersector
    {
        public BinarySearchIntersector(HeightField heightField)
            : base(heightField)
        {
        }

        internal override Intersection Intersect(
            Vector3 start, Vector3 end,
            ref FootprintDebugInfo debugInfo)
        {
            int stepCount = 5;

            //float near = 2;
            //float far = 10;
            //Vector2 start = (near / (float)ray.Direction.Z) * new Vector2((float)ray.Origin.X, (float)ray.Origin.Y);
            //Vector3d rayEnd = ray.Origin + ray.Direction;
            //Vector2 end = (far / (float)ray.Direction.Z) * new Vector2((float)rayEnd.X, (float)rayEnd.Y);

            Vector2 startXY = start.Xy;
            Vector2 endXY = end.Xy;

            float depthSize = 1;
            float currentDepth = 0;
            float isecDepth = 1;

            Vector2 position = startXY;

            for (int i = 0; i < stepCount; i++)
            {
                depthSize *= 0.5f;
                position = startXY + currentDepth * endXY;
                float hfDepth = HeightField.GetDepthBilinear(position, 0);
                if (currentDepth >= hfDepth)
                {
                    isecDepth = currentDepth;
                    currentDepth -= 2 * depthSize;
                }
                currentDepth += depthSize;
            }
            if (isecDepth < (1 - 0.0001))
            {
                return new Intersection(new Vector3d(position.X, position.Y, isecDepth));
            }
            else
            {
                return null;
            }
        }
    }
}
