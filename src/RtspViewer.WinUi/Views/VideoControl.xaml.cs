using System;
using RtspViewer.Configuration;
using RtspViewer.RawFramesDecoding;
using RtspViewer.RawFramesDecoding.DecodedFrames;
using RtspViewer.RawFramesReceiving;
using System.Drawing;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using RtspViewer.WinUi.Extensions;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RtspViewer.WinUi.Views
{
    public sealed partial class VideoControl : UserControl
    {
        private IRawFramesSource _rawFramesSource;
        private IVideoSource _videoSource;

        private BitmapImage _image;
        private Bitmap _bitmap;
        private TransformParameters _transformParameters;
        private DateTime _startTime;

        public VideoControl()
        {
            InitializeComponent();
            InitialiseStream(new StreamConfiguration
            {
            });
            Start();
        }

        ~VideoControl()
        {
            Stop();
        }

        public string ConnectionStatus { get; private set; } = "Disconnected";

        public long FramesReceived { get; private set; }

        public bool Started { get; private set; }

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

        public void InitialiseStream(StreamConfiguration config)
        {
            Stop();

            _rawFramesSource = new RawFramesSource(config);
            _videoSource = new RealtimeVideoSource(_rawFramesSource);
        }

        public void Start()
        {
            if (!Started && _rawFramesSource != null)
            {
                Started = true;
                _videoSource.FrameReceived += (o, f) => Task.Run(() => VideoSource_FrameReceived(o, f));

                _bitmap = new Bitmap(320, 240);
                _transformParameters = _bitmap.GetTransformParameters();

                _rawFramesSource.Start();
                _startTime = DateTime.UtcNow;
            }
        }

        public void Stop()
        {
            if (Started && _rawFramesSource != null)
            {
                Started = false;
                _rawFramesSource.Stop();

                _videoSource.FrameReceived -= (o, f) => Task.Run(() => VideoSource_FrameReceived(o, f));

                ConnectionStatus = "Disconnected";
                FramesReceived = 0;
            }
        }

        private void ReinitializeBitmap(int width, int height)
        {
            if (Started)
            {
                // Allow the next frame to size correctly
                _bitmap = new Bitmap(width, height);
                _transformParameters = _bitmap.GetTransformParameters();
            }
            else if (_bitmap != null)
            {
                // Resize the current frame
                Bitmap resized = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(resized))
                {
                    g.DrawImage(_bitmap, 0, 0, width, height);
                }

                _bitmap = resized;
            }
        }

        private async Task VideoSource_FrameReceived(object sender, IDecodedVideoFrame frame)
        {
            FramesReceived++;
            _bitmap.UpdateBitmap(frame, _transformParameters);
            _image = await _bitmap.ToBitmapImageAsync();
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
            ReinitializeBitmap((int)ActualWidth, (int)ActualHeight);
        }
    }
}
