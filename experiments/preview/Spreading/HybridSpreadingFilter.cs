using System;
using System.Diagnostics;
using System.Drawing;
using BokehLab.FloatMap;
using BokehLab.Spreading.PSF.Perimeter;

namespace BokehLab.Spreading
{
    public class HybridSpreadingFilter : AbstractSpreadingFilter
    {
        private static readonly float DEFAULT_CRITERION_THRESHOLD = 0.25f;

        /// <summary>
        /// Filters ordered by quality. Higher index means higher quality.
        /// </summary>
        private AbstractSpreadingFilter[] Filters { get; set; }
        private int activeFilterIndex;
        private FilterSelectionCriterion criterion;

        public int MaxRadiusForQualityFilter
        {
            get { return criterion.MaxRadiusForQualityFilter; }
            set { criterion.MaxRadiusForQualityFilter = value; }
        }

        public HybridSpreadingFilter(AbstractSpreadingFilter rectangleFilter, AbstractSpreadingFilter perimeterFilter)
        {
            Filters = new AbstractSpreadingFilter[] {
                rectangleFilter,
                perimeterFilter,
            };
            activeFilterIndex = 0;
            criterion = new FilterSelectionCriterion();
        }

        internal override void SpreadPSF(int x, int y, int radius, float weight, float[, ,] origImage, float[, ,] spreadingImage, float[, ,] normalizationImage, int tableWidth, int tableHeight, uint bands)
        {
            // TODO:
            // - probably also spread PSF with radius higher than some limit
            //   with the rectangle or polynomial spreading filter
            //   - higher quality PSFs could be too costly or might not have
            //     rasterized the patterns with too high radius

            int filterIndex = activeFilterIndex;

            if (criterion.SelectFilter(x, y, radius) == activeFilterIndex)
            {
                Filters[activeFilterIndex].SpreadPSF(x, y, radius, weight, origImage, spreadingImage, normalizationImage, tableWidth, tableHeight, bands);
            }
        }

        protected override void Filter(FloatMapImage inputImage, FloatMapImage spreadingTable, FloatMapImage normalizationTable)
        {
            criterion.OriginalImage = inputImage;
            criterion.OriginalImageSAT = inputImage.Integrate();

            // spread rectangles
            activeFilterIndex = 0;
            Spread(inputImage, spreadingTable, normalizationTable);
            IntegrateVertically(spreadingTable, normalizationTable);

            // spread perimeter PSFs
            activeFilterIndex = 1;
            Spread(inputImage, spreadingTable, normalizationTable);
            IntegrateHorizontally(spreadingTable, normalizationTable);
        }

        internal class FilterSelectionCriterion {
            public FloatMapImage OriginalImage { get; set; }
            
            /// <summary>
            /// Summed area table of the original image.
            /// Useful for quick computing of an avergave intensity over a rectangle.
            /// </summary>
            public FloatMapImage OriginalImageSAT { get; set; }

            /// <summary>
            /// Threshold for cutting the contrast metric.
            /// </summary>
            /// <remarks>
            /// Values greater that the threshold will select the high-quality
            /// filter, lower values will select the low-quality filter.
            /// TODO: The threshold could be dependent on the radius:
            /// smaller radius -> smaller threshold.
            /// </remarks>
            public float Threshold { get; set; }

            public int MaxRadiusForQualityFilter { get; set; }

            //private int FilterCount { get; set; }

            public FilterSelectionCriterion()
            {
                Threshold = DEFAULT_CRITERION_THRESHOLD;
            }

            /// <summary>
            /// Selects the proper filter for a given pixel in the original
            /// image based on local contrast.
            /// </summary>
            /// <remarks>
            /// For high contrast pixels it selects a high-quality filter,
            /// for the rest a low quality filter.
            /// The local contrast metric is thresholded difference between
            /// current pixel intensity and average intensity of its neighborhood.
            /// This is similar to thresholding a edge-detected image.
            /// </remarks>
            /// <param name="x">pixel coordinate X</param>
            /// <param name="y">pixel coordinate Y</param>
            /// <returns>Chosen spreading filter index</returns>
            public int SelectFilter(int x, int y, int psfRadius) {
                if (psfRadius > MaxRadiusForQualityFilter)
                {
                    return 0;
                }

                float[, ,] sat = OriginalImageSAT.Image;
                int width = (int)OriginalImageSAT.Width;
                int height = (int)OriginalImageSAT.Height;
                // TODO:
                // - use all RGB values (probably sum of them), not just red
                // - better use brightness (weighted sum)
                
                float sourceIntensity = OriginalImage.Image[x, y, 0];

                int left = MathHelper.Clamp<int>(x - psfRadius - 1, 0, width - 1);
                int right = MathHelper.Clamp<int>(x + psfRadius, 0, width - 1);
                int top = MathHelper.Clamp<int>(y - psfRadius - 1, 0, height - 1);
                int bottom = MathHelper.Clamp<int>(y + psfRadius, 0, height - 1);

                float psfArea = (right - left) * (bottom - top);
                float psfSum = sat[right, bottom, 0] + sat[left, top, 0]
                    - sat[left, bottom, 0] - sat[right, top, 0];
                // average over neighborhood of the current pixel within the PSF radius
                // (except the current pixel itself)
                // - it is equivalent to make gathering-blurred image
                float averageOverPsf = (psfSum - sourceIntensity) / (psfArea - 1);
                //float averageOverPsf = psfSum / psfArea;
                //return (sourceIntensity > averageOverPsf) ? (FilterCount - 1) : 0;
                
                // thresholding the absolute difference [Kosloff08]
                //return (Math.Abs(sourceIntensity - averageOverPsf) > Threshold) ? (FilterCount - 1) : 0;
                // thresholding the difference
                // - seems to be better only to prefer place where the pixel is considerably
                //   lighter that its neighrborhood
                return (sourceIntensity - averageOverPsf > Threshold) ? 1 : 0;
            }
        }
    }
}
