using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using RtspClientSharp;
using RtspClientSharp.RawFrames;
using RtspViewer.RawFramesDecoding.DecodedFrames;
using RtspViewer.RawFramesReceiving;

namespace RtspViewer.Terminal
{
    class Program
    {
        private static IRawFramesSource _rawFramesSource;

        static void Main(string[] args)
        {
            var address = ConfigurationManager.AppSettings["Address"];
            var username = ConfigurationManager.AppSettings["Username"];
            var password = ConfigurationManager.AppSettings["Password"];

            var credential = new NetworkCredential(username, password);
            var connectionParameters = new ConnectionParameters(new Uri(address), credential)
            {
                RtpTransport = RtpTransportProtocol.TCP,
                CancelTimeout = TimeSpan.FromSeconds(1)
            };

            _rawFramesSource = new RawFramesSource(connectionParameters);
            _rawFramesSource.ConnectionStatusChanged += OnConnectionStatusChanged;
            _rawFramesSource.FrameReceived += OnFrameReceived;

            var videoSource = new RealtimeVideoSource(_rawFramesSource);
            videoSource.FrameReceived += VideoSource_FrameReceived;

            _rawFramesSource.Start();

            Console.WriteLine("Press enter to stop...");
            Console.ReadLine();
        }

        private static void OnConnectionStatusChanged(object sender, string status)
        {
            Console.WriteLine("ConnectionStatus: {0}", status);
        }

        private static void OnFrameReceived(object sender, RawFrame rawFrame)
        {
            Console.WriteLine("FrameReceived: {0:yyyy/MM/dd hh:mm:ss}, Type: {1}, Encoding: {2}",
                rawFrame.Timestamp, rawFrame.Type, rawFrame.GetType().Name);
        }

        private static void VideoSource_FrameReceived(object sender, IDecodedVideoFrame frame)
        {
            Console.WriteLine("VideoFrameDecoded: {0:yyyy/MM/dd hh:mm:ss}", frame.Timestamp);

            var bmp = new Bitmap(1280, 720);
            bmp.UpdateBitmap(frame);

            using (var fs = File.OpenWrite($".\\IMG\\{frame.Timestamp:yyyyMMddhhmmssFFFFF}.jpg"))
            {
                bmp.Save(fs, ImageFormat.Jpeg);
            }
        }
    }
}
