using SharpOverlay.Models;
using System;

namespace SharpOverlay.Services.LapServices
{
    public class LapCountCalculator : ILapCountCalculator
    {
        public int CalculateLapsRemaining(float leaderPctOnTrack, TimeSpan timeRemainingInSession, TimeSpan averageLapTime)
        {
            TimeSpan leaderTimeToCompleteLap = (1 - leaderPctOnTrack) * averageLapTime;

            return (int)Math.Ceiling((timeRemainingInSession - leaderTimeToCompleteLap) / averageLapTime) + 1;
        }

        public int CalculateLapsRemaining(int sessionLaps, int completedLaps)
            => sessionLaps - completedLaps;
    }
}
