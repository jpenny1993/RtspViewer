using System;
using RtspViewer.RawFramesDecoding.DecodedFrames;

namespace RtspViewer
{
    public interface IAudioSource
    {
        event EventHandler<IDecodedAudioFrame> FrameReceived;
    }
}
