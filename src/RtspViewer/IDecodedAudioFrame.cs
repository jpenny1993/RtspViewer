using System;
using RtspViewer.RawFramesDecoding.DecodedFrames;

namespace RtspViewer
{
    public interface IDecodedAudioFrame
    {
        DateTime Timestamp { get; }
        ArraySegment<byte> DecodedBytes { get; }
        AudioFrameFormat Format { get; }
    }
}