using iRacingSdkWrapper;
using iRacingSdkWrapper.JsonModels;
using SharpOverlay.Models;
using System;
using System.Collections.Generic;

namespace SharpOverlay.Utilities.Sessions
{
    public interface ISessionParser
    {
        Dictionary<int, Racer> Drivers { get; }
        SessionType SessionType { get; }
        List<Session> Sessions { get; }
        StartType StartType { get; }
        int SessionLaps { get; }
        int PaceCarIdx { get; }
        bool IsMultiClassRace { get; }
        List<Sector> Sectors { get; }

        void Clear();
        TimeSpan GetBestLapTime(int leaderIdx, int currentSessionNumber);
        void ParseCurrentSessionType(SessionInfo sessionInfo, int currentSessionNumber);
        void ParseRaceType(SessionInfo sessionInfo);
        void ParseDrivers(SessionInfo sessionInfo);
        void ParsePaceCarIdx(SessionInfo sessionInfo);
        void ParseLapsInSession(SessionInfo sessionInfo, int currentSessionNumber);
        void ParseSessions(SessionInfo sessionInfo);
        void ParseStartType(SessionInfo sessionInfo);
        void ParseSectors(SessionInfo sessionInfo);
    }
}
