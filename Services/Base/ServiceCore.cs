using SharpOverlay.Events;
using SharpOverlay.Models;
using System.Windows;

namespace SharpOverlay.Services.Base
{
    public abstract class ServiceCore<T> where T : Settings
    {
        private readonly BaseSettings _settings;
        private readonly Window _window;

        protected ServiceCore(WindowStateService windowStateService, BaseSettings settings, Window window)
        {
            _settings = settings;
            _window = window;
            windowStateService.WindowStateChanged += OnWindowStateChanged;
        }

        private void OnWindowStateChanged(object? sender, WindowStateEventArgs eventArgs)
        {
            if (eventArgs.IsOpen && eventArgs.IsEnabled)
            {
                _window.Show();

                //if (_settings.IsInTestMode)
                //    HandleTestMode(_settings.IsInTestMode);
            }
            else
            {
                _window.Hide();
            }
        }
    }
}
