using Core.Events;
using Core.Models;
using Core.Utilities;
using iRacingSdkWrapper;

namespace Core.Services
{
    public class SimReader : ISimReader 
    {
        private readonly SdkWrapper _sdkWrapper;
        private bool _disposed;

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

        public bool ReadNextFrame()
        {
            return _sdkWrapper.ProcessTelemetryFrame();
        }

        public void AdjustTickRate(int newTickRate)
        {
            _sdkWrapper.TelemetryUpdateFrequency = newTickRate;
        }

        public SessionOutputDTO GetSessionInfo()
        {
            return new SessionOutputDTO(_sdkWrapper.GetSessionInfoWithoutEvent());
        }

        public TelemetryOutputDTO GetTelemetryInfo()
        {
            return new TelemetryOutputDTO(_sdkWrapper.GetTelemetryInfoWithoutEvent());
        }

        public event EventHandler? OnConnected;
        public event EventHandler? OnDisconnected;
        public event EventHandler<TelemetryEventArgs>? OnTelemetryUpdated;
        public event EventHandler<SessionEventArgs>? OnSessionUpdated;

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
            var eventArgs = new SessionEventArgs(e.SessionInfo);
            OnSessionUpdated?.Invoke(this, eventArgs);
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
                _sdkWrapper.Stop();

                if (_sdkWrapper != null)
                {
                    _sdkWrapper.Connected -= ExecuteOnConnected;
                    _sdkWrapper.Disconnected -= ExecuteOnDisconnected;
                    _sdkWrapper.TelemetryUpdated -= ExecuteOnTelemetry;
                    _sdkWrapper.SessionUpdated -= ExecuteOnSession;
                }
            }

            // No unmanaged resources to free.

            _disposed = true;
        }
    }
}
