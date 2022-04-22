using System;
using RtspViewer.RawFramesDecoding;

namespace RtspViewer
{
    public interface IDecodedVideoFrame
    {
        DateTime Timestamp { get; }
        int Width { get; }
        int Height { get; }
        void TransformTo(IntPtr buffer, int bufferStride, TransformParameters transformParameters);
    }
}