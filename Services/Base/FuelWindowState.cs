using iRacingSdkWrapper;

namespace SharpOverlay.Services.Base
{
    public class FuelWindowState 
    {
        public bool State { get; private set; }

        public bool RequiresChange { get; private set; }

        public void Update(SdkWrapper.TelemetryUpdatedEventArgs eventArgs)
        {
            bool isCarOnTrack = eventArgs.TelemetryInfo.IsOnTrack.Value;

            if (State == !isCarOnTrack)
            {
                RequiresChange = true;
                State = isCarOnTrack;
            }
        }

        public void ConfirmChange()
        {
            RequiresChange = false;
        }
    }
}