using System;

namespace SharpOverlay.Services.LapServices
{
    public class LapCountCalculator
    {
        public int CalculateLapsRemaining(float leaderPctOnTrack, TimeSpan timeRemainingInSession, TimeSpan averageLapTime)
        {
            if (averageLapTime > TimeSpan.Zero)
            {
                TimeSpan leaderTimeToCompleteLap = (1 - leaderPctOnTrack) * averageLapTime;

                return (int)Math.Ceiling((timeRemainingInSession - leaderTimeToCompleteLap) / averageLapTime) + 1;
            }

            return default;
        }

        public int CalculateLapsRemaining(int sessionLaps, int completedLaps)
            => sessionLaps - completedLaps;
    }
}
