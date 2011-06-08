namespace BokehLab.RayTracing.Lens
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using OpenTK;
    using BokehLab.Math;

    // TODO:
    // - it would be great if the lens could be defined in XML
    // - complex lenses are not symetric in general
    //   - for light tracing (traced from front to back) a reversed definition
    //     could be used along with a common back-to-front tracing algorithm
    // - sample only exit pupil not the whole back surface

    /// <summary>
    /// Complex lens model where lens are composed of multiple spherical or
    /// circular surfaces or circular stops.
    /// </summary>
    /// <remarks>
    /// Lens model by Kolb et al. Sequential ray tracing is performed from
    /// back to front.
    /// </remarks>
    public class ComplexLens : ILens, IIntersectable
    {
        public IList<ElementSurface> ElementSurfaces { get; private set; }

        private Sphere frontSphericalSurface;
        private Sphere backSphericalSurface;

        private double backSurfaceSinTheta;
        private double frontSurfaceSinTheta;

        private double backSurfaceApertureRadius;
        private double frontSurfaceApertureRadius;

        public double MediumRefractiveIndex { get; set; }

        private static readonly double epsilon = 1e-6;

        /// <summary>
        /// Creates and initializes a new instance of a complex lens.
        /// </summary>
        /// <param name="surfaces">List of element surfaces, ordered from
        /// back to front.</param>
        public ComplexLens(IList<ElementSurface> surfaces)
        {
            ElementSurfaces = surfaces;

            ElementSurface frontSurface = surfaces.LastOrDefault((surface) => surface.Surface is Sphere);
            frontSphericalSurface = (Sphere)frontSurface.Surface;
            frontSurfaceSinTheta = frontSphericalSurface.GetCapElevationAngleSine(frontSurface.ApertureRadius);
            frontSurfaceApertureRadius = frontSurface.ApertureRadius;

            ElementSurface backSurface = surfaces.FirstOrDefault((surface) => surface.Surface is Sphere);
            backSphericalSurface = (Sphere)backSurface.Surface;
            backSurfaceSinTheta = backSphericalSurface.GetCapElevationAngleSine(backSurface.ApertureRadius);
            backSurfaceApertureRadius = backSurface.ApertureRadius;

            MediumRefractiveIndex = Materials.Fixed.AIR;
            frontSurface.NextRefractiveIndex = MediumRefractiveIndex;
        }

        public static ComplexLens Create(
           IList<SphericalElementSurfaceDefinition> surfaceDefs)
        {
            return Create(surfaceDefs, Materials.Fixed.AIR, 1.0);
        }

        /// <summary>
        /// Creates a new instance of a complex lens using a definition of
        /// elements.
        /// </summary>
        /// <remarks>
        /// The first and last surfaces have to be spherical. TODO: this is
        /// needed only for simpler sampling. In general planar surfaces or
        /// stops could be sampled too.
        /// </remarks>
        /// <param name="surfaceDefs">List of definitions of spherical or
        /// planar element surfaces or stops. Ordered from front to back.
        /// Must not be empty or null.
        /// </param>
        /// <param name="mediumRefractiveIndex">Index of refraction of medium
        /// outside the lens. It is assumed there is one medium on the scene
        /// side, senzor side and inside the lens.</param>
        /// <returns>The created complex lens instance.</returns>
        public static ComplexLens Create(
            IList<SphericalElementSurfaceDefinition> surfaceDefs,
            double mediumRefractiveIndex,
            double scale)
        {
            var surfaces = new List<ElementSurface>();

            var surfaceDefsReverse = surfaceDefs.Reverse().ToList();
            // scale the lens if needed
            if (Math.Abs(scale - 1.0) > epsilon)
            {
                surfaceDefsReverse = surfaceDefsReverse.Select(surface => surface.Scale(scale)).ToList();
            }
            // thickness of the whole lens (from front to back apex)
            // (without the distance to the senzor - backmost surface def.)
            double lensThickness = surfaceDefsReverse.Skip(1).Sum(def => def.Thickness);
            double elementBasePlaneShiftZ = lensThickness;

            double lastCapHeight = 0;
            double capHeight = 0;

            // definition list is ordered from front to back, working list
            // must be ordered from back to front, so a conversion has to be
            // performed
            int defIndex = 0;
            foreach (var definition in surfaceDefsReverse)
            {
                if (defIndex > 0)
                {
                    elementBasePlaneShiftZ -= definition.Thickness;
                }

                ElementSurface surface = new ElementSurface();
                surface.ApertureRadius = 0.5 * definition.ApertureDiameter;
                if (defIndex + 1 < surfaceDefsReverse.Count)
                {
                    surface.NextRefractiveIndex = surfaceDefsReverse[defIndex + 1].NextRefractiveIndex;
                }
                else
                {
                    surface.NextRefractiveIndex = mediumRefractiveIndex;
                }
                if (definition.CurvatureRadius.HasValue)
                {
                    // spherical surface
                    double radius = definition.CurvatureRadius.Value;
                    // convexity reverses when converting from front-to-back
                    // back-to-front ordering
                    surface.Convex = radius < 0;
                    Sphere sphere = new Sphere()
                    {
                        Radius = Math.Abs(radius)
                    };
                    sphere.Center = Math.Sign(radius) *
                        sphere.GetCapCenter(surface.ApertureRadius, Vector3d.UnitZ);
                    capHeight = Math.Sign(radius) * sphere.GetCapHeight(sphere.Radius, surface.ApertureRadius);
                    elementBasePlaneShiftZ -= lastCapHeight - capHeight;
                    sphere.Center += new Vector3d(0, 0, elementBasePlaneShiftZ);
                    surface.Surface = sphere;
                    surface.SurfaceNormalField = sphere;
                }
                else
                {
                    // planar surface
                    // both media are the same -> circular stop
                    // else -> planar element surface
                    surface.NextRefractiveIndex = definition.NextRefractiveIndex;
                    surface.Convex = true;
                    capHeight = 0;
                    elementBasePlaneShiftZ -= lastCapHeight - capHeight;
                    Circle circle = new Circle()
                    {
                        Radius = 0.5 * definition.ApertureDiameter,
                        Z = elementBasePlaneShiftZ,
                    };

                    surface.Surface = circle;
                    surface.SurfaceNormalField = circle;
                }
                lastCapHeight = capHeight;
                surfaces.Add(surface);
                defIndex++;
            }

            //DEBUG
            //foreach (var surface in surfaces)
            //{
            //    Console.WriteLine("{0}, {1}, {2}", surface.ApertureRadius,
            //        surface.Convex, surface.NextRefractiveIndex);
            //}

            ComplexLens lens = new ComplexLens(surfaces)
            {
                MediumRefractiveIndex = mediumRefractiveIndex
            };
            return lens;
        }

        /// <summary>
        /// Create a lens from a table.
        /// </summary>
        /// <param name="table">
        /// Rows: surfaces, ordered from front to back.
        /// Columns: CurvatureRadius, Thickness, NextRefractiveIndex, ApertureDiameter.
        /// </param>
        /// <param name="mediumRefractiveIndex"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static ComplexLens CreateFromSphericalElementTable(
            double?[,] table,
            double mediumRefractiveIndex,
            double scale)
        {
            var surfaceDefs = new List<SphericalElementSurfaceDefinition>();
            for (int surfaceIndex = 0; surfaceIndex < table.GetLength(0); surfaceIndex++)
            {
                var surface = new SphericalElementSurfaceDefinition();
                surface.CurvatureRadius = table[surfaceIndex, 0];
                surface.Thickness = table[surfaceIndex, 1].Value;
                surface.NextRefractiveIndex = table[surfaceIndex, 2].Value;
                surface.ApertureDiameter = table[surfaceIndex, 3].Value;
                surfaceDefs.Add(surface);
            }
            return Create(surfaceDefs, mediumRefractiveIndex, scale);
        }

        public static ComplexLens CreateBiconvexLens(
            double curvatureRadius,
            double apertureRadius,
            double thickness)
        {
            var surfaces = new List<ComplexLens.ElementSurface>();
            Sphere backSphere = new Sphere()
            {
                Radius = curvatureRadius,
            };
            backSphere.Center = backSphere.GetCapCenter(apertureRadius, -Vector3d.UnitZ);
            backSphere.Center += new Vector3d(0, 0, thickness);
            surfaces.Add(new ComplexLens.ElementSurface()
            {
                ApertureRadius = apertureRadius,
                NextRefractiveIndex = Materials.Fixed.GLASS_CROWN_K7,
                Surface = backSphere,
                SurfaceNormalField = backSphere,
                Convex = true
            });
            Sphere frontSphere = new Sphere()
            {
                Radius = curvatureRadius,
            };
            frontSphere.Center = frontSphere.GetCapCenter(apertureRadius, Vector3d.UnitZ);
            surfaces.Add(new ComplexLens.ElementSurface()
            {
                ApertureRadius = apertureRadius,
                Surface = frontSphere,
                SurfaceNormalField = frontSphere,
                Convex = false
            });

            ComplexLens lens = new ComplexLens(surfaces);
            return lens;
        }

        public static ComplexLens CreateDoubleGaussLens(
            double mediumRefractiveIndex,
            double scale)
        {
            double?[,] table = {
                { 58.950, 7.520, 1.670, 50.4 },
                { 169.660, 0.240, mediumRefractiveIndex, 50.4 },
                { 38.550, 8.050, 1.670, 46.0 },
                { 81.540, 6.550, 1.699, 46.0 },
                { 25.500, 11.410, mediumRefractiveIndex, 36.0 },
                { null, 9.0, mediumRefractiveIndex, 34.2 },
                { -28.990, 2.360, 1.603, 34.0 },
                { 81.540, 12.130, 1.658, 40.0 },
                { -40.770, 0.380, mediumRefractiveIndex, 40.0 },
                { 874.130, 6.440, 1.717, 40.0 },
                { -79.460, 72.228 /* distance to senzor*/, mediumRefractiveIndex, 40.0 },
            };
            return CreateFromSphericalElementTable(table, mediumRefractiveIndex, scale);
        }

        public static ComplexLens CreatePetzvalLens(
            double mediumRefractiveIndex,
            double scale)
        {
            double?[,] table = {
                { 52.9, 5.8, 1.517, 30 },
                { -41.4, 1.5, 1.575, 30 },
                { 436.2, 23.3, mediumRefractiveIndex, 30 },
                { null, 23.3, mediumRefractiveIndex, 25.17398 },
                { 104.8, 2.2, 1.575, 30 },
                { 36.8, 0.7, mediumRefractiveIndex, 30 },
                { 45.5, 3.6, 1.517000, 30 },
                { -149.5, 58.5, mediumRefractiveIndex, 30 },
            };
            // senzor diagonal diameter: 43.59859
            return CreateFromSphericalElementTable(table, mediumRefractiveIndex, scale);
        }

        #region ILens Members

        public Ray Transfer(Ray incomingRay)
        {
            IList<Vector3d> intersections;
            return TransferDebug(incomingRay, out intersections, false);
        }

        public Ray Transfer(Vector3d objectPos, Vector3d lensPos)
        {
            IList<Vector3d> intersections;
            Ray incomingRay = new Ray(objectPos, lensPos - objectPos);
            return TransferDebug(incomingRay, out intersections, false);
        }

        internal Ray TransferDebug(Ray incomingRay,
            out IList<Vector3d> intersections, bool saveIntersections)
        {
            //Console.WriteLine("Complex lens");

            intersections = null;
            if (saveIntersections)
            {
                intersections = new List<Vector3d>();
            }

            double lastRefractiveIndex = MediumRefractiveIndex;
            Vector3d shiftFromLensCenter = Vector3d.Zero;
            //Console.WriteLine("Blue, {0}, ", incomingRay.ToLine());
            //Console.WriteLine("Incoming: {0}, ", Ray.NormalizeDirection(incomingRay).ToString());
            Ray outgoingRay = new Ray(incomingRay);
            //if (ElementSurfaces.First().Surface.Intersect(incomingRay) == null)
            //{
            // The ray might have its origin just on the back surface.
            // Try move the origin a bit in order to let it intersect
            // the surface.
            Vector3d origin = incomingRay.Origin;
            origin.Z += 10e-6;
            incomingRay.Origin = origin;
            //}

            foreach (ElementSurface surface in ElementSurfaces)
            {
                double nextRefractiveIndex = surface.NextRefractiveIndex;

                // Compute intersection

                // the intersection is done on the element surface in its
                // normalized position
                Intersection intersection = surface.Surface.Intersect(incomingRay);
                if (intersection == null)
                {
                    // no intersection at all
                    return null;
                }

                if (saveIntersections)
                {
                    intersections.Add(intersection.Position);
                }

                if (intersection.Position.Xy.LengthSquared >
                    surface.ApertureRadius * surface.ApertureRadius)
                {
                    // intersection is outside the aperture
                    return null;
                }

                // Compute refracted ray

                if (Math.Abs(lastRefractiveIndex - nextRefractiveIndex) > epsilon)
                {
                    // there is a change of refractive index
                    Vector3d intersectionPos = intersection.Position;
                    Vector3d normal = surface.SurfaceNormalField.GetNormal(intersectionPos);
                    if (!surface.Convex)
                    {
                        normal = -normal;
                    }
                    incomingRay.NormalizeDirection();
                    Vector3d refractedDirection = Ray.Refract(
                        incomingRay.Direction, normal, lastRefractiveIndex,
                        nextRefractiveIndex, false);
                    if (refractedDirection == Vector3d.Zero)
                    {
                        return null;
                    }
                    outgoingRay = new Ray(intersectionPos, refractedDirection);
                }
                else
                {
                    // there's no border between different media and thus no refraction
                    outgoingRay = new Ray(incomingRay.Origin, incomingRay.Direction);
                }

                lastRefractiveIndex = nextRefractiveIndex;
                incomingRay = new Ray(outgoingRay.Origin, outgoingRay.Direction);
                //Console.WriteLine("Red, {0}, ", outgoingRay.ToLine());
                //Console.WriteLine("Outgoing: {0}, ", Ray.NormalizeDirection(outgoingRay).ToString());
            }

            outgoingRay.NormalizeDirection();
            return outgoingRay;
        }

        public Vector3d GetBackSurfaceSample(Vector2d sample)
        {
            Vector3d unitSphereSample = Sampler.UniformSampleSphereWithEqualArea(
                sample, backSurfaceSinTheta, 1);
            return backSphericalSurface.Center + backSphericalSurface.Radius * unitSphereSample;
        }

        public Vector3d GetFrontSurfaceSample(Vector2d sample)
        {
            Vector3d unitSphereSample = Sampler.UniformSampleSphereWithEqualArea(
                sample, frontSurfaceSinTheta, 1);
            return frontSphericalSurface.Center + frontSphericalSurface.Radius * (-unitSphereSample);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sample">Sample in parametric UV space [0; 1]^2</param>
        /// <param name="direction">Direction relative to local frame on the
        /// surface point in normalized spherical coordinates [0; 1]^2, similar
        /// to Sampler.UniformSampleSphere().
        /// X = theta ... elevation (from plane to normal)
        /// Y = phi ... azimuth</param>
        /// <returns></returns>
        public Ray ConvertParametersToBackSurfaceRay(
            LensRayTransferFunction.Parameters parameters)
        {
            Vector3d canonicalNormal = Vector3d.UnitZ;
            double surfaceSinTheta = backSurfaceSinTheta;
            Sphere sphericalSurface = backSphericalSurface;
            ElementSurface surface = ElementSurfaces.First();

            return ConvertParametersToSurfaceRay(parameters,
                canonicalNormal, surfaceSinTheta, sphericalSurface, surface);
        }

        public Ray ConvertParametersToFrontSurfaceRay(
            LensRayTransferFunction.Parameters parameters)
        {
            Vector3d canonicalNormal = -Vector3d.UnitZ;
            double surfaceSinTheta = frontSurfaceSinTheta;
            Sphere sphericalSurface = frontSphericalSurface;
            ElementSurface surface = ElementSurfaces.Last();

            return ConvertParametersToSurfaceRay(parameters,
                canonicalNormal, surfaceSinTheta, sphericalSurface, surface);
        }

        /// <summary>
        /// Convert a ray with origin at the back or front lens surface from
        /// its parametric representation.
        /// </summary>
        /// <param name="position">Position on lens surface in parameteric
        /// representation (normalized hemispherical coordinates).</param>
        /// <param name="direction">Direction of the ray with respect to the
        /// local frame in parameteric representation (normalized hemispherical
        /// coordinates).
        /// </param>
        /// <param name="canonicalNormal">Normal of the lens surface
        /// hemisphere (typically (0,0,1) for the back surface or (0,0,-1) for
        /// the front surface).</param>
        /// <param name="surfaceSinTheta">Sine of the surface spherical cap
        /// theta angle.</param>
        /// <param name="sphericalSurface">Lens surface represented as a
        /// sphere.</param>
        /// <param name="surface">Lens surface with its normal field.</param>
        /// <returns>Ray corresponding to its parametric representation.
        /// </returns>
        public Ray ConvertParametersToSurfaceRay(
            LensRayTransferFunction.Parameters parameters,
            Vector3d canonicalNormal, double surfaceSinTheta,
            Sphere sphericalSurface, ElementSurface surface)
        {
            // uniform spacing sampling for LRTF sampling
            Vector3d unitSpherePos = Sampler.SampleSphereWithUniformSpacing(
                parameters.Position, surfaceSinTheta, 1);
            unitSpherePos.Z *= canonicalNormal.Z;
            Vector3d lensPos = sphericalSurface.Center + sphericalSurface.Radius * unitSpherePos;

            // - get normal N at P
            Vector3d normalLocal = surface.SurfaceNormalField.GetNormal(lensPos);
            // - compute direction D from spherical coordinates (wrt normal Z = (0,0,+/-1))
            double theta = 0.5 * Math.PI * parameters.DirectionTheta;
            double phi = 2 * Math.PI * parameters.DirectionPhi;
            double cosTheta = Math.Cos(theta);
            Vector3d directionZ = new Vector3d(
                Math.Cos(phi) * cosTheta,
                Math.Sin(phi) * cosTheta,
                Math.Sin(theta) * canonicalNormal.Z);
            // - rotate D from Z to N frame
            //   - using a (normalized) quaternion Q
            //   - N and Z should be assumed to be already normalized
            //   - more efficient method: Efficiently building a matrix to
            //     rotate one vector to another [moller1999]
            normalLocal.Normalize(); // TODO: check if it is unnecessary
            Quaterniond q = Quaterniond.FromAxisAngle(
                Vector3d.Cross(canonicalNormal, normalLocal),
                Math.Acos(Vector3d.Dot(canonicalNormal, normalLocal)));
            q.Normalize();
            Vector3d rotatedDir = Vector3d.Transform(directionZ, q);
            if (surface.Convex)
            {
                rotatedDir = -rotatedDir;
            }
            Ray result = new Ray(lensPos, rotatedDir);
            return result;
        }

        public LensRayTransferFunction.Parameters ConvertBackSurfaceRayToParameters(Ray ray)
        {
            Vector3d canonicalNormal = Vector3d.UnitZ;
            double surfaceSinTheta = backSurfaceSinTheta;
            Sphere sphericalSurface = backSphericalSurface;
            ElementSurface surface = ElementSurfaces.First();

            return ConvertSurfaceRayToParameters(ray, canonicalNormal,
                surfaceSinTheta, sphericalSurface, surface);
        }

        public LensRayTransferFunction.Parameters ConvertFrontSurfaceRayToParameters(Ray ray)
        {
            Vector3d canonicalNormal = -Vector3d.UnitZ;
            double surfaceSinTheta = frontSurfaceSinTheta;
            Sphere sphericalSurface = frontSphericalSurface;
            ElementSurface surface = ElementSurfaces.Last();

            return ConvertSurfaceRayToParameters(ray, canonicalNormal,
                surfaceSinTheta, sphericalSurface, surface);
        }

        public LensRayTransferFunction.Parameters ConvertSurfaceRayToParameters(
            Ray ray,
            Vector3d canonicalNormal, double surfaceSinTheta,
            Sphere sphericalSurface, ElementSurface surface)
        {
            // - convert origin
            //   *- transform to hemispherical coordinates
            //     - find out if it is on the surface
            //   *- scale with respect to the spherical cap
            //   *- normalize
            Vector3d unitSpherePos = (ray.Origin - sphericalSurface.Center) / sphericalSurface.Radius;
            unitSpherePos.Z *= canonicalNormal.Z;
            Vector2d originParametric = Sampler.SampleSphereWithUniformSpacingInverse(
                unitSpherePos, surfaceSinTheta, 1);

            // - convert direction
            //   *- transform from camera space to local frame
            //     *- compute normal at origin
            Vector3d normalLocal = surface.SurfaceNormalField.GetNormal(ray.Origin);
            normalLocal.Normalize(); // TODO: check if it is unnecessary
            //     *- create rotation quaternion from canonical normal to local
            //       normal
            Quaterniond q = Quaterniond.FromAxisAngle(
                Vector3d.Cross(normalLocal, canonicalNormal),
                Math.Acos(Vector3d.Dot(normalLocal, canonicalNormal)));
            //     *- rotate
            Vector3d direction = ray.Direction;
            if (surface.Convex)
            {
                direction = -direction;
            }
            direction = Vector3d.Transform(ray.Direction, q);
            //   *- transform to hemispherical coordinates
            //     - find out if it is within the local hemisphere
            double sinTheta = direction.Z / canonicalNormal.Z;
            double dirTheta = Math.Asin(sinTheta);

            double cosTheta = Math.Sqrt(1 - sinTheta * sinTheta);
            // clamp to [-1; 1] to battle numerical inaccuracies
            double dirPhi = 0.0;
            if (cosTheta != 0)
            {
                double cosPhi = BokehLab.Math.MathHelper.Clamp(direction.X / cosTheta, -1, 1);
                dirPhi = Math.Acos(cosPhi);
            }
            //   *- normalize
            Vector2d directionParametric = new Vector2d(
                dirTheta / (0.5 * Math.PI),
                dirPhi / (2 * Math.PI));

            return new LensRayTransferFunction.Parameters(
                originParametric.X, originParametric.Y,
                directionParametric.X, directionParametric.Y);
        }

        #region IIntersectable Members

        public Intersection Intersect(Ray ray)
        {
            Intersection isec = backSphericalSurface.Intersect(ray);
            if ((isec != null) &&
                (isec.Position.Xy.LengthSquared >
                backSurfaceApertureRadius * backSurfaceApertureRadius))
            {
                return null;
            }
            return isec;
        }

        #endregion

        /// <summary>
        /// Representation of one element surface suitable for ray tracing.
        /// </summary>
        /// <remarks>
        /// The surfaces follow one after another from back of the lens to
        /// the front (the same way as ray tracing proceeds within the lens).
        /// Each surface is absolutely positioned in the camera space.
        /// </remarks>
        public class ElementSurface
        {
            /// <summary>
            /// The concrete intersectable surface object (sphere, plane, ...).
            /// </summary>
            public IIntersectable Surface;
            /// <summary>
            /// Surface which can be queried for normals.
            /// </summary>
            /// <remarks>
            /// This can be only a different view at one surface implementation
            /// (in addition to IIntersectable).
            /// </remarks>
            public INormalField SurfaceNormalField;
            /// <summary>
            /// Radius of aperture in the base (XY) plane.
            /// </summary>
            public double ApertureRadius;
            /// <summary>
            /// Index of refraction of the material after this surface.
            /// </summary>
            public double NextRefractiveIndex;
            /// <summary>
            /// Indicates whether the surface is convex when view from back.
            /// </summary>
            /// <remarks>
            /// Planar surface is convex.
            /// </remarks>
            public bool Convex;
        }

        /// <summary>
        /// Definition of one element surface. A list of such definitions
        /// should serve for creating an instance of the complex lens.
        /// </summary>
        /// <remarks>
        /// The surfaces follow one after another from front of the lens to
        /// the back. The format of definition should be suitable to the
        /// format in which the lens data are commonly available.
        /// Surfaces are relatively positioned one after another.
        /// </remarks>
        public class SphericalElementSurfaceDefinition
        {
            /// <summary>
            /// Signed radius of curvature. Positive: convex from front,
            /// negative: concave from front. In case of no value the surface
            /// is not spherical but rather planar (a planar glass surface or
            /// a stop).
            /// </summary>
            public double? CurvatureRadius;

            /// <summary>
            /// (Unsigned) distance from this element's apex to the next
            /// element's apex.
            /// </summary>
            public double Thickness;

            /// <summary>
            /// Index of refraction of the material after this surface.
            /// </summary>
            public double NextRefractiveIndex;

            /// <summary>
            /// Diameter (not radius!) of aperture in the base (XY) plane.
            /// </summary>
            public double ApertureDiameter;

            public SphericalElementSurfaceDefinition Scale(double scale)
            {
                return new SphericalElementSurfaceDefinition()
                {
                    ApertureDiameter = scale * ApertureDiameter,
                    CurvatureRadius = scale * CurvatureRadius,
                    Thickness = scale * Thickness,
                    NextRefractiveIndex = NextRefractiveIndex,
                };
            }
        }


    }
}
