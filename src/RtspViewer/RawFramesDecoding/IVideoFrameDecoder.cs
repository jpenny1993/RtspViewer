using RtspClientSharp.RawFrames.Video;
using RtspViewer.RawFramesDecoding.DecodedFrames;

namespace RtspViewer.RawFramesDecoding
{
    public interface IVideoFrameDecoder
    {
        IDecodedVideoFrame Decode(RawVideoFrame rawVideoFrame);
    }
}
