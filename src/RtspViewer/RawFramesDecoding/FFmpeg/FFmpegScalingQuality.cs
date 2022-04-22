﻿using System;

namespace RtspViewer.RawFramesDecoding.FFmpeg
{
    [Flags]
    public enum FFmpegScalingQuality
    {
        FastBilinear = 1,
        Bilinear = 2,
        Bicubic = 4,
        Point = 0x10,
        Area = 0x20,
    }
}
