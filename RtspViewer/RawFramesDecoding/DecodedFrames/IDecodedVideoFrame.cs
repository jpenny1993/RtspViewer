using System;

namespace RtspViewer.RawFramesDecoding.DecodedFrames
{
    public interface IDecodedVideoFrame
    {
        DateTime Timestamp { get; }
        int Width { get; }
        int Height { get; }
        void TransformTo(IntPtr buffer, int bufferStride, TransformParameters transformParameters);
    }
}