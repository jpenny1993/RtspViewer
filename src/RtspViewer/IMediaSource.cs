using System;
using RtspViewer.Configuration;

namespace RtspViewer
{
    public interface IMediaSource : IDisposable
    {
        bool Started { get; }
        DateTime TimeStartedUtc { get; }
        DateTime? TimeStoppedUtc { get; }
        TimeSpan TimeElapsed { get; }

        EventHandler<string> StatusChanged { get; set; }
        EventHandler<LockedFrame<IDecodedAudioFrame>> AudioFrameDecoded { get; set; }
        EventHandler<LockedFrame<IDecodedVideoFrame>> VideoFrameDecoded { get; set; }

        void ConfigureStream(StreamConfiguration config);
        void Start();
        void Stop();
        void TogglePlay();
    }
}
