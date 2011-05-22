namespace BokehLab.RayTracing.Lens
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using OpenTK;
    using BokehLab.Math;

    /// <summary>
    /// Represents a lens which transfers rays going through it.
    /// </summary>
    public interface ILens
    {
        // TODO:
        // - better use signature: Ray Transfer(Ray incomingRay);

        /// <summary>
        /// Transfers an incoming ray through the lens.
        /// </summary>
        /// <remarks>
        /// The ray start at objectPos and enters the lens at lensPos.
        /// </remarks>
        /// <param name="objectPos"></param>
        /// <param name="lensPos"></param>
        /// <returns>The outgoing ray or null if the ray cannot be transferred.
        /// </returns>
        Ray Transfer(Vector3d objectPos, Vector3d lensPos);

        // TODO:
        // - possibly better signature would be
        //   Ray Transfer(Ray incomingRay);
        //   where the incoming ray's origin is at lensPos and its direction is
        //   objectPos - lensPos

        /// <summary>
        /// Computes a sample point on the lens surface away from the scene.
        /// </summary>
        /// <param name="sample">Sample of unit square [0;1]^2.</param>
        /// <returns></returns>
        Vector3d GetBackSurfaceSample(Vector2d sample);

        /// <summary>
        /// Computes a sample point on the lens surface adjacent to the scene.
        /// </summary>
        /// <param name="sample">sample of unit square [0;1]^2.</param>
        /// <returns></returns>
        Vector3d GetFrontSurfaceSample(Vector2d sample);
    }
}
