
using iRacingSdkWrapper;
using System;

namespace SharpOverlay.Services
{
    public class iRacingTelemetryService
    {
        private readonly SdkWrapper _sdk;

        public iRacingTelemetryService()
        {
            _sdk = iRacingDataService.Wrapper;
        }

        public EventHandler<SdkWrapper.TelemetryUpdatedEventArgs> OnTelemetryUpdate;

        public void HookUpToTelemetryEvent(Action<object?, SdkWrapper.TelemetryUpdatedEventArgs> action)
        {
            _sdk.TelemetryUpdated += (sender, e) => action(sender, e);
        }        

        public void HookUpToSessionEvent(Action<object?, SdkWrapper.SessionUpdatedEventArgs> action)
        {
            _sdk.SessionUpdated += (sender, e) => action(sender, e);
        }
    }
}
