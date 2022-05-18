using System;
using RtspClientSharp.RawFrames.Audio;

namespace RtspViewer.RawFramesDecoding
{
    public interface IAudioFrameDecoder : IDisposable
    {
        IDecodedAudioFrame Decode(RawAudioFrame rawAudioFrame);
    }
}
