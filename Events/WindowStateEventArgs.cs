using SharpOverlay.Services.Base;
using System;

namespace SharpOverlay.Events
{
    public class WindowStateEventArgs : EventArgs
    {
        public WindowStateEventArgs(WindowState windowState)
        {
            IsOpen = windowState.IsOpen;
            IsEnabled = windowState.IsEnabled;
            IsInTestMode = windowState.IsInTestMode;
        }

        public bool IsOpen { get; }
        public bool IsEnabled { get; }
        public bool IsInTestMode { get; }
    }
}