using iRacingSdkWrapper.Bitfields;
using System;

namespace SharpOverlay.Services.FuelServices.LapServices
{
    public class LapCountCalculator
    {
        public int CalculateLapsRemaining(float driverPctOnTrack, TimeSpan timeRemainingInSession, TimeSpan averageLapTime)
        {
            if (averageLapTime > TimeSpan.Zero && 
                timeRemainingInSession > TimeSpan.Zero)
            {
                TimeSpan timeToCompleteLap = (1 - driverPctOnTrack) * averageLapTime;

                double lapsBeforeRounding = (timeRemainingInSession - timeToCompleteLap) / averageLapTime + 1;

                int lapsRemaining = (int) Math.Ceiling(lapsBeforeRounding);

                return lapsRemaining;
            }

            return default;
        }

        public int CalculateLapsRemaining(int sessionLaps, int completedLaps)
            => sessionLaps - completedLaps;

        public int CalculateLapsRemainingMultiClass(TimeSpan timeLeftInSession,
            float raceLeaderPctOnTrack, float playerPctOnTrack,
            TimeSpan avgTimeRaceLeader, TimeSpan avgTimePlayer, SessionFlags flag)
        {
            if (avgTimeRaceLeader <= TimeSpan.Zero)
            {
                return 0;
            }

            var timeToCompleteLapLeader = (1 - raceLeaderPctOnTrack) * avgTimeRaceLeader;

            var timeRemainingAfterLineCross = timeLeftInSession - timeToCompleteLapLeader;
            
            if (timeRemainingAfterLineCross <= TimeSpan.Zero)
            {
                if (flag == SessionFlags.Green)
                {
                    return 2;
                }

                return 1;
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
