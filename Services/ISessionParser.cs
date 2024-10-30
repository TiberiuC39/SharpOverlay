using iRacingSdkWrapper;
using iRacingSdkWrapper.JsonModels;
using SharpOverlay.Models;
using System;
using System.Collections.Generic;

namespace SharpOverlay.Services
{
    public interface ISessionParser
    {
        Dictionary<int, Racer> Drivers { get; }
        SessionType SessionType { get; }
        List<Session> Sessions { get; }
        StartType StartType { get; }
        int SessionLaps { get; }
        int PaceCarIdx { get; }

        void Clear();
        TimeSpan GetBestLapTime(int leaderIdx, int currentSessionNumber);
        void ParseCurrentSessionType(SessionInfo sessionInfo, int currentSessionNumber);
        void ParseDrivers(SessionInfo sessionInfo);
        void ParsePaceCarIdx(SessionInfo sessionInfo);
        void ParseLapsInSession(SessionInfo sessionInfo, int currentSessionNumber);
        void ParseSessions(SessionInfo sessionInfo);
        void ParseStartType(SessionInfo sessionInfo);
    }
}
