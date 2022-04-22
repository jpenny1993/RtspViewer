using System.Drawing;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using RtspViewer.WinUi.Extensions;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RtspViewer.WinUi.Views
{
    public sealed partial class BitmapControl : UserControl
    {
        private const string FilePath = @"C:\Users\jpenn\OneDrive\Pictures\album-covers\4b2f0b37de20e99fd912b86f1d89a26c.png";
        private BitmapImage BitmapImage;

        public BitmapControl()
        {
            InitializeComponent();
            BitmapImage = new Bitmap(FilePath).ToBitmapImageAsync().GetAwaiter().GetResult();
        }
    }
}
