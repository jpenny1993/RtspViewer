using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using DeviceId;
using Newtonsoft.Json;
using RtspViewer.Extensions;
using RtspViewer.Forms.Models;

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
            SaveConfiguration(_config);

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
            if (TryLoadConfiguration(out _config))
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

        private static SymmetricAlgorithm GetEncryptor()
        {
            var algorithm = new AesManaged();
            var keyBytes = new byte[algorithm.KeySize / 8];  // default is 258 bits / 32 bytes
            var ivBytes = new byte[algorithm.BlockSize / 8]; // default is 128 bits / 16 bytes

            // Use hardware + user data to determine Key & IV
            // That way we can reload the config without storing the keys
            var machineInfo = new DeviceIdBuilder()
                .AddMacAddress()
                .AddProcessorId()
                .AddMotherboardSerialNumber()
                .ToString();

            var machineKeyBytes = Encoding.Default.GetBytes(machineInfo);
            Array.Copy(machineKeyBytes, keyBytes, keyBytes.Length);

            var userInfo = new DeviceIdBuilder()
                .AddOSInstallationID()
                .AddUserName()
                .ToString();

            var userInfoBytes = Encoding.Default.GetBytes(userInfo);
            Array.Copy(userInfoBytes, ivBytes, ivBytes.Length);

            algorithm.Key = keyBytes;
            algorithm.IV = ivBytes;

            return algorithm;
        }

        private static bool TryLoadConfiguration(out StreamConfiguration config)
        {
            if (!File.Exists(ConfigurationFilename))
            {
                config = null;
                return false;
            }

            try
            {
                using (var fs = new FileStream(ConfigurationFilename, FileMode.Open))
                using (var sr = new StreamReader(fs))
                {
                    var encryptedText = sr.ReadToEnd();
                    var json = GetEncryptor().DecryptText(encryptedText);
                    config = JsonConvert.DeserializeObject<StreamConfiguration>(json);
                }
            }
            catch
            {
                config = null;
                return false;
            }
 
            return true;
        }

        private static void SaveConfiguration(StreamConfiguration config)
        {
            
            using (var fs = new FileStream(ConfigurationFilename, FileMode.OpenOrCreate))
            using (var sw = new StreamWriter(fs))
            {
                var json = JsonConvert.SerializeObject(config);
                var encryptedText = GetEncryptor().EncryptText(json);

                sw.Write(encryptedText);
            }
        }
    }
}
