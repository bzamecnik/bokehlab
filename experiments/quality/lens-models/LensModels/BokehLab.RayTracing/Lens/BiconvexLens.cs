namespace BokehLab.RayTracing.Lens
{
    using System;
    using BokehLab.Math;
    using OpenTK;

    class BiconvexLens : ILens
    {
        #region ILens Members

        public Ray Transfer(Vector3d objectPos, Vector3d lensPos)
        {
            throw new NotImplementedException();
        }

        public Vector3d GetBackSurfaceSample(Vector2d sample)
        {
            throw new NotImplementedException();
        }

        public Vector3d GetFrontSurfaceSample(Vector2d sample)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
