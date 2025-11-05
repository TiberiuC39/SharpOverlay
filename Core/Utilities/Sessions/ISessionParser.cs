using Core.Models;

namespace Core.Utilities.Sessions
{
    public interface ISessionParser
    {
        Dictionary<int, Driver> Drivers { get; }
        string EventType { get; }
        SessionType SessionType { get; }
        List<SessionDTO> Sessions { get; }
        StartType StartType { get; }
        int SessionLaps { get; }
        int PaceCarIdx { get; }
        bool IsMultiClassRace { get; }
        List<SectorDTO> Sectors { get; }
        int CarId { get; }
        int TrackId { get; }

        void Clear();
        TimeSpan GetBestLapTime(int leaderIdx, int currentSessionNumber);
        void ParseCurrentSessionType(SessionOutputDTO sessionInfo, int currentSessionNumber);
        void ParseRaceType(SessionOutputDTO sessionInfo);
        void ParseDrivers(SessionOutputDTO sessionInfo);
        void ParsePaceCarIdx(SessionOutputDTO sessionInfo);
        void ParseLapsInSession(SessionOutputDTO sessionInfo, int currentSessionNumber);
        void ParseSessions(SessionOutputDTO sessionInfo);
        void ParseStartType(SessionOutputDTO sessionInfo);
        void ParseSectors(SessionOutputDTO sessionInfo);
        void ParseTrackId(SessionOutputDTO sessionInfo);
        void ParseCarId(SessionOutputDTO sessionInfo);
        void ParseEventType(SessionOutputDTO sessionInfo);
    }
}
