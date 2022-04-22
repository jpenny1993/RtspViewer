using System;
using RtspClientSharp.RawFrames;
using RtspClientSharp.RawFrames.Audio;
using RtspClientSharp.RawFrames.Video;
using RtspViewer.Configuration;
using RtspViewer.RawFramesDecoding;
using RtspViewer.RawFramesReceiving;

namespace RtspViewer
{
    public sealed class MediaSource : IMediaSource
    {
        private readonly IAudioFrameDecoder _audioSource;
        private readonly IVideoFrameDecoder _videoSource;

        private IRawFramesSource _rawFramesSource;

        public bool Started { get; private set; }
        public EventHandler<string> StatusChanged { get; set; }
        public EventHandler<IDecodedAudioFrame> AudioFrameReceived { get; set; }
        public EventHandler<IDecodedVideoFrame> VideoFrameReceived { get; set; }

        public MediaSource()
        {
            _audioSource = new AudioFrameDecoder();
            _videoSource = new VideoFrameDecoder();
        }

        public MediaSource(StreamConfiguration config)
        {
            _audioSource = new AudioFrameDecoder();
            _videoSource = new VideoFrameDecoder();
            _rawFramesSource = new RawFramesSource(config);
        }

        ~MediaSource() => Stop();

        public void ConfigureStream(StreamConfiguration config)
        {
            Stop();
            StatusChanged?.Invoke(this, "Configuring");
            _rawFramesSource = new RawFramesSource(config);
            StatusChanged?.Invoke(this, "Ready");
        }

        public void Start()
        {
            if (!Started && _rawFramesSource != null)
            {
                Started = true;
                StatusChanged?.Invoke(this, "Starting");
                _rawFramesSource.ConnectionStatusChanged += RawFramesSource_OnConnectionStatusChanged;
                _rawFramesSource.FrameReceived += RawFramesSource_OnFrameReceived;
                _rawFramesSource.Start();
            }
        }

        public void Stop()
        {
            if (Started && _rawFramesSource != null)
            {
                Started = false;
                _rawFramesSource.Stop();
                _rawFramesSource.ConnectionStatusChanged -= RawFramesSource_OnConnectionStatusChanged;
                _rawFramesSource.FrameReceived -= RawFramesSource_OnFrameReceived;
                StatusChanged?.Invoke(this, "Disconnected");
            }
        }

        private void RawFramesSource_OnConnectionStatusChanged(object sender, string status)
        {
            StatusChanged?.Invoke(sender, status);
        }

        private void RawFramesSource_OnFrameReceived(object sender, RawFrame rawFrame)
        {
            if (rawFrame is RawVideoFrame videoFrame)
            {
                var decodedVideoFrame = _videoSource.Decode(videoFrame);
                VideoFrameReceived?.Invoke(this, decodedVideoFrame);
                return;
            }

            if (rawFrame is RawAudioFrame audioFrame)
            {
                var decodedAudioFrame = _audioSource.Decode(audioFrame);
                if (decodedAudioFrame.DecodedBytes.Count > 0)
                {
                    AudioFrameReceived?.Invoke(this, decodedAudioFrame);
                }
                return;
            }
        }
    }
}
