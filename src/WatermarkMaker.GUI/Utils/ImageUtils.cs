using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace WatermarkMaker.Utils
{
    internal static class ImageUtils
    {
        public static BitmapImage ConvertToBitmap(Image source)
        {
            using var memoryStream = new MemoryStream();
            source.Save(memoryStream, ImageFormat.Png);

            var image = new BitmapImage();
            image.BeginInit();
            memoryStream.Seek(0, SeekOrigin.Begin);
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = memoryStream;
            image.EndInit();

            image.Freeze();

            return image;
        }
    }
}