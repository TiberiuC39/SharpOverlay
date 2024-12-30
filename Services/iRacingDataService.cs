using iRacingSdkWrapper;
using System;

namespace SharpOverlay.Services
{
    public class iRacingDataService
    {
        private const int DefaultTickRate = 60;

        public event EventHandler<EventArgs> OnEnterExitCar = null!;

        public SdkWrapper Wrapper { get; private set; }
        public int PlayerId => Wrapper.DriverId;
        public bool IsConnected => Wrapper.IsConnected;
        public iRacingDataService(int tickrate = DefaultTickRate)
        {
            Wrapper = new SdkWrapper();
            AdjustTickRate(tickrate);
            Wrapper.Start();
        }

        public SessionInfo GetSessionInfo()
        {
            return Wrapper.GetSessionInfoWithoutEvent();
        }

        public TelemetryInfo GetTelemetryInfo()
        {
            return Wrapper.GetTelemetryInfoWithoutEvent();
        }

        private void AdjustTickRate(int tickrate)
        {
            if (tickrate > 0 && tickrate <= DefaultTickRate)
            {
                Wrapper.TelemetryUpdateFrequency = tickrate;
            }
            else
            {
                Wrapper.TelemetryUpdateFrequency = DefaultTickRate;
            }
        }

        public void HookUpToTelemetryEvent(Action<object?, SdkWrapper.TelemetryUpdatedEventArgs> action)
        {
            Wrapper.TelemetryUpdated += (sender, e) => action(sender, e);
        }

        public void HookUpToSessionEvent(Action<object?, SdkWrapper.SessionUpdatedEventArgs> action)
        {
            Wrapper.SessionUpdated += (sender, e) => action(sender, e);
        }

        internal void HookUpToConnectedEvent(Action<object?, EventArgs> action)
        {
            Wrapper.Connected += (sender, e) => action(sender, e);
        }

        internal void HookUpToDisconnectedEvent(Action<object?, EventArgs> action)
        {
            Wrapper.Disconnected += (sender, e) => action(sender, e);
        }
    }
}
