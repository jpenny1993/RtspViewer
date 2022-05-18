using RtspViewer.WinUi.Models;

namespace RtspViewer.WinUi.Services
{
    public interface ISettingsService
    {
        SettingsModel LoadSettings();
        void SaveSettings(SettingsModel settings);
    }
}