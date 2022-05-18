using System;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using RtspViewer.Configuration;
using RtspViewer.WinUi.Constants;
using RtspViewer.WinUi.Converters;
using RtspViewer.WinUi.Models;

namespace RtspViewer.WinUi.ViewModels
{
    public partial class VideoViewModel : ObservableObject, IDisposable
    {
        private readonly IFrameTransformer _frameTransformer;
        private readonly BitmapConverter _bitmapConverter;

        private bool _disposedValue;

        [ObservableProperty]
        private string _status;

        [ObservableProperty]
        private string _totalRuntime;

        [ObservableProperty]
        private BitmapImage _image;

        [ObservableProperty]
        private string _togglePlayIcon;

        [ObservableProperty]
        private Stretch _imageStretch;

        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(VolumeIcon))]
        private float _volume;

        public string VolumeIcon => _volume switch
        {
            0.0f => Glyphs.Mute,
            < 0.3f => Glyphs.Volume0,
            < 0.5f => Glyphs.Volume1,
            < 0.8f => Glyphs.Volume2,
            1.0f => Glyphs.Volume3,
            _ => Glyphs.Volume
        };

        public bool AutoStart { get; private set; }

        public IMediaSource MediaSource { get; }

        public ICommand TogglePlayCommand { get; }

        public VideoViewModel()
        {
            _togglePlayIcon = Glyphs.Play;
            _status = "Ready";
            _totalRuntime = "00:00:00";
            _volume = 1.0f;
            _image = new BitmapImage();
            _imageStretch = Stretch.None;
            _frameTransformer = new FrameTransformer();
            _bitmapConverter = new BitmapConverter(_image);
            MediaSource = new MediaSource();
            TogglePlayCommand = new RelayCommand(MediaSource.TogglePlay);
        }

        public void ConfigureStream(StreamModel streamModel)
        {
            if (Uri.TryCreate(streamModel.Address, UriKind.Absolute, out _))
            {
                MediaSource.ConfigureStream(streamModel);
                AutoStart = streamModel.AutoStart;
            }
            else
            {
                Status = "Invalid stream address";
                AutoStart = false;
            }

            ImageStretch = streamModel.Scale switch
            {
                StreamScale.None        => Stretch.None,
                StreamScale.AdjustToFit => Stretch.Uniform,
                StreamScale.Overscan    => Stretch.UniformToFill,
                StreamScale.Stretch     => Stretch.Fill,
                StreamScale.Manual      => Stretch.None,
                _ => throw new NotSupportedException("Suggested stream scale not supported")
            };

            if (streamModel.Scale == StreamScale.Manual)
            {
                _frameTransformer.UpdateFrameSize(streamModel.ManualWidth, streamModel.ManualHeight);
            }

            Volume = streamModel.Volume;
        }

        public void UpdateVideoFrame(IDecodedVideoFrame videoFrame)
        {
            if (!MediaSource.Started) return;
            var bitmap = _frameTransformer.TransformToBitmap(videoFrame);
            Image = _bitmapConverter.Convert(bitmap);
        }

        public void UpdateRuntime()
        {
            var runtime = (int)MediaSource.TimeElapsed.TotalDays > 0
                ? $"{MediaSource.TimeElapsed:dd\\.hh\\:mm\\:ss}"
                : $"{MediaSource.TimeElapsed:hh\\:mm\\:ss}";

            if (TotalRuntime != runtime)
            {
                TotalRuntime = runtime;
            }
        }

        public void UpdateStatus(string status)
        {
            Status = status;
            TogglePlayIcon = MediaSource.Started ? Glyphs.Pause : Glyphs.Play;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    MediaSource.Dispose();
                    _bitmapConverter.Dispose();
                    _frameTransformer.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
