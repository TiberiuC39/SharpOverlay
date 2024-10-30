using SharpOverlay.Models;
using System;
using System.Collections.Generic;

namespace SharpOverlay.Services.LapServices
{
    public interface ILapTimeCalculator
    {
        TimeSpan CalculateLapTime(List<Lap> driversLaps);
    }
}
