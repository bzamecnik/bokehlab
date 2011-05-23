namespace BokehLab.RayTracing.Lens
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using OpenTK;
    using BokehLab.Math;

    // TODO:
    // - create lens from surface definition list
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
            foreach (var surface in surfaces)
            {
                Console.WriteLine("{0}, {1}, {2}", surface.ApertureRadius,
                    surface.Convex, surface.NextRefractiveIndex);
            }

            ComplexLens lens = new ComplexLens(surfaces)
            {
                MediumRefractiveIndex = mediumRefractiveIndex
            };
            return lens;
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
            var surfaces = new List<ComplexLens.SphericalElementSurfaceDefinition>();
            surfaces.Add(new ComplexLens.SphericalElementSurfaceDefinition()
            {
                CurvatureRadius = 58.950,
                Thickness = 7.520,
                NextRefractiveIndex = 1.670,
                ApertureDiameter = 50.4,
            });
            surfaces.Add(new ComplexLens.SphericalElementSurfaceDefinition()
            {
                CurvatureRadius = 169.660,
                Thickness = 0.240,
                NextRefractiveIndex = mediumRefractiveIndex,
                ApertureDiameter = 50.4,
            });
            surfaces.Add(new ComplexLens.SphericalElementSurfaceDefinition()
            {
                CurvatureRadius = 38.550,
                Thickness = 8.050,
                NextRefractiveIndex = 1.670,
                ApertureDiameter = 46.0,
            });
            surfaces.Add(new ComplexLens.SphericalElementSurfaceDefinition()
            {
                CurvatureRadius = 81.540,
                Thickness = 6.550,
                NextRefractiveIndex = 1.699,
                ApertureDiameter = 46.0,
            });
            surfaces.Add(new ComplexLens.SphericalElementSurfaceDefinition()
            {
                CurvatureRadius = 25.500,
                Thickness = 11.410,
                NextRefractiveIndex = mediumRefractiveIndex,
                ApertureDiameter = 36.0,
            });
            surfaces.Add(new ComplexLens.SphericalElementSurfaceDefinition()
            {
                CurvatureRadius = null,
                Thickness = 9.0,
                NextRefractiveIndex = mediumRefractiveIndex,
                ApertureDiameter = 34.2,
            });
            surfaces.Add(new ComplexLens.SphericalElementSurfaceDefinition()
            {
                CurvatureRadius = -28.990,
                Thickness = 2.360,
                NextRefractiveIndex = 1.603,
                ApertureDiameter = 34.0,
            });
            surfaces.Add(new ComplexLens.SphericalElementSurfaceDefinition()
            {
                CurvatureRadius = 81.540,
                Thickness = 12.130,
                NextRefractiveIndex = 1.658,
                ApertureDiameter = 40.0,
            });
            surfaces.Add(new ComplexLens.SphericalElementSurfaceDefinition()
            {
                CurvatureRadius = -40.770,
                Thickness = 0.380,
                NextRefractiveIndex = mediumRefractiveIndex,
                ApertureDiameter = 40.0,
            });
            surfaces.Add(new ComplexLens.SphericalElementSurfaceDefinition()
            {
                CurvatureRadius = 874.130,
                Thickness = 6.440,
                NextRefractiveIndex = 1.717,
                ApertureDiameter = 40.0,
            });
            surfaces.Add(new ComplexLens.SphericalElementSurfaceDefinition()
            {
                CurvatureRadius = -79.460,
                Thickness = 72.228, // distance to senzor
                NextRefractiveIndex = mediumRefractiveIndex,
                ApertureDiameter = 40.0,
            });
            return ComplexLens.Create(surfaces, mediumRefractiveIndex, scale);
        }

        #region ILens Members

        public Ray Transfer(Vector3d objectPos, Vector3d lensPos)
        {
            IList<Vector3d> intersections;
            return TransferDebug(objectPos, lensPos, out intersections, false);
        }

        internal Ray TransferDebug(Vector3d objectPos, Vector3d lensPos,
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
            Ray incomingRay = new Ray(objectPos, lensPos - objectPos);
            //Console.WriteLine("Blue, {0}, ", incomingRay.ToLine());
            //Console.WriteLine("Incoming: {0}, ", Ray.NormalizeDirection(incomingRay).ToString());
            Ray outgoingRay = new Ray(incomingRay.Origin, incomingRay.Direction);

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
            Vector3d unitSphereSample = Sampler.UniformSampleSphere(
                sample, backSurfaceSinTheta, 1);
            return backSphericalSurface.Center + backSphericalSurface.Radius * unitSphereSample;
        }

        public Vector3d GetFrontSurfaceSample(Vector2d sample)
        {
            Vector3d unitSphereSample = Sampler.UniformSampleSphere(
                sample, frontSurfaceSinTheta, 1);
            return frontSphericalSurface.Center + frontSphericalSurface.Radius * (-unitSphereSample);
        }

        #endregion

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
            /// Diameter (not radius!) of aperture in the base (XY) plane.
            /// </summary>
            public double ApertureDiameter;
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
