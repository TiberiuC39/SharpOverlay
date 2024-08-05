using iRacingSdkWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
