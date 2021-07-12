using System;

namespace RtspViewer.RawFramesDecoding.DecodedFrames
{
    public class DecodedVideoFrame : IDecodedVideoFrame
    {
        private readonly Action<IntPtr, int, TransformParameters> _transformAction;

        public DateTime Timestamp { get; }

        public int Width { get; }

        public int Height { get; }

        public DecodedVideoFrame(int width, int height, DateTime timestamp, Action<IntPtr, int, TransformParameters> transformAction)
        {
            Width = width;
            Height = height;
            Timestamp = timestamp;
            _transformAction = transformAction;
        }

        public void TransformTo(IntPtr buffer, int bufferStride, TransformParameters transformParameters)
        {
            _transformAction(buffer, bufferStride, transformParameters);
        }
    }
}