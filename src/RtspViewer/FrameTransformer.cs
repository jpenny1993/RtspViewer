using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using RtspViewer.RawFramesDecoding;

namespace RtspViewer
{
    public sealed class FrameTransformer : IFrameTransformer
    {
        private TransformParameters _transformParameters;
        private Rectangle _imageRectangle;
        private Bitmap _bitmap;
        private int _imageBufferSize;
        private int _imageBufferStride;
        private byte[] _imageBuffer;

        public FrameTransformer() : this(1280, 720)
        {
        }

        public FrameTransformer(int pictureWidth, int pictureHeight)
        {
            UpdateFrameSize(pictureWidth, pictureHeight);
        }

        public Bitmap TransformToBitmap(IDecodedVideoFrame decodedFrame)
        {
            TransformFrame(decodedFrame, _imageBuffer, _imageBufferSize, _imageBufferStride, _transformParameters);
            CopyDataToBitmap(_imageBuffer, _bitmap, _imageRectangle);
            return _bitmap;
        }

        public Bitmap UpdateFrameSize(int width, int height)
        {
            _bitmap = _bitmap is null
                ? new Bitmap(width, height, PixelFormat.Format32bppArgb)
                : ResizeBitmap(_bitmap, width, height);

            _transformParameters = new TransformParameters(
                 RectangleF.Empty,
                 _bitmap.Size,
                 ScalingPolicy.Stretch,
                 PixelFormats.Bgra32,
                 ScalingQuality.FastBilinear);

            _imageRectangle = new Rectangle(0, 0, _bitmap.Width, _bitmap.Height);

            _imageBufferStride = width * 4;
            _imageBufferSize = width * height * 4;
            _imageBuffer = new byte[_imageBufferSize];

            return _bitmap;
        }

        private static Bitmap ResizeBitmap(Bitmap bitmap, int width, int height)
        {
            var resizedBitmap = new Bitmap(width, height);
            using (var graphic = Graphics.FromImage(resizedBitmap))
            {
                graphic.DrawImage(bitmap, 0, 0, width, height);
            }
            return resizedBitmap;
        }

        private static void TransformFrame(IDecodedVideoFrame decodedFrame, byte[] buffer, int bufferLength, int bufferStride, TransformParameters transformParameters)
        {
            var unmanagedPointer = Marshal.AllocHGlobal(bufferLength);
            decodedFrame.TransformTo(unmanagedPointer, bufferStride, transformParameters);
            Marshal.Copy(unmanagedPointer, buffer, 0, bufferLength);
            Marshal.FreeHGlobal(unmanagedPointer);
        }

        private static void CopyDataToBitmap(byte[] data, Bitmap bitmap, Rectangle rectangle)
        {
            var bmpData = bitmap.LockBits(rectangle, ImageLockMode.WriteOnly, bitmap.PixelFormat);
            Marshal.Copy(data, 0, bmpData.Scan0, data.Length);
            bitmap.UnlockBits(bmpData);
        }
    }
}
