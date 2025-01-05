using iRacingSdkWrapper;

namespace SharpOverlay.Services.Base
{
    public class FuelWindowState 
    {
        public FuelWindowState(bool startingState)
        {
            Update(startingState);
        }

        public bool State { get; private set; }

        public bool RequiresChange { get; private set; }

        public void UpdateOnEvent(SdkWrapper.TelemetryUpdatedEventArgs eventArgs)
        {
            bool isCarOnTrack = eventArgs.TelemetryInfo.IsOnTrack.Value;

            Update(isCarOnTrack);
        }

        public void ConfirmChange()
        {
            RequiresChange = false;
        }

        private void Update(bool state)
        {
            if (State == !state)
            {
                RequiresChange = true;
                State = state;
            }
        }
    }
}