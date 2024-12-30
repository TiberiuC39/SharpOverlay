using iRacingSdkWrapper;
using System;

namespace SharpOverlay.Services.Base
{
    public class SimReader 
    {
        private readonly SdkWrapper _sdkWrapper;

        public int DriverId => _sdkWrapper.DriverId;

        public SimReader(int tickRate = 60)
        {
            _sdkWrapper = new SdkWrapper();
            AdjustTickRate(tickRate);
            
            _sdkWrapper.Connected += ExecuteOnConnected;
            _sdkWrapper.Disconnected += ExecuteOnDisconnected;
            _sdkWrapper.TelemetryUpdated += ExecuteOnTelemetry;
            _sdkWrapper.SessionUpdated += ExecuteOnSession;

            _sdkWrapper.Start();
        }

        public void AdjustTickRate(int newTickRate)
        {
            _sdkWrapper.TelemetryUpdateFrequency = newTickRate;
        }

        public SessionInfo GetSessionInfo()
        {
            return _sdkWrapper.GetSessionInfoWithoutEvent();
        }

        public TelemetryInfo GetTelemetryInfo()
        {
            return _sdkWrapper.GetTelemetryInfoWithoutEvent();
        }

        public event EventHandler? OnConnected;
        public event EventHandler? OnDisconnected;
        public event EventHandler<SdkWrapper.TelemetryUpdatedEventArgs>? OnTelemetryUpdated;
        public event EventHandler<SdkWrapper.SessionUpdatedEventArgs>? OnSessionUpdated;

        protected virtual void ExecuteOnConnected(object? sender, EventArgs args)
        {
            OnConnected?.Invoke(this, args);
        }

        protected virtual void ExecuteOnDisconnected(object? sender, EventArgs args)
        {
            OnDisconnected?.Invoke(this, args);
        }

        protected virtual void ExecuteOnTelemetry(object? sender, SdkWrapper.TelemetryUpdatedEventArgs e)
        {
            OnTelemetryUpdated?.Invoke(this, e);
        }

        protected virtual void ExecuteOnSession(object? sender, SdkWrapper.SessionUpdatedEventArgs e)
        {
            OnSessionUpdated?.Invoke(this, e);
        }
    }
}
