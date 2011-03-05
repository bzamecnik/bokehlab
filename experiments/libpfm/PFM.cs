using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace libpfm
{
    public class PFMImage
    {
        public readonly float[, ,] Image;

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

            // read the image data
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
                throw new OutOfMemoryException("Bad header: image signature.");
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

            // image dimensions - width, height

            token = ReadToken(fs);

            uint width;
            if (!uint.TryParse(token, out width))
            {
                throw new OutOfMemoryException("Bad header: image width");
            }
            token = ReadToken(fs);
            uint height;
            if (!uint.TryParse(token, out height))
            {
                throw new OutOfMemoryException("Bad header: image height");
            }

            // image endianness and scale
            token = ReadToken(fs);
            //reader.Dispose();

            float scale;
            if (!float.TryParse(token, NumberStyles.Float, CultureInfo.CreateSpecificCulture("en-US"), out scale))
            {
                throw new OutOfMemoryException("Bad header: image endianness and scale");
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

            // write the image data
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

            // write image dimensions - width, height
            writer.Write("{0} {1}\n", image.Width, image.Height);

            // write image scale and endianness
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
