using System;

namespace RtspViewer
{
    public static class LockedFrame
    {
        public static LockedFrame<TFrame> Create<TFrame>(TFrame decodedFrame, Action releaseDecoderCallback)
        {
            return new LockedFrame<TFrame>(decodedFrame, releaseDecoderCallback);
        }
    }

    public sealed class LockedFrame<TFrame>
    {
        public LockedFrame(TFrame decodedFrame, Action releaseDecoderCallback)
        {
            DecodedFrame = decodedFrame;
            Release = releaseDecoderCallback;
        }

        public TFrame DecodedFrame { get; }
        public Action Release { get; }
    }
}
