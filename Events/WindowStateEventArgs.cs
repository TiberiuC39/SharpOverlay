using SharpOverlay.Services.Base;
using System;

namespace SharpOverlay.Events
{
    public class WindowStateEventArgs : EventArgs
    {
        public WindowStateEventArgs()
        {
            IsOpen = true;
        }

        public WindowStateEventArgs(FuelWindowState windowState)
        {
            IsOpen = windowState.State;
        }

        public WindowStateEventArgs(WindowStateEventArgs priorEventArgs, bool newState)
        {
            IsOpen = newState;

            if (IsOpen != priorEventArgs.IsOpen)
            {
                ShouldChange = true;
            }
        }

        public bool IsOpen { get; set; }
        public bool ShouldChange { get; set; }
    }
}