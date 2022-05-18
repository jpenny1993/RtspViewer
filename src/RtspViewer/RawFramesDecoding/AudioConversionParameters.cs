﻿namespace RtspViewer.RawFramesDecoding
{
    public sealed class AudioConversionParameters
    {
        public int OutSampleRate { get; set; }
        public int OutBitsPerSample { get; set; }
        public int OutChannels { get; set; }
    }
}
