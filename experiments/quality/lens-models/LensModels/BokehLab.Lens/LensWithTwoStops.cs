namespace BokehLab.Lens
{
    using BokehLab.Math;
    using OpenTK;

    public class LensWithTwoStops : ILens
    {
        ILens Lens;

        #region ILens Members

        public Ray Transfer(Vector3d objectPos, Vector3d lensPos)
        {
            throw new System.NotImplementedException();
        }

        public Vector3d GetBackSurfaceSample(Vector2d sample)
        {
            throw new System.NotImplementedException();
        }

        public Vector3d GetFrontSurfaceSample(Vector2d sample)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
