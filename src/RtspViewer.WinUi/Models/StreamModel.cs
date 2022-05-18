using System;
using RtspViewer.Configuration;

namespace RtspViewer.WinUi.Models
{
    [Serializable]
    public sealed class StreamModel : StreamConfiguration
    {
        public string Identifier { get; set; }
        public string Name { get; set; }
        public bool AutoStart { get; set; }
        public StreamScale Scale { get; set; }
        public int ManualWidth { get; set; }
        public int ManualHeight { get; set; }
        public float Volume { get; set; }
    }
}
