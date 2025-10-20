using Core.Models;
using iRacingSdkWrapper;
using iRacingSdkWrapper.Bitfields;

namespace Core.Utilities.Telemetries
{
    public interface ITelemetryParser
    {
        public int PlayerCarIdx { get; }
        public int PlayerCarClassId { get; }
        public double PlayerPctOnTrack { get; }
        Dictionary<int, int> PositionCarIdxInClass { get; }
        Dictionary<int, int> PositionCarIdxInRace { get; }
        Dictionary<int, int> CarIdxLastPitLap { get; }
        int CurrentSessionNumber { get; }
        bool HasSwitchedSessions { get; }
        float[] CarIdxPctOnTrack { get; }

        void Clear();
        void ParseCurrentSessionNumber(TelemetryOutputDTO telemetry);
        void ParsePlayerCarClassId(TelemetryOutputDTO telemetry);
        void ParsePlayerCarIdx(TelemetryOutputDTO telemetry);
        void ParsePlayerPctOnTrack(TelemetryOutputDTO telemetry);
        void ParsePositionCarIdxInPlayerClass(TelemetryOutputDTO telemetry, int paceCarIdx);
        void ParsePositionCarIdxForWholeRace(TelemetryOutputDTO telemetry, int paceCarIdx);
        SessionFlags GetSessionFlag(TelemetryOutputDTO telemetry);
        void ParseCarIdxOnTrack(TelemetryOutputDTO telemetry);
        void ParseCarIdxLastPitLap(TelemetryOutputDTO telemetry, int paceCarIdx);
    }
}
