using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RtspViewer.WinUi.Controls;
using RtspViewer.WinUi.Models;

namespace RtspViewer.WinUi.Views
{
    public sealed partial class MainPage : Page
    {
        private App App => Application.Current as App;

        public MainPage()
        {
            InitializeComponent();
            LoadVideos();
        }

        private void LoadVideos()
        {
            var settings = App.Settings;
            var totalStreams = settings.Streams.Length;
            int maxVideosPerRow = totalStreams switch
            {
                > 9 => 5,
                > 4 => 3,
                >2 => 2,
                2 => 1,
                _ => 1
            };

            for (int streamIndex = 0; streamIndex < totalStreams; streamIndex++)
            {
                AddStream(streamIndex, maxVideosPerRow, settings.Streams[streamIndex]);
            }
        }

        private void AddStream(int index, int maxVideosPerRow, StreamModel streamModel)
        {
            var row = index / maxVideosPerRow;
            var column = index % maxVideosPerRow;

            if (index < maxVideosPerRow && index > 0)
            {
                VideoGrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
            }

            if (column == 0 && index > 0)
            {
                VideoGrid.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(1, GridUnitType.Star)
                });
            }

            var videoControl = new VideoControl();
            videoControl.ConfigureStream(streamModel);

            VideoGrid.Children.Add(videoControl);
            Grid.SetColumn(videoControl, column);
            Grid.SetRow(videoControl, row);
        }
    }
}
