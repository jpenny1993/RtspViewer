using System;
using System.Linq;
using System.Threading;
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
        // Our instance of FFMPEG can only process a single frame at once, I'm using semaphore prevent multithreaded access
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
        private static IMediaSource ActiveDecoderOwner;

        // These cannot be static, if we want to provide multiple streams, then we need multiple decoder handles
        private readonly IAudioFrameDecoder AudioDecoder = new AudioFrameDecoder();
        private readonly IVideoFrameDecoder VideoDecoder = new VideoFrameDecoder();

        private IRawFramesSource _rawFramesSource;
        private bool disposedValue;

        public bool Started { get; private set; }
        public DateTime TimeStartedUtc { get; private set; }
        public DateTime? TimeStoppedUtc { get; private set; }
        public TimeSpan TimeElapsed => Started ? (TimeStoppedUtc ?? DateTime.UtcNow) - TimeStartedUtc : TimeSpan.Zero;
        public EventHandler<string> StatusChanged { get; set; }
        public EventHandler<LockedFrame<IDecodedAudioFrame>> AudioFrameDecoded { get; set; }
        public EventHandler<LockedFrame<IDecodedVideoFrame>> VideoFrameDecoded { get; set; }

        public MediaSource()
        {
        }

        public MediaSource(StreamConfiguration config)
        {
            _rawFramesSource = new RawFramesSource(config);
        }

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
                TimeStartedUtc = DateTime.UtcNow;
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
                TimeStoppedUtc = DateTime.UtcNow;
                StatusChanged?.Invoke(this, "Disconnected");
                ReleaseDecoderLock();
            }
        }

        public void TogglePlay()
        {
            if (!Started)
                Start();
            else
                Stop();
        }

        private void RawFramesSource_OnConnectionStatusChanged(object sender, string status)
        {
            if (HasSubscribers(StatusChanged))
            {
                StatusChanged.Invoke(sender, status);
            }
        }

        private void RawFramesSource_OnFrameReceived(object sender, RawFrame rawFrame)
        {
            // Skip frames that have waited too long
            if (!Semaphore.Wait(100)) return;
            ActiveDecoderOwner = this;

            if (HasSubscribers(VideoFrameDecoded) && rawFrame is RawVideoFrame videoFrame)
            {
                var decodedVideoFrame = VideoDecoder.Decode(videoFrame);
                if (decodedVideoFrame != null) // Ignore video decoding errors
                {
                    var args = LockedFrame.Create(decodedVideoFrame, ReleaseDecoderLock);
                    VideoFrameDecoded?.Invoke(this, args);
                    return;
                }
            }

            if (HasSubscribers(AudioFrameDecoded) && rawFrame is RawAudioFrame audioFrame)
            {
                var decodedAudioFrame = AudioDecoder.Decode(audioFrame);
                if (decodedAudioFrame != null)  // Ignore audio decoding errors
                {
                    var args = LockedFrame.Create(decodedAudioFrame, ReleaseDecoderLock);
                    AudioFrameDecoded?.Invoke(this, args);
                    return;
                }
            }

            // No decoding required due to either decoding errors or no event subscribers
            ReleaseDecoderLock();
        }

        private void ReleaseDecoderLock()
        {
            if (Semaphore.CurrentCount == 0 && ActiveDecoderOwner == this)
            {
                Semaphore.Release();
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _rawFramesSource?.Dispose();
                    _rawFramesSource = null;
                    ReleaseDecoderLock();
                }

                disposedValue = true;
            }
        }

        private static bool HasSubscribers<T>(EventHandler<T> handler)
        {
            return handler?.GetInvocationList().Any() ?? false;
        }
    }
}
