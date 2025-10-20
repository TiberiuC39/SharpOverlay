using Core.Models;
using iRacingSdkWrapper;

namespace Core.Events;

public class TelemetryEventArgs : EventArgs
{
    public readonly TelemetryOutputDTO TelemetryOutput;

    public TelemetryEventArgs(TelemetryInfo telemetryInfo)
    {
        TelemetryOutput = new TelemetryOutputDTO(telemetryInfo);
    }
}
