using System;
using RtspViewer.RawFramesDecoding.DecodedFrames;

namespace RtspViewer
{
    public interface IVideoSource
    {
        event EventHandler<IDecodedVideoFrame> FrameReceived;
    }
}