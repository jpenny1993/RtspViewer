using System;
using System.Drawing;
using System.Drawing.Imaging;
using RtspViewer.RawFramesDecoding;
using RtspViewer.RawFramesDecoding.DecodedFrames;

namespace RtspViewer
{
    public static class BitmapExtensions
    {
        public static TransformParameters GetTransformParameters(this Bitmap bmp)
        {
            return new TransformParameters(
                   RectangleF.Empty,
                   new Size(bmp.Width, bmp.Height),
                   ScalingPolicy.Stretch,
                   PixelFormats.Bgra32,
                   ScalingQuality.FastBilinear);
        }

        public static void UpdateBitmap(this Bitmap bmp, IDecodedVideoFrame frame, TransformParameters transformParameters)
        {
            BitmapData bmpData = null;
            try
            {
                // Potentially locked by UI
                bmpData = bmp.LockBits(
                     new Rectangle(0, 0, bmp.Width, bmp.Height),
                     ImageLockMode.WriteOnly,
                     bmp.PixelFormat);

                frame.TransformTo(bmpData.Scan0, bmpData.Stride, transformParameters);
            }
            catch (InvalidOperationException) { } // Locked by UI thread
            finally
            {
                if (bmpData != null)
                    bmp.UnlockBits(bmpData);
            }
        }

        public static void UpdateBitmap(this Bitmap bmp, IDecodedVideoFrame frame)
        {
            UpdateBitmap(bmp, frame, GetTransformParameters(bmp));
        }
    }
}
