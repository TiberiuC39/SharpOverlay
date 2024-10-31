using SharpOverlay.Models;
using System;

namespace SharpOverlay.Services.LapServices
{
    public interface ILapCountCalculator
    {
        int CalculateLapsRemaining(float leaderPctOnTrack, TimeSpan timeRemainingInSession, TimeSpan averageLapTime);
        int CalculateLapsRemaining(int sessionLaps, int completedLaps);
    }
}
