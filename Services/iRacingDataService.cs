using iRacingSdkWrapper;

namespace SharpOverlay.Services
{
    public class iRacingDataService
    {
        public static readonly SdkWrapper Wrapper = new SdkWrapper();

        public int Tickrate => 60;

        public iRacingDataService() {
            Wrapper.TelemetryUpdateFrequency = Tickrate;
        }
    }
}
