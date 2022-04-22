﻿using System;
using System.Windows.Forms;
using RtspViewer.Configuration;

namespace RtspViewer.Forms.Windows
{
    public partial class ConfigForm : Form
    {
        private const string RtspPrefix = "rtsp://";
        private const string HttpPrefix = "http://";

        public event EventHandler<StreamConfiguration> ConfigurationUpdated;

        public ConfigForm()
        {
            InitializeComponent();
        }

        public void InitialiseFields(StreamConfiguration config)
        {
            if (config != null)
            {
                txtAddress.Text = config.Address;
                txtUsername.Text = config.Username;
                txtPassword.Text = config.Password;
                cbxProtocol.SelectedItem = config.Protocol.ToString();
            }
            else
            {
                cbxProtocol.SelectedItem = "TCP";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string streamAddress = txtAddress.Text;

            if (!streamAddress.StartsWith(RtspPrefix) && !streamAddress.StartsWith(HttpPrefix))
            {
                streamAddress = RtspPrefix + streamAddress;
            }

            if (!Uri.TryCreate(streamAddress, UriKind.Absolute, out Uri deviceUri))
            {
                lblError.Text = "Invalid device address";
                lblError.Visible = true;
                return;
            }

            lblError.Visible = false;
            ConfigurationUpdated(this, new StreamConfiguration
            {
                Address = streamAddress,
                Username = txtUsername.Text,
                Password = txtPassword.Text,
                Protocol = (ConnectionType) cbxProtocol.SelectedIndex
            });

            Close();
        }
    }
}
