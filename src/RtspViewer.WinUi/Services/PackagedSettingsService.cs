using System;
using RtspViewer.Configuration;
using RtspViewer.WinUi.Models;
using Windows.Storage;

namespace RtspViewer.WinUi.Services
{
    public class PackagedSettingsService : ISettingsService
    {
        private const string SettingsFileName = "settings.xml";
        private readonly ApplicationDataContainer _container;

        public PackagedSettingsService(ApplicationDataContainer container)
        {
            _container = container;
        }

        public SettingsModel LoadSettings()
        {
            SettingsModel settings = null;
            try
            {
                var applicationData = _container.Values[SettingsFileName];
                if (applicationData != null)
                {
                    settings = StreamConfiguration.Decrypt<SettingsModel>(applicationData.ToString());
                }
            }
            catch (Exception)
            {
                // Unable to read the settings (e.g. corrupt file or running an unpackaged build)
                _container.Values.Remove(SettingsFileName);
            }

            if (settings == default)
            {
                settings = new SettingsModel();
            }

            return settings;
        }

        public void SaveSettings(SettingsModel settings)
        {
            try
            {
                var applicationData = StreamConfiguration.Encrypt(settings);
                ApplicationData.Current.LocalSettings.Values[SettingsFileName] = applicationData;
            }
            catch (Exception)
            {
                // Unable to write the settings (e.g. out of disk space, permissions required, running an unpackaged build, etc)
            }
        }
    }
}
