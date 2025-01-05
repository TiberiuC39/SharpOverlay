using SharpOverlay.Services.Base;
using System;

namespace SharpOverlay.Events
{
    public class WindowStateEventArgs : EventArgs
    {
        public WindowStateEventArgs(FuelWindowState windowState)
        {
            IsOpen = windowState.State;
        }

        public bool IsOpen { get; set; }
    }
}