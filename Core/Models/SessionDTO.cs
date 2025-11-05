using iRacingSdkWrapper.JsonModels;

namespace Core.Models
{
    public class SessionDTO
    {

        public int SessionNum { get; set; }

        public string SessionLaps { get; set; }

        public double SessionTime { get; set; }

        public int SessionNumLapsToAvg { get; set; }

        public string SessionType { get; set; }

        public string SessionTrackRubberState { get; set; }

        public string SessionName { get; set; }

        public string SessionSubType { get; set; }

        public int SessionSkipped { get; set; }

        public int SessionRunGroupsUsed { get; set; }

        public int SessionEnforceTireCompoundChange { get; set; }

        public List<DriverPositionDTO> ResultsPositions { get; set; } = new List<DriverPositionDTO>();

        public List<FastestLapDTO> ResultsFastestLap { get; set; } = new List<FastestLapDTO>();

        public double ResultsAverageLapTime { get; set; }

        public int ResultsNumCautionFlags { get; set; }

        public int ResultsNumCautionLaps { get; set; }

        public int ResultsNumLeadChanges { get; set; }

        public int ResultsLapsComplete { get; set; }

        public int ResultsOfficial { get; set; }

        public SessionDTO()
        {
        }

        public SessionDTO(Session s)
        {
            SessionNum = s.SessionNum;
            SessionLaps = s.SessionLaps;
            SessionTime = s.SessionTime;
            SessionNumLapsToAvg = s.SessionNumLapsToAvg;
            SessionType = s.SessionType;
            SessionTrackRubberState = s.SessionTrackRubberState;
            SessionName = s.SessionName;
            SessionSubType = s.SessionSubType;
            SessionSkipped = s.SessionSkipped;
            SessionRunGroupsUsed = s.SessionRunGroupsUsed;
            SessionEnforceTireCompoundChange = s.SessionEnforceTireCompoundChange;
            ResultsPositions = s.ResultsPositions is not null ? s.ResultsPositions.Select(p => new DriverPositionDTO(p)).ToList() : new List<DriverPositionDTO>();
            ResultsFastestLap = s.ResultsFastestLap is not null ? s.ResultsFastestLap.Select(l => new FastestLapDTO(l)).ToList() : new List<FastestLapDTO>();
            ResultsAverageLapTime = s.ResultsAverageLapTime;
            ResultsNumCautionFlags = s.ResultsNumCautionFlags;
            ResultsNumCautionLaps = s.ResultsNumCautionLaps;
            ResultsNumLeadChanges = s.ResultsNumLeadChanges;
            ResultsLapsComplete = s.ResultsLapsComplete;
            ResultsOfficial = s.ResultsOfficial;
        }
    }
}


