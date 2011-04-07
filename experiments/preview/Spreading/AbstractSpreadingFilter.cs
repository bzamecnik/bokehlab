using BokehLab.FloatMap;

// TODO:
// - shouldn't the table be of the same size as the input image?
//   - now it seems it has to be larger by 1px
// - should the spreading table and normalization channel be squashed into
//   one image for better locality?
// - could alpha channel be used as a normalization channel?
// - implement better PSFs:
//   - perimeter spreading
//   - polynomial spreading

namespace BokehLab.Spreading
{
    public abstract class AbstractSpreadingFilter : ISpreadingFilter
    {
        public BlurMap Blur;

        public FloatMapImage FilterImage(FloatMapImage inputImage, FloatMapImage outputImage)
        {
            if (inputImage == null) return null;

            uint width = inputImage.Width;
            uint height = inputImage.Height;

            if (width < 1 || height < 1) return null;

            if (outputImage == null)
            {
                outputImage = new FloatMapImage(width, height, inputImage.PixelFormat);
            }

            // alpha channel works as the normalization channel
            FloatMapImage spreadingTable = new FloatMapImage(width + 1, height + 1, inputImage.PixelFormat.AddAlpha());

            InitializeTables(spreadingTable, inputImage);

            // phase 1: distribute corners into the table
            Spread(inputImage, spreadingTable);

            // phase 2: accumulate the corners into rectangles
            Integrate(spreadingTable);

            Normalize(spreadingTable, outputImage);

            spreadingTable.Dispose();

            return outputImage;
        }

        private static void InitializeTables(FloatMapImage spreadingTable, FloatMapImage inputImage)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Reset();
            //sw.Start();

            //uint bands = spreadingTable.ColorChannelsCount;
            //bool hasAlpha = inputImage.PixelFormat.HasAlpha();
            uint bands = spreadingTable.TotalChannelsCount;
            
            // NOTE: the following properties are stored as an optimization
            float[, ,] spreadingImage = spreadingTable.Image;
            int tableWidth = (int)spreadingTable.Width;
            int tableHeight = (int)spreadingTable.Height;
            //uint alphaBand = bands;

            // initialize the spreading table and the normalization channel to 0.0
            for (int x = 0; x < tableWidth; x++)
            {
                for (int y = 0; y < tableHeight; y++)
                {
                    for (int band = 0; band < bands; band++)
                    {
                        spreadingImage[x, y, band] = 0;
                    }
                }
            }
            //if (hasAlpha)
            //{
            //    for (int x = 0; x < tableWidth - 1; x++)
            //    {
            //        for (int y = 0; y < tableHeight - 1; y++)
            //        {
            //            spreadingImage[x, y, alphaBand] = inputImage.Image[x, y, alphaBand];
            //        }
            //    }
            //}
            //Console.WriteLine("Initializing spreading and normalization tables: {0} ms", sw.ElapsedMilliseconds);
        }

        private void Spread(FloatMapImage inputImage, FloatMapImage spreadingTable)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Reset();
            //sw.Start();

            uint bands = inputImage.ColorChannelsCount;
            uint width = inputImage.Width;
            uint height = inputImage.Height;

            float[, ,] origImage = inputImage.Image;
            float[, ,] spreadingImage = spreadingTable.Image;

            int tableWidth = (int)spreadingTable.Width;
            int tableHeight = (int)spreadingTable.Height;

            bool hasAlpha = inputImage.PixelFormat.HasAlpha();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float radius = Blur.GetPSFRadius(x, y);

                    // spread PSFs of a non-integer radius using two weighted
                    // PSFs of integer size close to the original radius
                    int smallerRadius = (int)radius;
                    int biggerRadius = smallerRadius + 1;
                    float weight = biggerRadius - radius;

                    //SpreadPSF(x, y, smallerRadius, 1.0f, origImage, spreadingImage, tableWidth, tableHeight, colorBands);

                    SpreadPSF(x, y, smallerRadius, weight, origImage, spreadingImage, tableWidth, tableHeight, bands, hasAlpha);
                    SpreadPSF(x, y, biggerRadius, 1 - weight, origImage, spreadingImage, tableWidth, tableHeight, bands, hasAlpha);
                }
            }
            //Console.WriteLine("Phase 1, spreading: {0} ms", sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <param name="weight"></param>
        /// <param name="origImage"></param>
        /// <param name="spreadingImage"></param>
        /// <param name="tableWidth"></param>
        /// <param name="tableHeight"></param>
        /// <param name="bands">Color bands in original image.</param>
        protected abstract void SpreadPSF(int x, int y, int radius, float weight, float[, ,] origImage, float[, ,] spreadingImage, int tableWidth, int tableHeight, uint bands, bool hasAlpha);

        protected abstract void Integrate(FloatMapImage spreadingTable);

        protected static void IntegrateVertically(FloatMapImage spreadingTable)
        {
            uint bands = spreadingTable.TotalChannelsCount;
            float[, ,] spreadingImage = spreadingTable.Image;
            int tableWidth = (int)spreadingTable.Width;
            int tableHeight = (int)spreadingTable.Height;

            for (int x = 0; x < tableWidth; x++)
            {
                for (int y = 1; y < tableHeight; y++)
                {
                    for (int band = 0; band < bands; band++)
                    {
                        spreadingImage[x, y, band] += spreadingImage[x, y - 1, band];
                    }
                }
            }
        }

        protected static void IntegrateHorizontally(FloatMapImage spreadingTable)
        {
            uint bands = spreadingTable.TotalChannelsCount;
            float[, ,] spreadingImage = spreadingTable.Image;
            int tableWidth = (int)spreadingTable.Width;
            int tableHeight = (int)spreadingTable.Height;

            for (int y = 0; y < tableHeight; y++)
            {
                for (int x = 1; x < tableWidth; x++)
                {
                    for (int band = 0; band < bands; band++)
                    {
                        spreadingImage[x, y, band] += spreadingImage[x - 1, y, band];
                    }
                }
            }
        }

        private static void Normalize(FloatMapImage spreadingTable, FloatMapImage outputImage)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Reset();
            //sw.Start();

            uint bands = outputImage.ColorChannelsCount;
            uint alphaBand = bands;
            bool hasAlpha = outputImage.PixelFormat.HasAlpha();
            float[, ,] image = outputImage.Image;
            float[, ,] spreadingImage = spreadingTable.Image;
            int width = (int)outputImage.Width;
            int height = (int)outputImage.Height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float normalization = 1 / spreadingImage[x, y, alphaBand];
                    for (int band = 0; band < bands; band++)
                    {
                        image[x, y, band] = spreadingImage[x, y, band] * normalization;
                    }
                    if (hasAlpha)
                    {
                        //image[x, y, alphaBand] = spreadingImage[x, y, alphaBand];
                        image[x, y, alphaBand] = 1.0f;
                    }
                }
            }
            //Console.WriteLine("Normalizing to output image: {0} ms", sw.ElapsedMilliseconds);
        }
    }
}
