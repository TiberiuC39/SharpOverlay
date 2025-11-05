using System.ComponentModel;
using Core.Events;
using Core.Services;
using Presentation.Events;
using Presentation.Models;

namespace Presentation.Services
{
    public class WindowStateService : IDisposable
    {
        private bool _disposed;
        private readonly TrackedWindowState _windowState;
        private readonly ISimReader _reader;
        private readonly IBaseSettings _settings;
        public event EventHandler<WindowStateEventArgs>? WindowStateChanged;

        public WindowStateService(ISimReader reader, IBaseSettings settings)
        {
            _settings = settings;
            _settings.PropertyChanged += OnPropertyChange;
            _reader = reader;
            _reader.OnTelemetryUpdated += OnTelemetryChange;

            _windowState = new TrackedWindowState(settings);
        }

        public void Initialize()
        {
            RaiseEventIfNewData();
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
                _windowState.CompleteChange();
                RaiseEvent();
            }
        }

        private void RaiseEvent()
        {
            WindowStateChanged?.Invoke(this, new WindowStateEventArgs(_windowState));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_settings != null)
                {
                    _settings.PropertyChanged -= OnPropertyChange;
                }
                if (_reader != null)
                {
                    _reader.OnTelemetryUpdated -= OnTelemetryChange;
                }
            }

            _disposed = true;
        }
    }
}
