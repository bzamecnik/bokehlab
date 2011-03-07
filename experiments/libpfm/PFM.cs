using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Drawing;
using System.Drawing.Imaging;
using mathHelper;

namespace libpfm
{
    public class PFMImage
    {
        public float[, ,] Image { get; private set; }

        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public PixelFormat PixelFormat { get; private set; }
        public float Scale { get; private set; }

        public uint ChannelsCount { get; set; }
        public Endianness Endianness { get; private set; }

        private static readonly int FLOAT_BYTE_COUNT = 4;
        private static readonly char[] WHITESPACE = new char[] { '\n', '\r', ' ', '\t', '\f' };

        public PFMImage(uint width, uint height) :
            this(width, height, PixelFormat.RGB)
        { }

        public PFMImage(uint width, uint height, PixelFormat pixelFormat) :
            this(width, height, pixelFormat, 1.0f, Endianness.LittleEndian)
        { }

        public PFMImage(uint width, uint height, PixelFormat pixelFormat, float scale, Endianness endianness)
        {
            Width = width;
            Height = height;
            PixelFormat = pixelFormat;
            ChannelsCount = GetChannelsCount(pixelFormat);
            Scale = scale;
            Endianness = endianness;
            Image = new float[Width, Height, ChannelsCount];
        }

        public static PFMImage LoadImage(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);

            // read the header
            PFMImage image = ReadHeader(fs);

