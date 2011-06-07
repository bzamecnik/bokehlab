namespace BokehLab.RayTracing.Lens
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using BokehLab.Math;
    using BokehLab.RayTracing;
    using OpenTK;

    /// <summary>
    /// Represents the Lens Ray Transfer Function (LRTF).
    /// </summary>
    /// <remarks>
    /// <para>
    /// The LRTF describes the behavior of a lens. It is a 4D->4D mapping from
    /// incoming ray to outgoing rays. Each ray can be described by four
    /// parameters: hemispherical coordinates (theta, phi) of the ray origin on
    /// the back or front surface of the lens and hemispherical coordinates of
    /// the ray direction (in a frame local to the ray origin with hemisphere
    /// pole aligned with the surface normal - pointing outside the lens).
    /// </para>
    /// <para>
    /// The purpose of the LRTF is to compute the ray traversal through a
    /// complex lens more efficiently. Instead of permanently traversing the
    /// lens its LRTF can be precomputed for some sample points and then
    /// approximately evaluated with interpolation. The precomputed samples
    /// can be stored in a table.
    /// </para>
    /// <para>
    /// Note that the LRTF is not always defined - some incoming rays
    /// can be absorbed inside the lens. Also rays that happen to be totally
    /// internally reflected are considered as not leaving the lens.
    /// </para>
    /// <para>
    /// For lenses with rotation symmetry around the optical axis the LRTF is
    /// also rotationally symmetric and one of the input dimensions (position
    /// phi) might not be stored. It is only needed to separate the rotation.
    /// Then the evaluation consists of three phases:
    /// - rotate the input (position and direction) so that its position phi
    ///   is 0
    /// - evaluate the LRTF
    /// - rotate the ouput using the original position phi
    /// </para>
    /// </remarks>
    public class LensRayTransferFunction
    {
        ComplexLens lens;

        public LensRayTransferFunction(ComplexLens lens)
        {
            this.lens = lens;
        }

        /// <summary>
        /// Sample the LRTF with fixed position and direction phi parameters,
        /// ie. vary the respective theta parameters.
        /// </summary>
        /// <param name="sampleCount">Number of sample in each of the two
        /// variable dimensions.</param>
        /// <returns>Table of rays in parametrized representation.</returns>
        public Parameters[] SampleLrtf(Vector2d position, Vector2d direction,
            VariableParameter variableParam, int sampleCount)
        {
            Parameters[] table = new Parameters[sampleCount];
            double positionTheta = position.X;
            double positionPhi = position.Y;
            double directionTheta = direction.X;
            double directionPhi = direction.Y;

            switch (variableParam)
            {
                case VariableParameter.PositionTheta:
                    positionTheta = 0;
                    break;
                case VariableParameter.PositionPhi:
                    positionPhi = 0;
                    break;
                case VariableParameter.DirectionTheta:
                    directionTheta = 0;
                    break;
                case VariableParameter.DirectionPhi:
                    directionPhi = 0;
                    break;
                default:
                    break;
            }

            double param = 0.0;
            double step = 1 / (double)(sampleCount - 1);
            for (int i = 0; i < sampleCount; i++)
            {
                switch (variableParam)
                {
                    case VariableParameter.PositionTheta:
                        positionTheta += param;
                        break;
                    case VariableParameter.PositionPhi:
                        positionPhi += param;
                        break;
                    case VariableParameter.DirectionTheta:
                        directionTheta += param;
                        break;
                    case VariableParameter.DirectionPhi:
                        directionPhi += param;
                        break;
                    default:
                        break;
                }
                Ray incomingRay = lens.ConvertParametersToBackSurfaceRay(
                    new Vector2d(positionTheta, positionPhi),
                    new Vector2d(directionTheta, directionPhi));
                Ray outgoingRay = lens.Transfer(incomingRay);
                if (outgoingRay != null)
                {
                    Vector4d outgoingRayParameters = lens.ConvertFrontSurfaceRayToParameters(
                        outgoingRay);
                    table[i] = new Parameters(
                        outgoingRayParameters.X,
                        outgoingRayParameters.Y,
                        outgoingRayParameters.Z,
                        outgoingRayParameters.W);
                }
                else
                {
                    table[i] = null;
                }
                param += step;
            }
            return table;
        }

        public enum VariableParameter
        {
            PositionTheta,
            PositionPhi,
            DirectionTheta,
            DirectionPhi,
        }

        public class Parameters
        {
            public double PositionTheta { get; set; }
            public double PositionPhi { get; set; }
            public double DirectionTheta { get; set; }
            public double DirectionPhi { get; set; }

            public Parameters(double posTheta, double posPhi,
                double dirTheta, double dirPhi)
            {
                PositionTheta = posTheta;
                PositionPhi = posPhi;
                DirectionTheta = dirTheta;
                DirectionPhi = dirPhi;
            }

            public override string ToString()
            {
                return string.Format("[{0}, {1}, {2}, {3}]", PositionTheta,
                    PositionPhi, DirectionTheta, DirectionPhi);
            }
        }
    }
}
