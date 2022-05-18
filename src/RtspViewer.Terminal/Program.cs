using System;
using System.Configuration;
using System.Drawing.Imaging;
using System.IO;
using RtspViewer.Configuration;

namespace RtspViewer.Terminal
{
    class Program
    {
        private static readonly IFrameTransformer _frameTransformer = new FrameTransformer(1280, 720);

        static void Main(string[] args)
        {
            var config = new StreamConfiguration
            {
                Address = ConfigurationManager.AppSettings["Address"],
                Username = ConfigurationManager.AppSettings["Username"],
                Password = ConfigurationManager.AppSettings["Password"]
            };

            IMediaSource _mediaSource = new MediaSource(config);
            _mediaSource.StatusChanged += OnStatusChanged;
            _mediaSource.VideoFrameDecoded += MediaSource_OnVideoFrameReceived;

            if (!Directory.Exists(".\\IMG"))
            {
                Directory.CreateDirectory(".\\IMG");
            }

            _mediaSource.Start();

            Console.WriteLine("Press enter to stop...");
            Console.ReadLine();
        }

        private static void OnStatusChanged(object sender, string status)
        {
            Console.WriteLine("ConnectionStatus: {0}", status);
        }

        private static void MediaSource_OnVideoFrameReceived(object sender, LockedFrame<IDecodedVideoFrame> args)
        {
            var decodedFrame = args.DecodedFrame;
            Console.WriteLine("VideoFrameDecoded: {0:yyyy/MM/dd hh:mm:ss}, {1}x{2}",
                    decodedFrame.Timestamp, decodedFrame.Width, decodedFrame.Height);

            var bmp = _frameTransformer.TransformToBitmap(decodedFrame);
            Console.WriteLine("Converted to bitmap");

            using (var fs = File.OpenWrite($".\\IMG\\{decodedFrame.Timestamp:yyyyMMddhhmmssFFFFF}.jpg"))
            {
                bmp.Save(fs, ImageFormat.Jpeg);
            }
            args.Release();
        }
    }
}