            // read the hdrImage data
            byte[] rawIntensity = new byte[FLOAT_BYTE_COUNT];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    for (int band = 0; band < image.ChannelsCount; band++)
                    {
                        // read one float
                        if (fs.Read(rawIntensity, 0, FLOAT_BYTE_COUNT) != FLOAT_BYTE_COUNT)
                        {
                            throw new OutOfMemoryException(String.Format(
                                "Cannot read float at: [{0}, {1}], channel {2}", x, y, band));
                        }
                        if (image.Endianness == Endianness.BigEndian)
                        {
                            // swap the bytes to little endian
                            SwapFloatEndianness(ref rawIntensity);
                        }
                        float intensity = BitConverter.ToSingle(rawIntensity, 0);
                        image.Image[x, y, band] = intensity;
                    }
                }
            }

            fs.Close();

            return image;
        }

        private static PFMImage ReadHeader(FileStream fs)
        {
            // read signature - pixel format
            string token = ReadToken(fs);
            if ((token.Length != 2) || (token[0] != 'P'))
            {
                throw new OutOfMemoryException("Bad header: hdrImage signature.");
            }
            PixelFormat pixelFormat;
            switch (token[1])
            {
                case 'F':
                    pixelFormat = PixelFormat.RGB;
                    break;
                case 'f':
                    pixelFormat = PixelFormat.Greyscale;
                    break;
                default:
                    throw new OutOfMemoryException("Bad header: pixel format.");
            }

            // hdrImage dimensions - width, height

            token = ReadToken(fs);

            uint width;
            if (!uint.TryParse(token, out width))
            {
                throw new OutOfMemoryException("Bad header: hdrImage width");
            }
            token = ReadToken(fs);
            uint height;
            if (!uint.TryParse(token, out height))
            {
                throw new OutOfMemoryException("Bad header: hdrImage height");
            }

            // hdrImage endianness and scale
            token = ReadToken(fs);
            //reader.Dispose();

            float scale;
            if (!float.TryParse(token, NumberStyles.Float, CultureInfo.CreateSpecificCulture("en-US"), out scale))
            {
                throw new OutOfMemoryException("Bad header: hdrImage endianness and scale");
            }

            Endianness endianness = (scale < 0) ? Endianness.LittleEndian : Endianness.BigEndian;
            scale = Math.Abs(scale);

            return new PFMImage(width, height, pixelFormat, scale, endianness);
        }

        private static string ReadToken(FileStream fs)
        {
            int currentByte;
            StringBuilder token = new StringBuilder();
            while (true) {
                currentByte = fs.ReadByte();
                if (currentByte < 0)
                {
                    //throw new OutOfMemoryException("Premature end of stream.");
                    return token.ToString();
                }
                bool isWhitespace = WHITESPACE.Contains((char)currentByte);
                if (isWhitespace)
                {
                    return token.ToString();
                }
                else
                {
                    token.Append((char)currentByte);
                }
            }
        }

        public void SaveImage(string filename)
        {
            SaveImage(filename, Endianness);
        }

        public void SaveImage(string filename, Endianness endianness)
        {
            FileStream fs = new FileStream(filename, FileMode.Create);

            // write the header
            SaveHeader(fs, this);

            // write the hdrImage data
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int band = 0; band < ChannelsCount; band++)
                    {
                        float intensity = Image[x, y, band];
                        byte[] rawIntensity = BitConverter.GetBytes(intensity);
                        if (endianness == Endianness.BigEndian)
                        {
                            // swap the bytes from little to big endian
                            SwapFloatEndianness(ref rawIntensity);
                        }
                        // write one float
                        fs.Write(rawIntensity, 0, rawIntensity.Length);
                    }
                }
            }

            fs.Flush();
            fs.Close();
        }

        private static void SaveHeader(FileStream fs, PFMImage image)
        {
            StreamWriter writer = new StreamWriter(fs);

            // write PFM signature with information on the number of color channels
            // RGB, greyscale
            string signature;
            switch (image.PixelFormat)
            {
                case PixelFormat.RGB:
                    signature = "PF";
                    break;
                case PixelFormat.Greyscale:
                    signature = "Pf";
                    break;
                default:
                    throw new ArgumentException(String.Format("Unsupported pixel format: {0}", image.PixelFormat));
            }
            writer.Write("{0}\n", signature);

            // write hdrImage dimensions - width, height
            writer.Write("{0} {1}\n", image.Width, image.Height);

            // write hdrImage scale and endianness
            float scale = image.Scale;
            if (image.Endianness == Endianness.LittleEndian)
            {
                scale = -scale;
            }
            writer.Write(String.Format(CultureInfo.CreateSpecificCulture("en-US"), "{0:F6}\n", scale));
            writer.Flush();
        }

        private static void SwapFloatEndianness(ref byte[] rawIntensity)
        {
            byte tmp = rawIntensity[0];
            rawIntensity[0] = rawIntensity[3];
            rawIntensity[3] = tmp;

            tmp = rawIntensity[1];
            rawIntensity[1] = rawIntensity[2];
            rawIntensity[2] = tmp;
        }

        private static uint GetChannelsCount(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.RGB:
                    return 3;
                case PixelFormat.Greyscale:
                    return 1;
                default:
                    throw new ArgumentException(String.Format("Unsupported pixel format: {0}", pixelFormat));
            }
        }

        public Bitmap ToLdr()
        {
            return ToLdr(false, 1.0f, 0.0f);
        }

        public Bitmap ToLdr(bool tonemappingEnabled, float scale, float shift)
        {
            int width = (int)Width;
            int height = (int)Height;
            
            Bitmap outputImage = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            int bands = (int)ChannelsCount;
            int maxBand = bands - 1;

            float minValue = float.MaxValue;
            float maxValue = float.MinValue;
            if (tonemappingEnabled)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        for (int band = maxBand; band >= 0; band--)
                        {
                            float value = Image[x, y, band];
                            minValue = Math.Min(minValue, value);
                            maxValue = Math.Max(maxValue, value);
                        }
                    }
                }
            }

            BitmapData outputData = outputImage.LockBits(new Rectangle(0, 0, width, height),
               ImageLockMode.ReadOnly, outputImage.PixelFormat);
            unsafe
            {
                bool isGreyscale = (PixelFormat == PixelFormat.Greyscale);
                float scaleRangeInv = 1.0f / (maxValue - minValue); // for tone-mapping
                for (int y = 0; y < Height; y++)
                {
                    byte* outputRow = (byte*)outputData.Scan0 + (y * outputData.Stride);
                    for (int x = 0; x < Width; x++)
                    {
                        for (int band = 2; band >= 0; band--)
                        {
                            int hdrBand = (isGreyscale) ? 0 : maxBand - band;
                            // translate RGB input image to BGR output image
                            float intensity = Image[x, y, hdrBand];
                            intensity = (intensity + shift) * scale;
                            if (tonemappingEnabled)
                            {
                                // do a simple tone-mapping - linear scaling
                                // from [min; max] to [0.0; 1.0]
                                intensity = (intensity - minValue) * scaleRangeInv;
                            }
                            outputRow[x * 3 + band] = (byte)MathHelper.Clamp(intensity * 255.0f, 0.0f, 255.0f);
                        }
                    }
                }
            }
            outputImage.UnlockBits(outputData);

            return outputImage;
        }

        public static PFMImage FromLdr(System.Drawing.Bitmap ldrImage)
        {
            Bitmap inputImage = ldrImage;
            int width = ldrImage.Width;
            int height = ldrImage.Height;
            PixelFormat pixelFormat;
            switch (ldrImage.PixelFormat) {
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    inputImage = ldrImage.Clone(new Rectangle(0, 0, width, height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    pixelFormat = PixelFormat.RGB;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    pixelFormat = PixelFormat.RGB;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                    pixelFormat = PixelFormat.Greyscale;
                    break;
                default:
                    throw new ArgumentException(String.Format("Unsupported input LDR image pixel format: {0}", ldrImage.PixelFormat));
            }
            
            PFMImage hdrImage = new PFMImage((uint)width, (uint)height, pixelFormat);

            BitmapData inputData = inputImage.LockBits(new Rectangle(0, 0, width, height),
               ImageLockMode.ReadOnly, inputImage.PixelFormat);
            float conversionFactor = 1 / 255.0f;
            unsafe
            {
                bool isGreyscale = (hdrImage.PixelFormat == PixelFormat.Greyscale);
                int bands = (int)hdrImage.ChannelsCount;
                int maxBand = bands - 1;
                for (int y = 0; y < height; y++)
                {
                    byte* inputRow = (byte*)inputData.Scan0 + (y * inputData.Stride);
                    for (int x = 0; x < width; x++)
                    {
                        for (int band = maxBand; band >= 0; band--)
                        {
                            int hdrBand = (isGreyscale) ? 0 : maxBand - band;
                            // translate BGR input image to RGB output image in case of a RGB input image
                            hdrImage.Image[x, y, hdrBand] = inputRow[x * bands + band] * conversionFactor;
                        }
                    }
                }
            }
            inputImage.UnlockBits(inputData);
            return hdrImage;
        }
    }

    public enum PixelFormat
    {
        RGB,
        Greyscale,
    }

    public enum Endianness
    {
        BigEndian,
        LittleEndian,
    }
}
