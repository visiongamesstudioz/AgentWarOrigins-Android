using System.IO;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
    public static class MB_TGAWriter
    {
        public static void Write(Color[] pixels, int width, int height, string path)
        {
            if (File.Exists(path))
                File.Delete(path);
            FileStream fileStream = File.Create(path);
            Write(pixels, width, height, (Stream)fileStream);
        }

        public static void Write(Color[] pixels, int width, int height, Stream output)
        {
            byte[] buffer1 = new byte[pixels.Length * 4];
            int index1 = 0;
            int index2 = 0;
            for (int index3 = 0; index3 < height; ++index3)
            {
                for (int index4 = 0; index4 < width; ++index4)
                {
                    Color pixel = pixels[index1];
                    buffer1[index2] = (byte)(pixel.b * (double)byte.MaxValue);
                    buffer1[index2 + 1] = (byte)(pixel.g * (double)byte.MaxValue);
                    buffer1[index2 + 2] = (byte)(pixel.r * (double)byte.MaxValue);
                    buffer1[index2 + 3] = (byte)(pixel.a * (double)byte.MaxValue);
                    ++index1;
                    index2 += 4;
                }
            }
            byte[] numArray = new byte[18];
            numArray[2] = (byte)2;
            numArray[12] = (byte)(width & (int)byte.MaxValue);
            numArray[13] = (byte)((width & 65280) >> 8);
            numArray[14] = (byte)(height & (int)byte.MaxValue);
            numArray[15] = (byte)((height & 65280) >> 8);
            numArray[16] = (byte)32;
            byte[] buffer2 = numArray;
            using (BinaryWriter binaryWriter = new BinaryWriter(output))
            {
                binaryWriter.Write(buffer2);
                binaryWriter.Write(buffer1);
            }
        }
    }
}
