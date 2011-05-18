namespace BokehLab.RayTracing.Lens
{
    using BokehLab.Math;
    using OpenTK;

    public class PinholeLens : ILens
    {
        #region ILens Members

        public Ray Transfer(Vector3d objectPos, Vector3d lensPos)
        {
            return new Ray(lensPos, lensPos - objectPos);
        }

        public Vector3d GetBackSurfaceSample(Vector2d sample)
        {
            return Vector3d.Zero;
        }

        public Vector3d GetFrontSurfaceSample(Vector2d sample)
        {
            return Vector3d.Zero;
        }

        #endregion
    }
}
