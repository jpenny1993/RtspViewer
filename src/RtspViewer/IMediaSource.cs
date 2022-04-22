using System;
using RtspViewer.Configuration;

namespace RtspViewer
{
    public interface IMediaSource
    {
        EventHandler<IDecodedAudioFrame> AudioFrameReceived { get; set; }
        bool Started { get; }
        EventHandler<string> StatusChanged { get; set; }
        EventHandler<IDecodedVideoFrame> VideoFrameReceived { get; set; }

        void ConfigureStream(StreamConfiguration config);
        void Start();
        void Stop();
    }
}
