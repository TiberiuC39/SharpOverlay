using Core.Models;
using iRacingSdkWrapper.Bitfields;

namespace Core.Utilities.Telemetries
{
    public class TelemetryParser : ITelemetryParser
    {
        public int PlayerCarClassId { get; private set; }
        public int PlayerCarIdx { get; private set; }
        public int CurrentSessionNumber { get; private set; }
        public bool HasSwitchedSessions { get; private set; }
        public Dictionary<int, int> PositionCarIdxInClass { get; private set; } = [];
        public Dictionary<int, int> PositionCarIdxInRace { get; private set; } = [];

        public double PlayerPctOnTrack { get; private set; }
        public float[] CarIdxPctOnTrack { get; private set; } = null!;

        public Dictionary<int, int> CarIdxLastPitLap => throw new NotImplementedException();

        public void ParsePositionCarIdxForWholeRace(TelemetryOutputDTO telemetry, int paceCarIdx)
        {
            var carIdxPositions = telemetry.CarIdxPosition;

            for (int idx = 0; idx < carIdxPositions.Length; idx++)
            {
                if (idx == paceCarIdx)
                    continue;

                var currentPosition = carIdxPositions[idx];

                if (currentPosition > 0)
                {
                    if (!PositionCarIdxInRace.TryAdd(currentPosition, idx))
                    {
                        PositionCarIdxInRace[currentPosition] = idx;
                    }
                }
            }
        }

        public void ParsePositionCarIdxInPlayerClass(TelemetryOutputDTO telemetry, int paceCarIdx)
        {
            var carIdxClass = telemetry.CarIdxClass;
            var carIdxPositions = telemetry.CarIdxPosition;

            for (int idx = 0; idx < carIdxClass.Length; idx++)
            {
                if (idx == paceCarIdx)
                    continue;

                if (carIdxClass[idx] == PlayerCarClassId)
                {
                    var currentPosition = carIdxPositions[idx];

                    if (currentPosition > 0)
                    {
                        if (!PositionCarIdxInClass.TryAdd(currentPosition, idx))
                        {
                            PositionCarIdxInClass[currentPosition] = idx;
                        }
                    }
                }
            }
        }

        public void ParseCarIdxOnTrack(TelemetryOutputDTO telemetry)
        {
            CarIdxPctOnTrack = telemetry.CarIdxTrackDistPct;
        }



        public void ParsePlayerCarIdx(TelemetryOutputDTO telemetry)
        {
            int playerIdx = telemetry.PlayerCarIdx;

            PlayerCarIdx = playerIdx;
        }

        public void ParsePlayerCarClassId(TelemetryOutputDTO telemetry)
        {
            int playerCarClass = telemetry.CarIdxClass[PlayerCarIdx];

            PlayerCarClassId = playerCarClass;
        }

        public void ParseCurrentSessionNumber(TelemetryOutputDTO telemetry)
        {
            int currentSessionNumber = telemetry.SessionNum;

            if (CurrentSessionNumber != currentSessionNumber)
            {
                HasSwitchedSessions = true;
            }
            else if (HasSwitchedSessions)
            {
                HasSwitchedSessions = false;
            }

            CurrentSessionNumber = currentSessionNumber;
        }

        public static Dictionary<int, TimeSpan> GetDriversLastLapTime(int paceCarIdx, float[] lapTimes)
        {
            var driversLastLaps = new Dictionary<int, TimeSpan>();

            for (int idx = 0; idx < lapTimes.Length; idx++)
            {
                if (idx == paceCarIdx)
                    continue;

                float lapTime = lapTimes[idx];

                driversLastLaps.Add(idx, TimeSpan.FromSeconds(lapTime));
            }

            return driversLastLaps;
        }

        public SessionFlags GetSessionFlag(TelemetryOutputDTO telemetry)
        {
            var flag = telemetry.SessionFlag;

            return (SessionFlags)flag;
        }

        public void Clear()
        {
            PlayerCarIdx = -1;
            PlayerCarClassId = 0;
            PositionCarIdxInClass.Clear();
            PositionCarIdxInRace.Clear();
            HasSwitchedSessions = false;
            PlayerPctOnTrack = 0;
            CurrentSessionNumber = 0;
        }

        public void ParsePlayerPctOnTrack(TelemetryOutputDTO telemetry)
        {
            PlayerPctOnTrack = telemetry.CarIdxTrackDistPct[PlayerCarIdx];
        }

        public void ParseCarIdxLastPitLap(TelemetryOutputDTO telemetry, int paceCarIdx)
        {
            throw new NotImplementedException();
        }
    }
}
