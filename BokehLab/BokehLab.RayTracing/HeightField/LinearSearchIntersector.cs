namespace BokehLab.RayTracing.HeightField
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using OpenTK;

    public class LinearSearchIntersector : AbstractIntersector
    {
        public int Steps { get; set; }

        public LinearSearchIntersector(HeightField heightField)
            : base(heightField)
        {
            Steps = 10;
        }

        internal override Intersection Intersect(Vector3 start, Vector3 end, ref FootprintDebugInfo debugInfo)
        {
            int steps = (int)Math.Floor(Math.Max((end - start).Xy.Length, 1));
            Vector3 rayStep = (end - start) / (float)steps;
            //Vector3 rayStep = (end - start) / (float)Steps;

            Vector3 currentPos = start;
            int prevIndicator = GetRayLayerIndicator(start.Z, HeightField.GetDepthBilinear(start.Xy, 0));
            int isecLayer = 0;

            for (int i = 0; i < steps; i++)
            {
                currentPos += rayStep;
                float layerDepth = HeightField.GetDepthBilinear(currentPos.Xy, 0);
                int indicator = GetRayLayerIndicator(currentPos.Z, layerDepth);
                if (layerDepth > 0)
                {
                    // some data present

                    // Find the first layer with indicator value 1. In case
                    // of a single layer it is the single value.
                    int indicatorDiff = prevIndicator - indicator;
                    if (indicatorDiff == 1)
                    {
                        isecLayer = 1;
                    }
                    if (isecLayer != 0)
                    {
                        if (debugInfo != null)
                        {
                            debugInfo.LayerOfIntersection = isecLayer - 1;
                        }
                        return new Intersection((Vector3d)currentPos);
                        // TODO: grab color from the particular color layer
                    }
                }
                prevIndicator = indicator;
            }
            return null;
        }

        private int[] GetRayLayersIndicators(float rayDepth, float[] layerDepths)
        {
            return layerDepths.Select(depth => GetRayLayerIndicator(rayDepth, depth)).ToArray();
        }

        private int GetRayLayerIndicator(float rayDepth, float layerDepth)
        {
            return Math.Max(Math.Sign(layerDepth - rayDepth), 0);
        }
    }
}
