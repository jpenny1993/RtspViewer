using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;
using NAudio.Wave;
using RtspViewer.Configuration;

namespace RtspViewer.Forms.Controls
{
    public partial class VideoView : UserControl
    {
        private readonly IFrameTransformer _frameTransformer;
        private readonly IMediaSource _mediaSource;

        private Dispatcher _dispatcher;
        private Bitmap _bitmap;
        private DateTime _startTime;
        private BufferedWaveProvider _player;
        private WaveOut _waveOut;

        public string ConnectionStatus { get; private set; } = "Disconnected";

        public long FramesReceived { get; private set; }

        public bool Started => _mediaSource.Started;

        public TimeSpan TimeElapsed
        {
            get 
            {
                if (Started)
                {
                    return DateTime.UtcNow - _startTime;
                }

                return TimeSpan.Zero;
            }
        }

        public VideoView()
        {
            InitializeComponent();
            _dispatcher = Dispatcher.CurrentDispatcher;
            _frameTransformer = new FrameTransformer();
            _mediaSource = new MediaSource();
            _mediaSource.StatusChanged += MediaSource_OnStatusChanged;
            _mediaSource.AudioFrameReceived += MediaSource_OnAudioFrameReceived;
            _mediaSource.VideoFrameReceived += MediaSource_OnVideoFrameReceived;
        }

        ~VideoView() 
        {
            _mediaSource.Stop();
            _mediaSource.StatusChanged -= MediaSource_OnStatusChanged;
            _mediaSource.AudioFrameReceived -= MediaSource_OnAudioFrameReceived;
            _mediaSource.VideoFrameReceived -= MediaSource_OnVideoFrameReceived;
        }

        public void InitialiseStream(StreamConfiguration config)
        {
            _mediaSource.Stop();
            _mediaSource.ConfigureStream(config);
            _frameTransformer.UpdateFrameSize(Width, Height);
        }

        public void Start()
        {
            if (!Started)
            {
                FramesReceived = 0;
                _startTime = DateTime.UtcNow;
                _mediaSource.Start();
            }
        }

        public void Stop()
        {
            _mediaSource.Stop();
        }

        private void MediaSource_OnStatusChanged(object sender, string status)
        {
            ConnectionStatus = status;
        }

        private void MediaSource_OnAudioFrameReceived(object sender, IDecodedAudioFrame audioFrame)
        {
            if (_player == null)
            {
                _player = new BufferedWaveProvider(
                    new WaveFormat(
                        audioFrame.Format.SampleRate,
                        audioFrame.Format.BitPerSample,
                        audioFrame.Format.Channels));
                _player.BufferLength = 2560 * 16;
                _player.DiscardOnBufferOverflow = true;

                _waveOut = new WaveOut();
                _waveOut.Init(_player);
                _waveOut.Volume = 1.0f;
            }

            _player.AddSamples(
                audioFrame.DecodedBytes.Array,
                audioFrame.DecodedBytes.Offset,
                audioFrame.DecodedBytes.Count);

            if (_waveOut.PlaybackState != PlaybackState.Playing && Started)
            {
                _waveOut.Play();
            }
        }

        private void MediaSource_OnVideoFrameReceived(object sender, IDecodedVideoFrame videoFrame)
        {
            FramesReceived++;
            _bitmap = _frameTransformer.TransformToBitmap(videoFrame);
            _dispatcher.Invoke(() => video.Image = _bitmap, DispatcherPriority.Send);
        }

        private void ReinitializeBitmap(int width, int height)
        {
            if (Started)
            {
                // Allow the next frame to size correctly
                _frameTransformer.UpdateFrameSize(Width, Height);
            }
            else if (_bitmap != null)
            {
                // Resize the current frame
                _dispatcher.Invoke(() =>
                {
                    Bitmap resized = new Bitmap(width, height);
                    using (Graphics g = Graphics.FromImage(resized))
                    {
                        g.DrawImage(_bitmap, 0, 0, width, height);
                    }

                    video.Image = resized;
                }, DispatcherPriority.Send);
            }
        }

        private void Video_Click(object sender, EventArgs e)
        {
            if (Started) 
            {
                Stop();
            }
            else 
            {
                Start();
            }
        }

        private void Video_SizeChanged(object sender, EventArgs e)
        {
            ReinitializeBitmap(Width, Height);
        }
    }
}
