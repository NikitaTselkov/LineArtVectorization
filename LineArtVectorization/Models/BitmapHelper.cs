using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media;

namespace LineArtVectorization.Models
{
    public static class BitmapHelper
    {
        public static BitmapSource BinaryPixelsArrayToBitmapSource(byte[,] pixels)
        {
            int height = pixels.GetLength(0);
            int width = pixels.GetLength(1);
            int stride = (width + 7) / 8; // Round up to next multiple of 8

            byte[] pixelData = new byte[height * stride];

            for (int y = 0; y < height; y++)
            {
                int byteIndex = y * stride;
                for (int x = 0; x < width; x++)
                {
                    int bitIndex = x % 8;
                    byte pixelByte = 0;

                    if (pixels[y, x] == 1)
                    {
                        pixelByte |= (byte)(1 << (7 - bitIndex));
                    }

                    pixelData[byteIndex + x / 8] |= pixelByte;
                }
            }

            return BitmapSource.Create(width, height, 96, 96,
              PixelFormats.Indexed1,
              new BitmapPalette(new List<Color>() { Colors.White, Colors.Black }),
              pixelData, stride);
        } 

        public static byte[,] BitmapSourceToBlackAndWhitePixelsArray(BitmapSource bitmapSource, int threshold)
        {
            int width = bitmapSource.PixelWidth;
            int height = bitmapSource.PixelHeight;
            int stride = (width * bitmapSource.Format.BitsPerPixel + 7) / 8;

            byte[] pixelData = new byte[height * stride];
            byte[,] pixels = new byte[height, width];

            bitmapSource.CopyPixels(new Int32Rect(0, 0, width, height), pixelData, stride, 0);

            Parallel.For(0, height, y =>
            {
                int rowOffset = y * stride;
                for (int x = 0; x < width; x++)
                {
                    int pixelOffset = rowOffset + x * 4;

                    byte blue = pixelData[pixelOffset];
                    byte green = pixelData[pixelOffset + 1];
                    byte red = pixelData[pixelOffset + 2];

                    byte newColor = ((red + green + blue) / 3) <= threshold ? (byte)1 : (byte)0;

                    pixels[y, x] = newColor;
                }
            });

            return pixels;
        }
    }

}
