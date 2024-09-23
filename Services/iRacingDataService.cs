using iRacingSdkWrapper;
using System;

namespace SharpOverlay.Services
{
    public class iRacingDataService
    {
        private const int DefaultTickRate = 60;

        public static SdkWrapper Wrapper = new SdkWrapper();
        public int PlayerId => Wrapper.DriverId;
        public bool IsConnected => Wrapper.IsConnected;
        public iRacingDataService(int tickrate = DefaultTickRate)
        {
            AdjustTickRate(tickrate);
        }

        public SessionInfo GetSessionInfo()
        {
            return Wrapper.GetSessionInfoWithoutEvent().SessionInfo;
        }

        private static void AdjustTickRate(int tickrate)
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

        public EventHandler<SdkWrapper.TelemetryUpdatedEventArgs> OnTelemetryUpdate = null!;

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

        internal void DisconnectedEvent(Action<object?, EventArgs> action)
        {
            Wrapper.Disconnected += (sender, e) => action(sender, e);
        }
    }
}
