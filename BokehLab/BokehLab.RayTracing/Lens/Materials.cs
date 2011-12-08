namespace BokehLab.RayTracing.Lens
{
    using System;

    /// <summary>
    /// Relative refractive indices for various media.
    /// </summary>
    /// <remarks>
    /// Relative refractive index:
    /// velocity of light in a vacuum / velocity of light in medium.
    /// Following refractive indexes are for yellow light of a sodium source
    /// (with wavelength of 589.29 nanometers).
    /// Source: http://en.wikipedia.org/wiki/Refractive_index
    /// Source of dispersion data: http://refractiveindex.info/
    /// </remarks>
    public class Materials
    {
        public static class Fixed
        {
            public static readonly double VACUUM = 1.0;

            public static readonly double AIR = 1.00027715;

            /// <summary>
            /// Flourite crown glass FK51A.
            /// </summary>
            public static readonly double GLASS_CROWN_FK51A = 1.48651;
            /// <summary>
            /// Crown glass K7 (K7 by SCHOTT).
            /// </summary>
            public static readonly double GLASS_CROWN_K7 = 1.51105;
            /// <summary>
            /// Borosilicate crown glass BK7 (N-BK7 by SCHOTT).
            /// </summary>
            public static readonly double GLASS_CROWN_BK7 = 1.51673;
            /// <summary>
            /// Flint glasss F2 (F2 by SCHOTT).
            /// </summary>
            public static readonly double GLASS_FLINT_F2 = 1.61989;
            /// <summary>
            /// Dense flint glasss SF10 (N-SF10 by SCHOTT).
            /// </summary>
            public static readonly double GLASS_FLINT_SF10 = 1.72806;

            /// <summary>
            /// Acrylic glass (Polymethyl methacrilate).
            /// </summary>
            public static readonly double PLASTIC_PMMA_ = 1.4913;
        }

        public static class Dispersible
        {

            // formula: n = 1 + C1/(C2-lambda^(-2)) + C3/(C4-lambda^(-2))
            public static readonly DispersionRecord AIR = new DispersionRecord()
            {
                C1 = 5792105E-8,
                C2 = 238.0185,
                C3 = 167917E-8,
                C4 = 57.362
            };

            /// <summary>
            /// Borosilicate crown glass BK7 (N-BK7 by SCHOTT).
            /// </summary>
            public static readonly DispersionRecord GLASS_CROWN_BK7 = new DispersionRecord()
            {
                C1 = 1.03961212,
                C2 = 0.00600069867,
                C3 = 0.231792344,
                C4 = 0.0200179144,
                C5 = 1.01046945,
                C6 = 103.560653
            };

            /// <summary>
            /// Flourite crown glass FK51A.
            /// </summary>
            public static readonly DispersionRecord GLASS_CROWN_FK51A = new DispersionRecord()
            {
                C1 = 0.971247817,
                C2 = 0.00472301995,
                C3 = 0.216901417,
                C4 = 0.0153575612,
                C5 = 0.904651666,
                C6 = 168.68133
            };

            /// <summary>
            /// Dense flint glasss SF10 (N-SF10 by SCHOTT).
            /// </summary>
            public static readonly DispersionRecord GLASS_CROWN_SF10 = new DispersionRecord()
            {
                C1 = 1.62153902,
                C2 = 0.0122241457,
                C3 = 0.256287842,
                C4 = 0.0595736775,
                C5 = 1.64447552,
                C6 = 147.468793
            };

            /// <summary>
            /// Computes the refractive index of a material for given wavelength
            /// with respect to dispersion.
            /// </summary>
            /// <remarks>
            /// Dispersion formula is the Sellmeier equation,
            /// http://en.wikipedia.org/wiki/Sellmeier_equation.
            /// </remarks>
            /// <param name="wavelength">Wavelength of the light (in micrometers).</param>
            /// <param name="material">Description of the material.</param>
            /// <returns>Refractive index of the material at given wavelegth.</returns>
            public static double DispersedRefractiveIndex(
                DispersionRecord material,
                double wavelength)
            {
                double lambdaSqr = Math.Pow(wavelength, 2);
                double refractiveIndex = 1
                    + material.C1 * lambdaSqr / (lambdaSqr - material.C2)
                    + material.C3 * lambdaSqr / (lambdaSqr - material.C4);
                if ((material.C5 != 0) && (material.C6 != 0))
                {
                    refractiveIndex += material.C5 * lambdaSqr / (lambdaSqr - material.C6);
                }
                refractiveIndex = Math.Sqrt(refractiveIndex);
                return refractiveIndex;
            }

            public struct DispersionRecord
            {
                public double C1;
                public double C2;
                public double C3;
                public double C4;
                public double C5;
                public double C6;
            }
        }
    }
}
