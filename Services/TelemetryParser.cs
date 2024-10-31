using iRacingSdkWrapper;
using iRacingSdkWrapper.Bitfields;
using SharpOverlay.Models;
using System;
using System.Collections.Generic;

namespace SharpOverlay.Services
{
    public class TelemetryParser : ITelemetryParser
    {
        public int PlayerCarClassId { get; private set; }
        public int PlayerCarIdx { get; private set; }
        public int CurrentSessionNumber { get; private set; }
        public bool HasSwitchedSessions { get; private set; }
        public Dictionary<int, int> PositionCarIdxInClass { get; private set; } = [];
        public Dictionary<int, int> PositionCarIdxInRace { get; private set; } = [];
        public int[] CarIdxLapCompleted { get; private set; } = null!;
        public SessionStates SessionState { get; private set; }

        public double PlayerPctOnTrack { get; private set; }
        public float[] CarIdxPctOnTrack { get; private set; } = null!;

        public double FuelLevel { get; private set; }

        public void ParseSessionState(TelemetryInfo telemetry)
        {
            SessionState = telemetry.SessionState.Value;
        }

        public void ParseCarIdxLapCompleted(TelemetryInfo telemetry)
        {
            CarIdxLapCompleted = telemetry.CarIdxLapCompleted.Value;
        }

        public void ParsePositionCarIdxInPlayerClass(TelemetryInfo telemetry, int paceCarIdx)
        {
            var carIdxClass = telemetry.CarIdxClass.Value;
            var carIdxPositions = telemetry.CarIdxPosition.Value;
            
            for (int idx = 0; idx < carIdxClass.Length; idx++)
            {
                if (idx == paceCarIdx)
                    continue;

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

        public void ParseCarIdxOnTrack(TelemetryInfo telemetry)
        {
            CarIdxPctOnTrack = telemetry.CarIdxLapDistPct.Value;
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

        public SessionFlags GetSessionFlag(TelemetryInfo telemetry)
        {
            var flag = telemetry.SessionFlags.Value;

            return (SessionFlags) flag.Value;
        }

        public void Clear()
        {
            PlayerCarIdx = -1;
            PlayerCarClassId = 0;
            SessionState = SessionStates.Invalid;
            PositionCarIdxInClass.Clear();
            PositionCarIdxInRace.Clear();
            HasSwitchedSessions = false;
            PlayerPctOnTrack = 0;
            FuelLevel = 0;
            CurrentSessionNumber = 0;
        }

        public void ParsePlayerPctOnTrack(TelemetryInfo telemetry)
        {
            PlayerPctOnTrack = telemetry.CarIdxLapDistPct.Value[PlayerCarIdx];
        }

        public void ParseFuelLevel(TelemetryInfo telemetry)
        {
            FuelLevel = (double) telemetry.FuelLevel.Value;
        }
    }
}
