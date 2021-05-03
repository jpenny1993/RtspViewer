using System;
using System.Windows.Forms;
using RtspViewer.Configuration;

namespace RtspViewer.Forms.Windows
{
    public partial class MainForm : Form
    {
        private const string ConfigurationFilename = "config.json.enc";
        private ConfigForm _configForm;

        private StreamConfiguration _config = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void ConfigForm_ConfigurationUpdated(object sender, StreamConfiguration config)
        {
            _config = config;
            videoView.InitialiseStream(_config);
            StreamConfiguration.Save(ConfigurationFilename, _config);

            if (string.IsNullOrWhiteSpace(_config.Address))
            {
                videoView.Stop();
                menuBtnPlay.Enabled = false;
            }
            else
            {
                videoView.Start();
                menuBtnPlay.Enabled = true;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (StreamConfiguration.TryLoad(ConfigurationFilename, out _config))
            {
                videoView.InitialiseStream(_config);
                menuBtnPlay.Enabled = true;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            videoView.Stop();
        }

        private void MenuBtnPlay_Click(object sender, EventArgs e)
        {
            if (videoView.Started)
            {
                videoView.Stop();
                menuBtnPlay.Text = "Play";
            }
            else
            { 
                videoView.Start();
                menuBtnPlay.Text = "Pause";
            }
        }

        private void MenuBtnSettings_Click(object sender, EventArgs e)
        {
            _configForm = new ConfigForm();
            _configForm.InitialiseFields(_config);
            _configForm.ConfigurationUpdated += ConfigForm_ConfigurationUpdated;
            _configForm.FormClosing += (object _s, FormClosingEventArgs _e) => 
            {
                _configForm.ConfigurationUpdated += ConfigForm_ConfigurationUpdated;
            };
            _configForm.Show();
        }

        private void MenuBtnQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var timeElapsed = videoView.TimeElapsed;
            var framesReceived = videoView.FramesReceived;

            var receivedFps = CalculateFps(framesReceived, timeElapsed);

            statusLblConnection.Text = videoView.ConnectionStatus;
            statusLblTimer.Text = $"{timeElapsed:hh}:{timeElapsed:mm}:{timeElapsed:ss}";
            statusLblFps.Text = $"{receivedFps:F2} fps";
        }

        private static double CalculateFps(double frames, TimeSpan elapsed)
        {
            if (frames > 0)
            {
                return 1000d * frames / elapsed.TotalMilliseconds;
            }

            return 0d;
        }
    }
}
