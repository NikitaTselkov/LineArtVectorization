using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Runtime.ConstrainedExecution;
using System.Reflection;
using System.Windows.Markup;
using System.Runtime.InteropServices;

namespace LineArtVectorization.Models
{
    public static class BitmapHelper
    {
        public static byte[] ConvertBitmapImageToByteArray(BitmapSource bitmapSource)
        {
            byte[] data;
            PngBitmapEncoder encoder = new();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            using (MemoryStream ms = new())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }

            return data;
        }

        public static byte[] BitmapSourceToPixelsArray(BitmapSource bitmapSource)
        {
            // Stride = (width) x (bytes per pixel)
            byte bytesPerPixel = 4;
            int stride = (int)bitmapSource.PixelWidth * bytesPerPixel;
            byte[] pixels = new byte[(int)bitmapSource.PixelHeight * stride];
            byte[] result = new byte[pixels.Length / bytesPerPixel];

            bitmapSource.CopyPixels(pixels, stride, 0);

            for (int i = 0; i < pixels.Length; i+= bytesPerPixel)
            {
                if(i == 0) result[i] = pixels[i];
                else result[i / bytesPerPixel] = pixels[i];
            }

            return result;
        }

        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        public static Bitmap ConvertFormatToBlackAndWhite(BitmapSource bitmap, int P)
        {
            Bitmap result = new((int)bitmap.Width, (int)bitmap.Height);
            var color = new System.Windows.Media.Color();

            for (int j = 0; j < bitmap.Height - 1; j++)
            {
                for (int i = 0; i < bitmap.Width - 1; i++)
                {
                    color = GetPixelColor(bitmap, i, j);
                    int K = (color.R + color.G + color.B) / 3;
                    result.SetPixel(i, j, K <= P ? System.Drawing.Color.Black : System.Drawing.Color.White);
                }
            }

            return result;
        }

        private static System.Windows.Media.Color GetPixelColor(BitmapSource bitmap, int x, int y)
        {
            System.Windows.Media.Color color;
            var bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;
            var bytes = new byte[bytesPerPixel];
            var rect = new Int32Rect(x, y, 1, 1);

            bitmap.CopyPixels(rect, bytes, bytesPerPixel, 0);

            if (bitmap.Format == PixelFormats.Bgra32)
            {
                color = System.Windows.Media.Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
            }
            else if (bitmap.Format == PixelFormats.Bgr32)
            {
                color = System.Windows.Media.Color.FromRgb(bytes[2], bytes[1], bytes[0]);
            }
            else
            {
                color = Colors.Black;
            }

            return color;
        }
    }
}
