﻿using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace RtspViewer.WinUi.Models
{
    public partial class Settings : ObservableObject
    {
        [ObservableProperty]
        private bool isLightTheme;

        public Settings()
        {
            // Required for serialization.
        }
    }
}