using Presentation.Services;

namespace Presentation.Events
{
    public class WindowStateEventArgs : EventArgs
    {
        public WindowStateEventArgs(TrackedWindowState windowState)
        {
            IsOpen = windowState.IsOpen;
            IsEnabled = windowState.IsEnabled;
            IsInTestMode = windowState.IsInTestMode;
            IsInDebugMode = windowState.IsInDebugMode;
        }

        public bool IsOpen { get; }
        public bool IsEnabled { get; }
        public bool IsInTestMode { get; }
        public bool IsInDebugMode { get; }
    }
}
