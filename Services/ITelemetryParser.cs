using iRacingSdkWrapper;
using iRacingSdkWrapper.Bitfields;
using SharpOverlay.Models;
using System;
using System.Collections.Generic;

namespace SharpOverlay.Services
{
    public interface ITelemetryParser
    {
        public int PlayerCarIdx { get; }
        public int PlayerCarClassId { get; }
        Dictionary<int, int> PositionCarIdxInClass { get; }
        Dictionary<int, int> PositionCarIdxInRace { get; }
        int CurrentSessionNumber { get; }
        bool HasSwitchedSessions { get; }
        SessionStates SessionState { get; }

        void Clear();
        TimeSpan GetTimeRemaining(TelemetryInfo telemetry);
        void ParseSessionState(TelemetryInfo telemetry);
        void ParseCurrentSessionNumber(TelemetryInfo telemetry);
        void ParsePlayerCarClassId(TelemetryInfo telemetry);
        void ParsePlayerCarIdx(TelemetryInfo telemetry);
        void ParsePositionCarIdxInPlayerClass(TelemetryInfo telemetry, SessionType sessionType);
        SessionFlags GetSessionFlag(TelemetryInfo telemetry);
    }
}
