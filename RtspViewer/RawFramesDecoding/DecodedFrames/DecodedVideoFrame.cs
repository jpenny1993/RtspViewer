using System;

namespace RtspViewer.RawFramesDecoding.DecodedFrames
{
    public class DecodedVideoFrame : IDecodedVideoFrame
    {
        private readonly Action<IntPtr, int, TransformParameters> _transformAction;

        public DateTime Timestamp { get; }

        public DecodedVideoFrame(DateTime timestamp, Action<IntPtr, int, TransformParameters> transformAction)
        {
            Timestamp = timestamp;
            _transformAction = transformAction;
        }

        public void TransformTo(IntPtr buffer, int bufferStride, TransformParameters transformParameters)
        {
            _transformAction(buffer, bufferStride, transformParameters);
        }
    }
}