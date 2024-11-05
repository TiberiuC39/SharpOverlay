using iRacingSdkWrapper.JsonModels;
using SharpOverlay.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpOverlay.Services.LapServices
{
    public class LapAnalyzer : IClear
    {
        private readonly Dictionary<int, List<Lap>> _driversLaps = [];

        public void Clear()
        {
            _driversLaps.Clear();
        }

        public void CollectAllDriversLaps(Dictionary<int, Racer> drivers, Dictionary<int, TimeSpan> lastLapTimes, int[] carIdxLapsCompleted)
        {
            foreach ((int idx, _) in drivers)
            {
                if (!_driversLaps.ContainsKey(idx))
                {
                    _driversLaps.Add(idx, new List<Lap>());
                }

                var laps = _driversLaps[idx];
                int? lastLapNumber = laps.LastOrDefault()?.Number;

                if (carIdxLapsCompleted[idx] > (lastLapNumber ?? 0))
                {
                    int lapNumber = carIdxLapsCompleted[idx];
                    var lapTime = lastLapTimes[idx];
                    laps.Add(new Lap(lapNumber, lapTime));
                }
            }
        }

        public int FindLeaderIdxInClass(Dictionary<int, int> positionIdxInClass)
        {
            const int invalidLeaderPosition = -1;

            int leaderInClassPosition = positionIdxInClass.Keys.Count > 0 ? positionIdxInClass.Keys.Min() : invalidLeaderPosition;

            if (leaderInClassPosition > invalidLeaderPosition)
            {
                return positionIdxInClass[leaderInClassPosition];
            }

            return -1;
        }

        public Dictionary<int, List<Lap>> GetDriversLaps()
            => _driversLaps;

        public TimeSpan GetLapTime(int carIdx)
        {
            if (carIdx < 0)
            {
                return TimeSpan.Zero;
            }

            var validLaps = _driversLaps[carIdx].Where(l => l.Time > TimeSpan.Zero);

            if (validLaps.Any())
            {
                return TimeSpan.FromSeconds(validLaps
                    .Average(l => l.Time.TotalSeconds));
            }

            return TimeSpan.Zero;
        }
    }
}
