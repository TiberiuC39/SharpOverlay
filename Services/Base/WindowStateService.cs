using iRacingSdkWrapper;
using SharpOverlay.Events;
using SharpOverlay.Models;
using System;
using System.ComponentModel;

namespace SharpOverlay.Services.Base
{
    public class WindowStateService
    {
        private readonly WindowState _windowState;
        public event EventHandler<WindowStateEventArgs>? WindowStateChanged;

        public WindowStateService(SimReader reader, BaseSettings settings)
        {
            settings.PropertyChanged += ExecuteOnProperty;

            reader.OnTelemetryUpdated += ExecuteOnTelemetry;

            _windowState = new WindowState(settings);
        }

        public void ExecuteOnTelemetry(object? sender, SdkWrapper.TelemetryUpdatedEventArgs args)
        {
            _windowState.Update(args);

            RaiseEventIfNewData();
        }

        public void ExecuteOnProperty(object? sender, PropertyChangedEventArgs args)
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
