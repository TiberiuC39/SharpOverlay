using iRacingSdkWrapper;
using iRacingSdkWrapper.Bitfields;
using System.Collections.Generic;

namespace SharpOverlay.Utilities.Telemetries
{
    public interface ITelemetryParser
    {
        public int PlayerCarIdx { get; }
        public int PlayerCarClassId { get; }
        public double PlayerPctOnTrack { get; }
        Dictionary<int, int> PositionCarIdxInClass { get; }
        Dictionary<int, int> PositionCarIdxInRace { get; }
        int CurrentSessionNumber { get; }
        bool HasSwitchedSessions { get; }
        float[] CarIdxPctOnTrack { get; }

        void Clear();
        void ParseCurrentSessionNumber(TelemetryInfo telemetry);
        void ParsePlayerCarClassId(TelemetryInfo telemetry);
        void ParsePlayerCarIdx(TelemetryInfo telemetry);
        void ParsePlayerPctOnTrack(TelemetryInfo telemetry);
        void ParsePositionCarIdxInPlayerClass(TelemetryInfo telemetry, int paceCarIdx);
        void ParsePositionCarIdxForWholeRace(TelemetryInfo telemetry, int paceCarIdx);
        SessionFlags GetSessionFlag(TelemetryInfo telemetry);
        void ParseCarIdxOnTrack(TelemetryInfo telemetry);
    }
}
