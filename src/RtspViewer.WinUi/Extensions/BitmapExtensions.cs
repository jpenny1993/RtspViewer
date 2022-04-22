using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace RtspViewer.WinUi.Extensions
{
    public static class BitmapExtensions
    {
        public static Task<BitmapImage> ToBitmapImageAsync(this Bitmap bitmap)
        {
            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Bmp);
            return GetBitmapImageAsync(ms.ToArray());
        }

        private static async Task<BitmapImage> GetBitmapImageAsync(byte[] data)
        {
            if (data is null)
            {
                return null;
            }

            var image = new BitmapImage();
            using (var stream = new InMemoryRandomAccessStream())
            {
                using (var writer = new DataWriter(stream.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(data);
                    await writer.StoreAsync();
                    await writer.FlushAsync();
                    writer.DetachStream();
                }

                stream.Seek(0);
                image.SetSource(stream);
            }
            return image;
        }
    }
}
