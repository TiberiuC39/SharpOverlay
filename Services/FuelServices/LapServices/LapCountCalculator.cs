using System;

namespace SharpOverlay.Services.FuelServices.LapServices
{
    public class LapCountCalculator
    {
        public int CalculateLapsRemaining(float driverPctOnTrack, TimeSpan timeRemainingInSession, TimeSpan averageLapTime)
        {
            if (timeRemainingInSession <= TimeSpan.Zero)
            {
                return 1;
            }
            else if (averageLapTime > TimeSpan.Zero)
            {
                TimeSpan timeToCompleteLap = (1 - driverPctOnTrack) * averageLapTime;
                return (int)Math.Ceiling((timeRemainingInSession - timeToCompleteLap) / averageLapTime) + 1;
            }

            return default;
        }

        public int CalculateLapsRemaining(int sessionLaps, int completedLaps)
            => sessionLaps - completedLaps;

        public int CalculateLapsRemainingMultiClass(TimeSpan timeLeftInSession,
            float raceLeaderPctOnTrack, float playerPctOnTrack,
            TimeSpan avgTimeRaceLeader, TimeSpan avgTimePlayer)
        {
            var timeToCompleteLapLeader = (1 - raceLeaderPctOnTrack) * avgTimeRaceLeader;

            var timeRemainingAfterLineCross = timeLeftInSession - timeToCompleteLapLeader;

            if (timeRemainingAfterLineCross == TimeSpan.Zero)
            {
                return -15;
            }
            else if (timeRemainingAfterLineCross < TimeSpan.Zero)
            {
                return -30;
            }
            else if (avgTimeRaceLeader > timeRemainingAfterLineCross)
            {
                timeLeftInSession += avgTimeRaceLeader - timeRemainingAfterLineCross;
            }

            int leaderLapsRemaining = CalculateLapsRemaining(raceLeaderPctOnTrack, timeLeftInSession, avgTimeRaceLeader);

            var timeRequiredForLeader = leaderLapsRemaining * avgTimeRaceLeader;

            int playerLapsRemaining = CalculateLapsRemaining(playerPctOnTrack, timeRequiredForLeader, avgTimePlayer);

            return playerLapsRemaining;
        }
    }
}
