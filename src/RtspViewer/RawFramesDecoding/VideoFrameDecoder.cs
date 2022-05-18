using System;
using System.Collections.Generic;
using RtspClientSharp.RawFrames.Video;
using RtspViewer.RawFramesDecoding.FFmpeg;

namespace RtspViewer.RawFramesDecoding
{
    public sealed class VideoFrameDecoder : IVideoFrameDecoder
    {
        private readonly Dictionary<FFmpegVideoCodecId, FFmpegVideoDecoder> VideoDecodersMap =
          new Dictionary<FFmpegVideoCodecId, FFmpegVideoDecoder>();
        private bool disposedValue;

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

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                // Decoders contain unmanaged objects and must be disposed first
                foreach (var decoder in VideoDecodersMap.Values)
                {
                    decoder.Dispose();
                }

                if (disposing)
                {
                    VideoDecodersMap.Clear();
                }

                disposedValue = true;
            }
        }

        ~VideoFrameDecoder()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
