using System;

namespace RtspViewer.WinUi.Models
{
    [Serializable]
    public sealed class SettingsModel
    {
        public bool IsLightTheme { get; set; }

        public StreamModel[] Streams { get; set; } = Array.Empty<StreamModel>();
    }
}
