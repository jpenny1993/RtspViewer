using System;
using System.IO;
using RtspViewer.Configuration;
using RtspViewer.WinUi.Models;

namespace RtspViewer.WinUi.Services
{
    public class UnpackagedSettingsService : ISettingsService
    {
        private readonly string _settingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "RtspViewer",
            "settings.xml");
        
        public SettingsModel LoadSettings()
        {
            if (StreamConfiguration.TryLoad(_settingsPath, out SettingsModel settings))
            {
                return settings;
            }

            return new SettingsModel();
        }

        public void SaveSettings(SettingsModel settings)
        {
            StreamConfiguration.Save(_settingsPath, settings);
        }
    }
}
