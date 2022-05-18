using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.UI.Xaml.Media.Imaging;

namespace RtspViewer.WinUi.Converters
{
    /// <summary>
    /// Converts a <see cref="Bitmap"/> to a <see cref="BitmapImage"/>.
    /// The same image instance is used for each conversion to save on garbage collection of heap memory.
    /// Conversions should always be done on the UI thread due to it locking the bitmap during render.
    /// </summary>
    public sealed class BitmapConverter : IDisposable
    {
        private bool _disposedValue;
        private BitmapImage _bitmapImage;
        private MemoryStream _memoryStream;

        public BitmapConverter(BitmapImage bitmapImage)
        {
            _bitmapImage = bitmapImage;
            _memoryStream = new MemoryStream();
        }

        public BitmapImage Convert(Bitmap bitmap)
        {
            if (_disposedValue || _bitmapImage?.DispatcherQueue is null)
                return _bitmapImage;
            _memoryStream.Seek(0, SeekOrigin.Begin);
            _memoryStream.SetLength(0);
            bitmap.Save(_memoryStream, ImageFormat.Jpeg);
            _memoryStream.Seek(0, SeekOrigin.Begin);
            _bitmapImage.SetSource(_memoryStream.AsRandomAccessStream());
            return _bitmapImage;
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _memoryStream.Dispose();
                    _memoryStream = null;
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
