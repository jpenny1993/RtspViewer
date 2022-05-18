using System;
using Microsoft.UI.Xaml;
using RtspViewer.WinUi.Models;
using RtspViewer.WinUi.Services;
using Windows.Storage;

namespace RtspViewer.WinUi
{
    public partial class App : Application
    {
        private readonly ISettingsService _settingsService;
        private MainWindow _main;

        public SettingsModel Settings { get; private set; }

        public INavigation Navigation => _main;

        public IThematic Theme => _main;

        public App()
        {
            InitializeComponent();
            Settings = new SettingsModel();

            try
            {
                var packagedSettingsProvider = ApplicationData.Current.LocalSettings;
                _settingsService = new PackagedSettingsService(packagedSettingsProvider);
            }
            catch (InvalidOperationException)
            {
                _settingsService = new UnpackagedSettingsService();
            }
        }

        public void SaveSettings(SettingsModel settings)
        {
            Settings = settings;
            _settingsService.SaveSettings(Settings);
            _main.RebuildCameraMenuItems(Settings);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Settings = _settingsService.LoadSettings();
            _main = new MainWindow(Settings);
            _main.Activate();
        }
    }
}
