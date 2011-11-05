using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using BokehLab.Math;

namespace BokehLab.RayTracing.HeightField
{
    public interface IIntersectionAlgorithm : IIntersectable
    {
        Intersection Intersect(Vector3 start, Vector3 end);
    }

    // TODO: project the ray into the frustum space to match it with the depth map

    // Assume the height field depth values are in range [0.0; 1.0] ~ [near; far].
    // Value 1.0 also mean no data.
    // Values in layers within each pixel are assumed to be ordered by increasing
    // depth - the input data are assumed to be obtained by depth peeling.

    public abstract class AbstractIntersector : IIntersectionAlgorithm
    {
        public HeightField HeightField { get; set; }

        public AbstractIntersector(HeightField heightField)
        {
            this.HeightField = heightField;
        }

        internal abstract Intersection Intersect(Vector3 start, Vector3 end, ref FootprintDebugInfo debugInfo);

        public Intersection Intersect(Vector3 start, Vector3 end)
        {
            FootprintDebugInfo debugInfo = null;
            return Intersect(start, end, ref debugInfo);
        }

        public Intersection Intersect(Ray ray)
        {
            FootprintDebugInfo debugInfo = null;
            return Intersect((Vector3)ray.Origin, (Vector3)(ray.Origin + ray.Direction), ref debugInfo);
        }
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
