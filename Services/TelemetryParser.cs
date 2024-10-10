using iRacingSdkWrapper;
using iRacingSdkWrapper.Bitfields;
using SharpOverlay.Models;
using System;
using System.Collections.Generic;

namespace SharpOverlay.Services
{
    public class TelemetryParser
    {
        public int PlayerCarClassId { get; private set; }
        public int PlayerCarIdx { get; private set; }
        public int CurrentSessionNumber { get; private set; }
        public Dictionary<int, int> PositionCarIdxInClass { get; private set; } = [];
        public Dictionary<int, int> PositionCarIdxInRace { get; private set; } = [];

        public void ParsePositionCarIdxInPlayerClass(TelemetryInfo telemetry, SessionType sessionType = SessionType.Practice)
        {
            int firstRacerIndex = 0;

            if (sessionType == SessionType.Race)
            {
                firstRacerIndex = 1;
            }

            var carIdxClass = telemetry.CarIdxClass.Value;
            var carIdxPositions = telemetry.CarIdxPosition.Value;

            for (int idx = firstRacerIndex; idx < carIdxClass.Length; idx++)
            {
                if (carIdxClass[idx] == PlayerCarClassId)
                {
                    var currentPosition = carIdxPositions[idx];

                    if (currentPosition != 0)
                    {
                        if (!PositionCarIdxInClass.ContainsKey(currentPosition))
                        {
                            PositionCarIdxInClass.Add(currentPosition, idx);
                        }
                        else
                        {
                            PositionCarIdxInClass[currentPosition] = idx;
                        }
                    }
                }
            }
        }

        public TimeSpan GetTimeRemaining(TelemetryInfo telemetry)
        {
            double timeRemaining = telemetry.SessionTimeRemain.Value;

            return TimeSpan.FromSeconds(timeRemaining);
        }

        public void ParsePositionCarIdxForWholeRace(TelemetryInfo telemetry)
        {
            throw new NotImplementedException();
        }

        public void ParsePlayerCarIdx(TelemetryInfo telemetry)
        {
            int playerIdx = telemetry.PlayerCarIdx.Value;

            PlayerCarIdx = playerIdx;
        }

        public void ParsePlayerCarClassId(TelemetryInfo telemetry)
        {
            int playerCarClass = telemetry.CarIdxClass.Value[PlayerCarIdx];

            PlayerCarClassId = playerCarClass;
        }

        public void ParseCurrentSessionNumber(TelemetryInfo telemetry)
        {
            int currentSessionNumber = telemetry.SessionNum.Value;

            CurrentSessionNumber = currentSessionNumber;
        }

        public static Dictionary<int, TimeSpan> GetDriversLastLapTime(TelemetryInfo telemetry, SessionType sessionType = SessionType.Practice)
        {
            int firstDriverIndex = 0;

            if (sessionType == SessionType.Race)
            {
                // 0 is pace car IN RACE
                firstDriverIndex = 1;
            }

            var lapTimes = telemetry.CarIdxLastLapTime.Value;
            var driversLastLaps = new Dictionary<int, TimeSpan>();

            for (int idx = firstDriverIndex; idx < lapTimes.Length; idx++)
            {
                float lapTime = lapTimes[idx];

                driversLastLaps.Add(idx, TimeSpan.FromSeconds(lapTime));
            }

            return driversLastLaps;
        }

        public static SessionFlags GetSessionFlag(TelemetryInfo telemetry)
        {
            var flag = telemetry.SessionFlags.Value;

            return (SessionFlags) flag.Value;
        }

        public void Clear()
        {
            CurrentSessionNumber = 0;
            PlayerCarClassId = 0;
            PlayerCarClassId = 0;
            PositionCarIdxInClass.Clear();
            PositionCarIdxInRace.Clear();            
        }
    }
}
