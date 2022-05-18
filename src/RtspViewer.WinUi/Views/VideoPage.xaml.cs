using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace RtspViewer.WinUi.Views
{
    public sealed partial class VideoPage : Page
    {
        public VideoPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var streamIdentifier = (string)e.Parameter;
            var app = Application.Current as App;
            var streamModel = app.Settings.Streams.First(s => s.Identifier == streamIdentifier);
            Video.ConfigureStream(streamModel);
        }
    }
}
