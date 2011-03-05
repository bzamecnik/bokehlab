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

        private uint ChannelsCount { get; set; }
        public Endianness Endianness { get; private set; }

        public PFMImage(uint width, uint height) :
            this(width, height, PixelFormat.RGB)
        { }

        public PFMImage(uint width, uint height, PixelFormat pixelFormat) :
            this(width, height, pixelFormat, 1.0f)
        { }

        public PFMImage(uint width, uint height, PixelFormat pixelFormat, float scale)
        {
            Width = width;
            Height = height;
            PixelFormat = pixelFormat;
            ChannelsCount = GetChannelsCount(pixelFormat);
            Scale = scale;
            Image = new float[Width, Height, ChannelsCount];
        }

        public static PFMImage LoadImage(string filename) {
            FileStream fs = new FileStream(filename, FileMode.Open);

            // read header
            PFMImage image = ReadHeader(fs);

            // read the image data
            const int floatByteCount = 4;
            byte[] rawIntensity = new byte[floatByteCount];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    for (int band = 0; band < image.ChannelsCount; band++)
                    {
                        // read one float
                        if (fs.Read(rawIntensity, 0, floatByteCount) < 0)
                        {
                            throw new OutOfMemoryException();
                        }
                        if (image.Endianness == Endianness.BigEndian)
                        {
                            // swap the bytes to little endian
                            byte tmp = rawIntensity[0];
                            rawIntensity[0] = rawIntensity[3];
                            rawIntensity[3] = tmp;

                            tmp = rawIntensity[1];
                            rawIntensity[1] = rawIntensity[2];
                            rawIntensity[2] = tmp;
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
            StreamReader reader = new StreamReader(fs);
            string line = reader.ReadLine();
            if (line.Length < 2)
            {
                throw new OutOfMemoryException("Header is missing.");
            }
            PixelFormat pixelFormat;
            if (line[0] != 'P')
            {
                throw new OutOfMemoryException("Bad header: image signature.");
            }
            switch (line[1])
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

            line = reader.ReadLine();

            string[] dimensions = line.Split(new char[] { ' ', '\t', '\n', '\f' }, 2);
            if ((dimensions == null) || (dimensions.Length != 2))
            {
                throw new OutOfMemoryException("Bad header: image dimensions");
            }
            uint width;
            if (!uint.TryParse(dimensions[0], out width))
            {
                throw new OutOfMemoryException("Bad header: image width");
            }
            uint height;
            if (!uint.TryParse(dimensions[1], out height))
            {
                throw new OutOfMemoryException("Bad header: image height");
            }

            // image endianness and scale
            line = reader.ReadLine();
            //reader.Dispose();

            float scale;
            if (!float.TryParse(line, NumberStyles.Float, CultureInfo.CreateSpecificCulture("en-US"), out scale))
            {
                throw new OutOfMemoryException("Bad header: image endianness and scale");
            }

            Endianness endianness = (scale < 0) ? Endianness.LittleEndian : Endianness.BigEndian;
            scale = Math.Abs(scale);

            return new PFMImage(width, height, pixelFormat, scale);
        }

        public static void SaveImage(string filename, PFMImage image, Endianness endianness)
        {

        }

        private static uint GetChannelsCount(PixelFormat pixelFormat)
        {
            switch (pixelFormat) {
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
