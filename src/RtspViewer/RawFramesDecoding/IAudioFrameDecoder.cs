using RtspClientSharp.RawFrames.Audio;
using RtspViewer.RawFramesDecoding.DecodedFrames;

namespace RtspViewer.RawFramesDecoding
{
    public interface IAudioFrameDecoder
    {
        IDecodedAudioFrame Decode(RawAudioFrame rawAudioFrame);
    }
}
