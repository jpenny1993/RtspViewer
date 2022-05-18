using System;
using RtspClientSharp.RawFrames.Video;

namespace RtspViewer.RawFramesDecoding
{
    public interface IVideoFrameDecoder : IDisposable
    {
        IDecodedVideoFrame Decode(RawVideoFrame rawVideoFrame);
    }
}
