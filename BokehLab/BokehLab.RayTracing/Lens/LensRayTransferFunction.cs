namespace BokehLab.RayTracing.Lens
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using BokehLab.Math;
    using BokehLab.RayTracing;
    using OpenTK;
    using System.IO;

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
        public ComplexLens Lens { get; set; }

        public LensRayTransferFunction(ComplexLens lens)
        {
            this.Lens = lens;
        }

        /// <summary>
        /// Sample the LRTF with fixed position and direction phi parameters,
        /// ie. vary the respective theta parameters.
        /// </summary>
        /// <param name="sampleCount">Number of sample in each of the two
        /// variable dimensions.</param>
        /// <returns>Table of rays in parametrized representation.</returns>
        public Parameters[] SampleLrtf1D(LensRayTransferFunction.Parameters parameters,
            VariableParameter variableParam, int sampleCount)
        {
            Parameters[] table = new Parameters[sampleCount];

            switch (variableParam)
            {
                case VariableParameter.PositionTheta:
                    parameters.PositionTheta = 0;
                    break;
                case VariableParameter.PositionPhi:
                    parameters.PositionPhi = 0;
                    break;
                case VariableParameter.DirectionTheta:
                    parameters.DirectionTheta = 0;
                    break;
                case VariableParameter.DirectionPhi:
                    parameters.DirectionPhi = 0;
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
                        parameters.PositionTheta = param;
                        break;
                    case VariableParameter.PositionPhi:
                        parameters.PositionPhi = param;
                        break;
                    case VariableParameter.DirectionTheta:
                        parameters.DirectionTheta = param;
                        break;
                    case VariableParameter.DirectionPhi:
                        parameters.DirectionPhi = param;
                        break;
                    default:
                        break;
                }
                var outputParams = ComputeLrtf(parameters);
                if (!outputParams.IsDefined)
                {
                    outputParams = null;
                }
                table[i] = outputParams;
                param += step;
            }
            return table;
        }

        /// <summary>
        /// Uniformly sample the LRTF into a 3D table.
        /// </summary>
        /// <param name="sampleCount">Number of samples in each dimension</param>
        /// <returns>3D table of LRTF: [position phi, direction theta, direction phi]
        /// </returns>
        public Table3d SampleLrtf3D(int sampleCount)
        {
            Table3d table = new Table3d(sampleCount);

            // position phi is always 0.0 as it is separable
            Parameters inputParams = new Parameters(0, 0, 0, 0);
            double step = 1 / (double)(sampleCount - 1);
            for (int i = 0; i < sampleCount; i++)
            {
                inputParams.DirectionTheta = 0.0;
                for (int j = 0; j < sampleCount; j++)
                {
                    inputParams.DirectionPhi = 0.0;
                    for (int k = 0; k < sampleCount; k++)
                    {
                        table.Table[i, j, k] = ComputeLrtf(inputParams).ToVector4d();
                        inputParams.DirectionPhi += step;
                    }
                    inputParams.DirectionTheta += step;
                }
                inputParams.PositionTheta += step;
            }
            return table;
        }

        public Table3d SampleLrtf3DCached(int sampleCount, string filename)
        {
            Table3d table;
            if (File.Exists(filename))
            {
                table = Table3d.Load(filename);
            }
            else
            {
                table = SampleLrtf3D(sampleCount);
                table.Save(filename);
            }
            return table;
        }

        public Parameters ComputeLrtf(Parameters incomingParams)
        {
            Ray incomingRay = Lens.ConvertParametersToBackSurfaceRay(incomingParams);
            Ray outgoingRay = Lens.Transfer(incomingRay);
            Parameters outgoingParams = null;
            //Console.WriteLine("IN: {0}", incomingParams);
            //Console.WriteLine("IN: {0}", incomingRay);
            if (outgoingRay != null)
            {
                outgoingParams = Lens.ConvertFrontSurfaceRayToParameters(outgoingRay);
            }
            //Console.WriteLine("OUT: {0}", outgoingParams);
            //Console.WriteLine("OUT: {0}", outgoingRay);
            if (outgoingParams == null)
            {
                outgoingParams = new Parameters(Vector4d.Zero);
            }
            return outgoingParams;
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

            public Vector2d Position
            {
                get
                {
                    return new Vector2d(PositionTheta, PositionPhi);
                }
            }
            public Vector2d Direction
            {
                get
                {
                    return new Vector2d(DirectionTheta, DirectionPhi);
                }
            }

            public bool IsDefined
            {
                get { return PositionTheta != 0; }
            }

            public double this[VariableParameter param]
            {
                get
                {
                    switch (param)
                    {
                        case LensRayTransferFunction.VariableParameter.PositionTheta:
                            return PositionTheta;
                        case LensRayTransferFunction.VariableParameter.PositionPhi:
                            return PositionPhi;
                        case LensRayTransferFunction.VariableParameter.DirectionTheta:
                            return DirectionTheta;
                        case LensRayTransferFunction.VariableParameter.DirectionPhi:
                            return DirectionPhi;
                        default:
                            throw new InvalidOperationException();
                    }
                }
            }

            public Parameters(double posTheta, double posPhi,
                double dirTheta, double dirPhi)
            {
                PositionTheta = posTheta;
                PositionPhi = posPhi;
                DirectionTheta = dirTheta;
                DirectionPhi = dirPhi;
            }

            public Parameters(Vector4d p)
            {
                PositionTheta = p.X;
                PositionPhi = p.Y;
                DirectionTheta = p.Z;
                DirectionPhi = p.W;
            }

            public Vector4d ToVector4d()
            {
                return new Vector4d(PositionTheta, PositionPhi, DirectionTheta, DirectionPhi);
            }

            public override string ToString()
            {
                return string.Format(CultureInfo.InvariantCulture.NumberFormat,
                    "{{ {0}, {1}, {2}, {3} }}", PositionTheta,
                    PositionPhi, DirectionTheta, DirectionPhi);
            }
        }

        public class Table3d
        {
            public Vector4d[, ,] Table { get; private set; }
            public int Size { get; private set; }

            public Table3d(int size)
            {
                Table = new Vector4d[size, size, size];
                Size = size;
            }


            /// <summary>
            /// Evaluate tabulated LRTF by trilinear interpolation.
            /// </summary>
            /// <param name="table"></param>
            /// <param name="incomingParams"></param>
            /// <returns></returns>
            public Parameters EvaluateLrtf3D(Parameters incomingParams)
            {
                double step = Size - 1;
                double x = incomingParams.PositionTheta * step;
                double y = incomingParams.DirectionTheta * step;
                double z = incomingParams.DirectionPhi * step;

                int xFloor = (int)Math.Floor(x);
                int yFloor = (int)Math.Floor(y);
                int zFloor = (int)Math.Floor(z);
                int xCeil = (int)Math.Ceiling(x);
                int yCeil = (int)Math.Ceiling(y);
                int zCeil = (int)Math.Ceiling(z);

                double xFloat = x - xFloor;
                double yFloat = y - yFloor;
                double zFloat = z - zFloor;

                // trilinear interpolation over position theta, direction theta, direction phi:
                // position phi is ignored here

                // first linear interpolation
                Vector4d i0 = Vector4d.Lerp(
                    Table[xFloor, yFloor, zFloor],
                    Table[xFloor, yFloor, zCeil], zFloat);
                Vector4d i1 = Vector4d.Lerp(
                    Table[xFloor, yCeil, zFloor],
                    Table[xFloor, yCeil, zCeil], zFloat);
                Vector4d i2 = Vector4d.Lerp(
                    Table[xCeil, yFloor, zFloor],
                    Table[xCeil, yFloor, zCeil], zFloat);
                Vector4d i3 = Vector4d.Lerp(
                    Table[xCeil, yCeil, zFloor],
                    Table[xCeil, yCeil, zCeil], zFloat);

                // second linear interpolation
                Vector4d j0 = Vector4d.Lerp(i0, i1, yFloat);
                Vector4d j1 = Vector4d.Lerp(i2, i3, yFloat);

                // third linear interpolation
                Vector4d k = Vector4d.Lerp(j0, j1, xFloat);

                var outgoingParams = new Parameters(k);

                // rotate the resulting ray as rotation is separated from the 3D table
                outgoingParams.PositionPhi += incomingParams.PositionPhi;
                return outgoingParams;
            }

            public void Save(string filename)
            {
                // format:
                // NUMBER_OF_SAMPLES (as unsigned short integer)
                // table of values (stream of doubles in binary)
                // - ordering by input parameters: {posTheta {dirTheta {dirPhi}}
                // - each value consists four doubles (output parameters):
                //   {posTheta, posPhi, dirTheta, dirPhi}

                using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create, FileAccess.Write)))
                {
                    bw.Write((ushort)Size);

                    for (int i = 0; i < Size; i++)
                    {
                        for (int j = 0; j < Size; j++)
                        {
                            for (int k = 0; k < Size; k++)
                            {
                                Vector4d value = Table[i, j, k];
                                bw.Write(value.X);
                                bw.Write(value.Y);
                                bw.Write(value.Z);
                                bw.Write(value.W);
                            }
                        }
                    }
                }
            }

            public static Table3d Load(string filename)
            {
                using (BinaryReader bw = new BinaryReader(File.Open(filename, FileMode.Open, FileAccess.Read)))
                {
                    int size = bw.ReadUInt16();

                    Table3d table = new Table3d(size);

                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            for (int k = 0; k < size; k++)
                            {
                                Vector4d value = new Vector4d();
                                value.X = bw.ReadDouble();
                                value.Y = bw.ReadDouble();
                                value.Z = bw.ReadDouble();
                                value.W = bw.ReadDouble();
                                table.Table[i, j, k] = value;
                            }
                        }
                    }
                    return table;
                }
            }
        }
    }
}
