using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Drawing;
using System.Drawing.Imaging;

namespace BokehLab.FloatMap
{
    /// <summary>
    /// Represents the Portable Float Map file format and provides means for I/O.
    /// </summary>
    /// <remarks>
    /// Supports reading and writing RGB and grayscale PFM images.
    /// </remarks>
    public static class PortableFloatMap {
        private static readonly int FLOAT_BYTE_COUNT = 4;
        private static readonly char[] WHITESPACE = new char[] { '\n', '\r', ' ', '\t', '\f' };

        public static FloatMapImage LoadImage(string filename)
        {
            Endianness endianness;
            return LoadImage(filename, out endianness);
        }

        public static FloatMapImage LoadImage(string filename, out Endianness endianness)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);

            // read the header
            FloatMapImage image = ReadHeader(fs, out endianness);

            // read the image data
            byte[] rawIntensity = new byte[FLOAT_BYTE_COUNT];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    for (int band = 0; band < image.TotalChannelsCount; band++)
                    {
                        // read one float
                        if (fs.Read(rawIntensity, 0, FLOAT_BYTE_COUNT) != FLOAT_BYTE_COUNT)
                        {
                            throw new OutOfMemoryException(String.Format(
                                "Cannot read float at: [{0}, {1}], channel {2}", x, y, band));
                        }
                        if (endianness == Endianness.BigEndian)
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

        private static FloatMapImage ReadHeader(FileStream fs, out Endianness endianness)
        {
            // read signature - pixel format
            string token = ReadToken(fs);
            if ((token.Length < 2) || (token.Length > 3) || (token[0] != 'P'))
            {
                throw new OutOfMemoryException("Bad header: image signature.");
            }
            PixelFormat pixelFormat;
            switch (token)
            {
                case "PF":
                    pixelFormat = PixelFormat.RGB;
                    break;
                case "Pf":
                    pixelFormat = PixelFormat.Greyscale;
                    break;
                case "PFA":
                    pixelFormat = PixelFormat.RGBA;
                    break;
                case "PfA":
                    pixelFormat = PixelFormat.GreyscaleA;
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

            endianness = (scale < 0) ? Endianness.LittleEndian : Endianness.BigEndian;
            scale = Math.Abs(scale);

            return new FloatMapImage(width, height, pixelFormat, scale);
        }

        private static string ReadToken(FileStream fs)
        {
            int currentByte;
            StringBuilder token = new StringBuilder();
            while (true)
            {
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

        public static void SaveImage(this FloatMapImage image, string filename)
        {
            SaveImage(image, filename, Endianness.LittleEndian);
        }

        public static void SaveImage(this FloatMapImage image, string filename, Endianness endianness)
        {
            FileStream fs = new FileStream(filename, FileMode.Create);

            // write the header
            SaveHeader(fs, image, endianness);

            // write the image data
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    for (int band = 0; band < image.TotalChannelsCount; band++)
                    {
                        float intensity = image.Image[x, y, band];
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

        private static void SaveHeader(FileStream fs, FloatMapImage image, Endianness endianness)
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
                case PixelFormat.RGBA:
                    signature = "PFA";
                    break;
                case PixelFormat.GreyscaleA:
                    signature = "PfA";
                    break;
                default:
                    throw new ArgumentException(String.Format("Unsupported pixel format: {0}", image.PixelFormat));
            }
            writer.Write("{0}\n", signature);

            // write image dimensions - width, height
            writer.Write("{0} {1}\n", image.Width, image.Height);

            // write image scale and endianness
            float scale = image.Scale;
            if (endianness == Endianness.LittleEndian)
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

        public enum Endianness
        {
            BigEndian,
            LittleEndian,
        }
    }
}