using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using RtspViewer.RawFramesDecoding;

namespace RtspViewer
{
    public class FrameTransformer : IFrameTransformer
    {
        private Size _pictureSize;

        public FrameTransformer() : this(1280, 720)
        {
        }

        public FrameTransformer(int pictureWidth, int pictureHeight)
        {
            _pictureSize = new Size(pictureWidth, pictureHeight);
        }

        public Bitmap TransformToBitmap(IDecodedVideoFrame decodedFrame)
        {
            var managedArray = TransformFrame(decodedFrame, _pictureSize);
            var im = CopyDataToBitmap(managedArray, _pictureSize);
            return im;
        }

        public byte[] TransformToBytes(IDecodedVideoFrame decodedFrame)
        {
            return TransformFrame(decodedFrame, _pictureSize);
        }

        public void UpdateFrameSize(int width, int height)
        {
            _pictureSize = new Size(width, height);
        }

        private static byte[] TransformFrame(IDecodedVideoFrame decodedFrame, Size pictureSize)
        {
            var transformParameters = new TransformParameters(
              RectangleF.Empty,
              pictureSize,
              ScalingPolicy.Stretch, PixelFormats.Bgra32, ScalingQuality.FastBilinear);

            var pictureArraySize = pictureSize.Width * pictureSize.Height * 4;
            var unmanagedPointer = Marshal.AllocHGlobal(pictureArraySize);

            decodedFrame.TransformTo(unmanagedPointer, pictureSize.Width * 4, transformParameters);
            var managedArray = new byte[pictureArraySize];
            Marshal.Copy(unmanagedPointer, managedArray, 0, pictureArraySize);
            Marshal.FreeHGlobal(unmanagedPointer);
            return managedArray;
        }

        private static Bitmap CopyDataToBitmap(byte[] data, Size pictureSize)
        {
            var bmp = new Bitmap(pictureSize.Width, pictureSize.Height, PixelFormat.Format32bppArgb);

            var bmpData = bmp.LockBits(
              new Rectangle(0, 0, bmp.Width, bmp.Height),
              ImageLockMode.WriteOnly, bmp.PixelFormat);

            Marshal.Copy(data, 0, bmpData.Scan0, data.Length);

            bmp.UnlockBits(bmpData);

            return bmp;
        }
    }
}
