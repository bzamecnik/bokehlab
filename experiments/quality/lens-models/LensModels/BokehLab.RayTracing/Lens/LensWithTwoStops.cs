namespace BokehLab.RayTracing.Lens
{
    using System;
    using BokehLab.Math;
    using OpenTK;

    /// <summary>
    /// Take an arbitrary lens and surrounds it with stop circular stops.
    /// </summary>
    public class LensWithTwoStops : ILens
    {
        /// <summary>
        /// An arbitrary lens.
        /// </summary>
        public ILens Lens { get; set; }

        /// <summary>
        /// Back stop. It should be behind the lens (on the senzor side).
        /// Its position can be set using its Z property.
        /// </summary>
        public Circle BackStop { get; set; }

        /// <summary>
        /// Front stop. It should be in front of the lens (on the scene side).
        /// Its position can be set using its Z property.
        /// </summary>
        public Circle FrontStop { get; set; }

        private Random random = new Random();

        public LensWithTwoStops()
        {
            Lens = new PinholeLens();
            BackStop = new Circle() { Z = 1 };
            FrontStop = new Circle() { Z = -1 };
        }

        #region ILens Members

        public Ray Transfer(Vector3d objectPos, Vector3d lensPos)
        {
            Ray incomingRay = new Ray(objectPos, lensPos - objectPos);
            if (BackStop.Intersect(incomingRay) == null)
            {
                return null;
            }
            Ray outgoingRay = Lens.Transfer(objectPos, lensPos);
            if (outgoingRay == null)
            {
                return null;
            }
            if (FrontStop.Intersect(outgoingRay) == null)
            {
                return null;
            }
            return outgoingRay;
        }

        public Vector3d GetBackSurfaceSample(Vector2d sample)
        {
            if (random.NextDouble() < 0.5)
            {
                return GenerateSampleAtStop(BackStop, sample);
            }
            else
            {
                return Lens.GetBackSurfaceSample(sample);
            }
        }

        public Vector3d GetFrontSurfaceSample(Vector2d sample)
        {
            if (random.NextDouble() < 0.5)
            {
                return GenerateSampleAtStop(FrontStop, sample);
            }
            else
            {
                return Lens.GetFrontSurfaceSample(sample);
            }
        }

        #endregion

        private Vector3d GenerateSampleAtStop(Circle stop, Vector2d sample)
        {
            var sample2d = Sampler.ConcentricSampleDisk(sample);
            return new Vector3d(
                stop.Radius * sample2d.X,
                stop.Radius * sample2d.Y,
                stop.Z);
        }
    }
}
