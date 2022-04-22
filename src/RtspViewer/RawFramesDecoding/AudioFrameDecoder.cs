using System;
using System.Collections.Generic;
using RtspClientSharp.RawFrames.Audio;
using RtspViewer.RawFramesDecoding.DecodedFrames;
using RtspViewer.RawFramesDecoding.FFmpeg;

namespace RtspViewer.RawFramesDecoding
{
    public sealed class AudioFrameDecoder : IAudioFrameDecoder
    {
        private static readonly Dictionary<FFmpegAudioCodecId, FFmpegAudioDecoder> AudioDecodersMap =
            new Dictionary<FFmpegAudioCodecId, FFmpegAudioDecoder>();

        private readonly AudioConversionParameters _conversionParameters;

        public AudioFrameDecoder()
        {
            _conversionParameters = new AudioConversionParameters
            {
                OutBitsPerSample = 16
            };
        }

        public AudioFrameDecoder(AudioConversionParameters conversionParameters)
        {
            _conversionParameters = conversionParameters;
        }

        public IDecodedAudioFrame Decode(RawAudioFrame rawAudioFrame)
        {
            var decoder = GetDecoderForFrame(rawAudioFrame);

            if (!decoder.TryDecode(rawAudioFrame))
            {
                return new DecodedAudioFrame(
                    rawAudioFrame.Timestamp,
                    new ArraySegment<byte>(),
                    new AudioFrameFormat(
                        _conversionParameters.OutSampleRate,
                        _conversionParameters.OutBitsPerSample,
                        _conversionParameters.OutChannels));
            }

            return decoder.GetDecodedFrame(_conversionParameters);
        }

        private FFmpegAudioDecoder GetDecoderForFrame(RawAudioFrame audioFrame)
        {
            var codecId = DetectCodecId(audioFrame);
            if (!AudioDecodersMap.TryGetValue(codecId, out FFmpegAudioDecoder decoder))
            {
                int bitsPerCodedSample = 0;

                if (audioFrame is RawG726Frame g726Frame)
                    bitsPerCodedSample = g726Frame.BitsPerCodedSample;

                decoder = FFmpegAudioDecoder.CreateDecoder(codecId, bitsPerCodedSample);
                AudioDecodersMap.Add(codecId, decoder);
            }

            return decoder;
        }

        private static FFmpegAudioCodecId DetectCodecId(RawAudioFrame audioFrame)
        {
            if (audioFrame is RawAACFrame)
                return FFmpegAudioCodecId.AAC;
            if (audioFrame is RawG711AFrame)
                return FFmpegAudioCodecId.G711A;
            if (audioFrame is RawG711UFrame)
                return FFmpegAudioCodecId.G711U;
            if (audioFrame is RawG726Frame)
                return FFmpegAudioCodecId.G726;

            throw new ArgumentOutOfRangeException(nameof(audioFrame));
        }
    }
}
