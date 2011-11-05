namespace BokehLab.RayTracing.Lens
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using OpenTK;
    using BokehLab.Math;

    public class PrecomputedComplexLens : ILens, IIntersectable
    {
        private ComplexLens ComplexLens { get; set; }
        private LensRayTransferFunction Lrtf { get; set; }
        private LensRayTransferFunction.Table3d LrtfTable { get; set; }

        public PrecomputedComplexLens(ComplexLens lens, string lrtfFilename, int sampleCount)
        {
            ComplexLens = lens;
            Lrtf = new LensRayTransferFunction(lens);
            // load precomputed LRTF from a file or compute it and save to file
            string filename = string.Format(lrtfFilename, sampleCount);
            LrtfTable = Lrtf.SampleLrtf3DCached(sampleCount, filename);
        }

        #region ILens Members

        public Ray Transfer(Ray incomingRay)
        {
            incomingRay.NormalizeDirection();
            var inParams = ComplexLens.ConvertBackSurfaceRayToParameters(incomingRay);
            var outParams = LrtfTable.EvaluateLrtf3D(inParams);
            return ComplexLens.ConvertParametersToFrontSurfaceRay(outParams);
        }

        public Ray Transfer(Vector3d objectPos, Vector3d lensPos)
        {
            Ray incomingRay = new Ray(lensPos, lensPos - objectPos);
            return Transfer(incomingRay);
        }

        public Vector3d GetBackSurfaceSample(Vector2d sample)
        {
            return ComplexLens.GetBackSurfaceSample(sample);
        }

        public Vector3d GetFrontSurfaceSample(Vector2d sample)
        {
            return ComplexLens.GetFrontSurfaceSample(sample);
        }

        #endregion

        #region IIntersectable Members

        public Intersection Intersect(Ray ray)
        {
            return ComplexLens.Intersect(ray);
        }

        #endregion
    }
}
