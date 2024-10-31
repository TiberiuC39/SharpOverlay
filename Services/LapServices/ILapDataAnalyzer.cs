using iRacingSdkWrapper.JsonModels;
using SharpOverlay.Models;
using System;
using System.Collections.Generic;

namespace SharpOverlay.Services.LapServices
{
    public interface ILapDataAnalyzer : IClear
    {
        void CollectAllDriversLaps(Dictionary<int, Racer> drivers, Dictionary<int, TimeSpan> lastLapTimes, int[] carIdxLapsCompleted);
        int FindLeaderIdxInClass(Dictionary<int, int> positionIdxInClass);
        Dictionary<int, List<Lap>> GetDriversLaps();
    }
}
