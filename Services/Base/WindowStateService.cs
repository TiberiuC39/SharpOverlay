using iRacingSdkWrapper;
using SharpOverlay.Events;
using System;

namespace SharpOverlay.Services.Base
{
    public class WindowStateService
    {
        private readonly FuelWindowState _windowState;
        public event EventHandler<WindowStateEventArgs>? WindowStateChanged;

        public WindowStateService()
        {
            bool startingState = App.appSettings.FuelSettings.IsOpen;
            _windowState = new(startingState);
        }

        public void ExecuteOnTelemetry(object? sender, SdkWrapper.TelemetryUpdatedEventArgs args)
        {
            UpdateWindowState(args);

            RaiseEventIfNewData();
        }

        private void UpdateWindowState(SdkWrapper.TelemetryUpdatedEventArgs args)
        {
            _windowState.UpdateOnEvent(args);
        }

        private void RaiseEventIfNewData()
        {
            if (_windowState.RequiresChange)
            {
                RaiseEvent();
                _windowState.ConfirmChange();
            }
        }
        private void RaiseEvent()
        {
            WindowStateChanged?.Invoke(this, new WindowStateEventArgs(_windowState));
        }

    }
}
