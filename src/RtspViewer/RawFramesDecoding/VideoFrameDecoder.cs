using System;
using System.Collections.Generic;
using RtspClientSharp.RawFrames.Video;
using RtspViewer.RawFramesDecoding.FFmpeg;

namespace RtspViewer.RawFramesDecoding
{
    public sealed class VideoFrameDecoder : IVideoFrameDecoder
    {
        private static readonly Dictionary<FFmpegVideoCodecId, FFmpegVideoDecoder> VideoDecodersMap =
          new Dictionary<FFmpegVideoCodecId, FFmpegVideoDecoder>();

        public IDecodedVideoFrame Decode(RawVideoFrame rawVideoFrame)
        {
            var decoder = GetDecoderForFrame(rawVideoFrame);
            return decoder.TryDecode(rawVideoFrame);
        }

        private FFmpegVideoDecoder GetDecoderForFrame(RawVideoFrame videoFrame)
        {
            var codecId = DetectCodecId(videoFrame);
            if (!VideoDecodersMap.TryGetValue(codecId, out FFmpegVideoDecoder decoder))
            {
                decoder = FFmpegVideoDecoder.CreateDecoder(codecId);
                VideoDecodersMap.Add(codecId, decoder);
            }

            return decoder;
        }

        private static FFmpegVideoCodecId DetectCodecId(RawVideoFrame videoFrame)
        {
            if (videoFrame is RawJpegFrame)
                return FFmpegVideoCodecId.MJPEG;
            if (videoFrame is RawH264Frame)
                return FFmpegVideoCodecId.H264;

            throw new ArgumentOutOfRangeException(nameof(videoFrame));
        }
    }
}
