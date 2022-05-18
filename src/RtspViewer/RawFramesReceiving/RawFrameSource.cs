using System;
using System.Net;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using RtspClientSharp;
using RtspClientSharp.RawFrames;
using RtspClientSharp.Rtsp;
using RtspViewer.Configuration;

namespace RtspViewer.RawFramesReceiving
{
    public class RawFramesSource : IRawFramesSource
    {
        private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(5);
        private readonly ConnectionParameters _connectionParameters;
        private CancellationTokenSource _cancellationTokenSource;
        private bool disposedValue;

        public EventHandler<RawFrame> FrameReceived { get; set; }
        public EventHandler<string> ConnectionStatusChanged { get; set; }

        public RawFramesSource(StreamConfiguration config)
        {
            var uri = new Uri(config.Address);

            _connectionParameters = !string.IsNullOrEmpty(uri.UserInfo)
                ? new ConnectionParameters(uri)
                : new ConnectionParameters(uri, new NetworkCredential(config.Username, config.Password));

            _connectionParameters.RtpTransport = (RtpTransportProtocol)((int)config.Protocol);
            _connectionParameters.CancelTimeout = TimeSpan.FromSeconds(1);
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            CancellationToken token = _cancellationTokenSource.Token;

            Task.Run(async () => await ReceiveAsync(token));
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }

        private async Task ReceiveAsync(CancellationToken token)
        {
            try
            {
                using (var rtspClient = new RtspClient(_connectionParameters))
                {
                    rtspClient.FrameReceived += RtspClientOnFrameReceived;

                    while (!token.IsCancellationRequested)
                    {
                        OnStatusChanged("Connecting");

                        try
                        {
                            await rtspClient.ConnectAsync(token);
                        }
                        catch (InvalidCredentialException)
                        {
                            OnStatusChanged("Invalid login and/or password");
                            await Task.Delay(RetryDelay, token);
                            return;
                        }
                        catch (RtspClientException e)
                        {
                            OnStatusChanged(e.Message);
                            await Task.Delay(RetryDelay, token);
                            continue;
                        }

                        OnStatusChanged("Receiving frames");

                        try
                        {
                            await rtspClient.ReceiveAsync(token);
                        }
                        catch (RtspClientException e)
                        {
                            OnStatusChanged(e.Message);
                            await Task.Delay(RetryDelay, token);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void RtspClientOnFrameReceived(object sender, RawFrame rawFrame)
        {
            FrameReceived?.Invoke(this, rawFrame);
        }

        private void OnStatusChanged(string status)
        {
            ConnectionStatusChanged?.Invoke(this, status);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();
                    _cancellationTokenSource = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
