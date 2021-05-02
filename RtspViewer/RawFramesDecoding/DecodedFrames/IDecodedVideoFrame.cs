using System;

namespace RtspViewer.RawFramesDecoding.DecodedFrames
{
    public interface IDecodedVideoFrame
    {
        DateTime Timestamp { get; }
        void TransformTo(IntPtr buffer, int bufferStride, TransformParameters transformParameters);
    }
}