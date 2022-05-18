using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NAudio.Wave;
using RtspViewer.WinUi.Models;

namespace RtspViewer.WinUi.Controls
{
    public sealed partial class VideoControl : UserControl
    {
        private BufferedWaveProvider _player;
        private WaveOut _waveOut;

        public VideoControl()
        {
            InitializeComponent();
        }

        public void ConfigureStream(StreamModel streamModel)
            => ViewModel.ConfigureStream(streamModel);

        private void VideoControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.MediaSource.StatusChanged += MediaSource_OnStatusChanged;
            ViewModel.MediaSource.VideoFrameDecoded += MediaSource_OnVideoFrameReceived;
            ViewModel.MediaSource.AudioFrameDecoded += MediaSource_OnAudioFrameReceived;

            if (ViewModel.AutoStart)
            {
                ViewModel.MediaSource.TogglePlay();
            }
        }

        private void VideoControl_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.MediaSource.StatusChanged -= MediaSource_OnStatusChanged;
            ViewModel.MediaSource.VideoFrameDecoded -= MediaSource_OnVideoFrameReceived;
            ViewModel.MediaSource.AudioFrameDecoded -= MediaSource_OnAudioFrameReceived;
            ViewModel.Dispose();
            ViewModel = null;
        }

        private void MediaSource_OnStatusChanged(object sender, string status)
        {
            DispatcherQueue.TryEnqueue(() => ViewModel?.UpdateStatus(status));   
        }

        private void MediaSource_OnAudioFrameReceived(object sender, LockedFrame<IDecodedAudioFrame> args)
        {
            if (!ViewModel.MediaSource.Started || ViewModel.Volume == 0.0f) 
            {
                args.Release();
                return;
            }

            var audioFrame = args.DecodedFrame;
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
                _waveOut.Volume = ViewModel.Volume;
            }

            _player.AddSamples(
                audioFrame.DecodedBytes.Array,
                audioFrame.DecodedBytes.Offset,
                audioFrame.DecodedBytes.Count);

            if (_waveOut.PlaybackState != PlaybackState.Playing)
            {
                _waveOut.Play();
            }

            args.Release();
        }

        private void MediaSource_OnVideoFrameReceived(object sender, LockedFrame<IDecodedVideoFrame> args)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    ViewModel?.UpdateVideoFrame(args.DecodedFrame);
                }
                finally
                { 
                    args.Release();
                }

                ViewModel?.UpdateRuntime();
            });
        }
    }
}
