using iRacingSdkWrapper;
using SharpOverlay.Utilities;
using System.Collections.Generic;

namespace SharpOverlay.Models
{
    public class Session
    {
        public Session(YamlQuery yaml)
        {
            ParseSession(yaml);

            //TODO Result Lists
        }

        private void ParseSession(YamlQuery yaml)
        {
            SessionNum = int.Parse(yaml[nameof(SessionNum)].Value);
            SessionLaps = yaml[nameof(SessionLaps)].Value;
            SessionTime = float.Parse(StringCleaner.ExtractNumbers(yaml[nameof(SessionTime)].Value));
            SessionNumLapsToAvg = int.Parse(yaml[nameof(SessionNumLapsToAvg)].Value);
            SessionType = yaml[nameof(SessionType)].Value;
            SessionTrackRubberState = yaml[nameof(SessionTrackRubberState)].Value;
            SessionName = yaml[nameof(SessionName)].Value;
            SessionSubType = yaml[nameof(SessionSubType)].Value;
            SessionSkipped = int.Parse(yaml[nameof(SessionSkipped)].Value);
            SessionRunGroupsUsed = int.Parse(yaml[nameof(SessionRunGroupsUsed)].Value);
            SessionEnforceTireCompoundChange = int.Parse(yaml[nameof(SessionEnforceTireCompoundChange)].Value);
            ResultsAverageLapTime = float.Parse(yaml[nameof(ResultsAverageLapTime)].Value);
            ResultsNumCautionFlags = int.Parse(yaml[nameof(ResultsNumCautionFlags)].Value);
            ResultsNumCautionLaps = int.Parse(yaml[nameof(ResultsNumCautionLaps)].Value);
            ResultsNumLeadChanges = int.Parse(yaml[nameof(ResultsNumLeadChanges)].Value);
            ResultsLapsComplete = int.Parse(yaml[nameof(ResultsLapsComplete)].Value);
            ResultsOfficial = int.Parse(yaml[nameof(ResultsOfficial)].Value);
        }

        public int SessionNum { get; set; }
        public string SessionLaps { get; set; }
        public float SessionTime { get; set; }
        public int SessionNumLapsToAvg { get; set; }
        public string SessionType { get; set; }
        public string SessionTrackRubberState { get; set; }
        public string SessionName { get; set; }
        public string SessionSubType { get; set; }
        public int SessionSkipped { get; set; }
        public int SessionRunGroupsUsed { get; set; }
        public int SessionEnforceTireCompoundChange { get; set; }
        public List<DriverPosition> ResultsPositions { get; set; } = new();
        public List<FastestLapDTO> ResultsFastestLap { get; set; } = new();
        public float ResultsAverageLapTime { get; set; }
        public int ResultsNumCautionFlags { get; set; }
        public int ResultsNumCautionLaps { get; set; }
        public int ResultsNumLeadChanges { get; set; }
        public int ResultsLapsComplete { get; set; }
        public int ResultsOfficial { get; set; }
    }
}
