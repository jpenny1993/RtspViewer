using System;
using System.Collections.Generic;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using RtspViewer.Configuration;
using RtspViewer.WinUi.Converters;
using RtspViewer.WinUi.Models;

namespace RtspViewer.WinUi.ViewModels
{
    public partial class StreamViewModel : ObservableObject
    {
        private const string RtspPrefix = "rtsp://";
        private const string HttpPrefix = "http://";

        public static List<EnumListItemSource<ConnectionType>> Protocols { get; } = EnumListItemSource<ConnectionType>.GetListItems();

        public static List<EnumListItemSource<StreamScale>> Scales { get; } = EnumListItemSource<StreamScale>.GetListItems();

        [ObservableProperty]
        private string _identifier;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _address;

        [ObservableProperty]
        private ConnectionType _protocol;

        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private bool _autoStart;

        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(AllowManualScaling))]
        private StreamScale _scale;

        public bool AllowManualScaling => _scale == StreamScale.Manual;

        [ObservableProperty]
        private int _manualWidth;

        [ObservableProperty]
        private int _manualHeight;

        [ObservableProperty]
        private float _volume;

        public StreamViewModel()
        {
            _identifier = Guid.NewGuid().ToString();
            _name = string.Empty;
            _address = string.Empty;
            _protocol = ConnectionType.TCP;
            _username = string.Empty;
            _password = string.Empty;
            _autoStart = false;
            _scale = StreamScale.None;
            _manualWidth = 0;
            _manualHeight = 0;
            _volume = 100;
        }

        public StreamViewModel(StreamModel model)
        {
            _identifier = model.Identifier;
            _name = model.Name;
            _address = model.Address;
            _protocol = model.Protocol;
            _username = model.Username;
            _password = model.Password;
            _autoStart = model.AutoStart;
            _scale = model.Scale;
            _manualWidth = model.ManualWidth;
            _manualHeight = model.ManualHeight;
            _volume = model.Volume;
        }

        public StreamViewModel Clone()
        {
            return new StreamViewModel
            {
                _identifier = Identifier,
                _name = Name,
                _address = Address,
                _protocol = Protocol,
                _username = Username,
                _password = Password,
                _autoStart = AutoStart,
                _scale = Scale,
                _manualWidth = ManualWidth,
                _manualHeight = ManualHeight,
                _volume = Volume
            };
        }

        public static StreamViewModel FromModel(StreamModel model)
        { 
            return new StreamViewModel(model);
        }

        public static StreamModel ToModel(StreamViewModel viewModel) 
        {
            var streamAddress = viewModel.Address;
            if (!string.IsNullOrWhiteSpace(streamAddress) &&
                !streamAddress.StartsWith(RtspPrefix) &&
                !streamAddress.StartsWith(HttpPrefix))
            {
                streamAddress = RtspPrefix + streamAddress;
            }

            return new StreamModel
            {
                Identifier = viewModel.Identifier,
                Name = viewModel.Name,
                Address = streamAddress,
                Protocol = viewModel.Protocol,
                Username = viewModel.Username,
                Password = viewModel.Password,
                AutoStart = viewModel.AutoStart,
                Scale = viewModel.Scale,
                ManualWidth = viewModel.ManualWidth,
                ManualHeight = viewModel.ManualHeight,
                Volume = viewModel.Volume
            };
        }
    }
}
