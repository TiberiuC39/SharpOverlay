using SharpOverlay.Models;
using System;
using System.Collections.Generic;

namespace SharpOverlay.Services.LapServices
{
    public interface ILapTracker : IClear
    {
        void StartNewLap(int lapNumber, double fuelLevel);
        void CompleteCurrentLap(double fuelLevel, TimeSpan lapTime);

        Lap? GetCurrentLap();
        List<Lap> GetCompletedLaps();
        int GetCompletedLapsCount();
        void ResetCurrentLap();
    }
}
