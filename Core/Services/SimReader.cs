using Core.Events;
using Core.Utilities;
using iRacingSdkWrapper;

namespace Core.Services
{
    public class SimReader
    {
        private readonly SdkWrapper _sdkWrapper;

        public int DriverId => _sdkWrapper.DriverId;

        public SimReader(int tickRate = DefaultTickRates.Default)
        {
            _sdkWrapper = new SdkWrapper();
            AdjustTickRate(tickRate);

            _sdkWrapper.Connected += ExecuteOnConnected;
            _sdkWrapper.Disconnected += ExecuteOnDisconnected;
            _sdkWrapper.TelemetryUpdated += ExecuteOnTelemetry;
            _sdkWrapper.SessionUpdated += ExecuteOnSession;

            _sdkWrapper.Start();
        }

        public SimReader(string filePath, int tickRate = DefaultTickRates.Default)
        {
            _sdkWrapper = new SdkWrapper(filePath);
            AdjustTickRate(tickRate);

            _sdkWrapper.Connected += ExecuteOnConnected;
            _sdkWrapper.Disconnected += ExecuteOnDisconnected;
            _sdkWrapper.TelemetryUpdated += ExecuteOnTelemetry;
            _sdkWrapper.SessionUpdated += ExecuteOnSession;
        }

        public bool ReadNextFrame() {
            return _sdkWrapper.ProcessTelemetryFrame();
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
        public event EventHandler<TelemetryEventArgs>? OnTelemetryUpdated;
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
            var eventArgs = new TelemetryEventArgs(e.TelemetryInfo);
            OnTelemetryUpdated?.Invoke(this, eventArgs);
        }

        protected virtual void ExecuteOnSession(object? sender, SdkWrapper.SessionUpdatedEventArgs e)
        {
            OnSessionUpdated?.Invoke(this, e);
        }
    }
}
