using System.ComponentModel;
using Core.Events;
using Core.Services;
using Presentation.Events;
using Presentation.Models;

namespace Presentation.Services
{
    public class WindowStateService
    {
        private readonly TrackedWindowState _windowState;
        public event EventHandler<WindowStateEventArgs>? WindowStateChanged;

        public WindowStateService(SimReader reader, BaseSettings settings)
        {
            settings.PropertyChanged += OnPropertyChange;
            reader.OnTelemetryUpdated += OnTelemetryChange;

            _windowState = new TrackedWindowState(settings);
        }

        public void OnTelemetryChange(object? sender, TelemetryEventArgs args)
        {
            _windowState.Update(args.TelemetryOutput.IsOnTrack);

            RaiseEventIfNewData();
        }

        public void OnPropertyChange(object? sender, PropertyChangedEventArgs args)
        {
            _windowState.Update(args);

            RaiseEventIfNewData();
        }

        private void RaiseEventIfNewData()
        {
            if (_windowState.RequiresChange)
            {
                RaiseEvent();
                _windowState.CompleteChange();
            }
        }
        private void RaiseEvent()
        {
            WindowStateChanged?.Invoke(this, new WindowStateEventArgs(_windowState));
        }
    }
}
