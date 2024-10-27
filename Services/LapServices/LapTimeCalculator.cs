using SharpOverlay.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpOverlay.Services.LapServices
{
    public class LapTimeCalculator : ILapTimeCalculator
    {
        public TimeSpan CalculateLapTime(List<Lap> driversLaps)
        {
            var validLaps = driversLaps.Where(l => l.Time > TimeSpan.Zero);

            if (validLaps.Any())
            {
                return TimeSpan.FromSeconds(validLaps
                    .Average(l => l.Time.TotalSeconds));
            }

            return TimeSpan.Zero;
        }
    }
}
