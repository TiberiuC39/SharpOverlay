using iRacingSdkWrapper;
using System;

namespace SharpOverlay.Models
{
    public class FuelViewModel
    {
        public int LapsCompleted { get; set; }
        public double RaceLapsRemaining { get; set; }
        public float CurrentFuelLevel { get; set; }
        public float ConsumedFuel { get; set; }
        public float AverageFuelConsumption { get; set; }
        public double RefuelRequired { get; set; }
        public bool DoesRequireRefueling => RefuelRequired > 0;
        public double LapsOfFuelRemaining { get; set; }
        public float FiveLapAverage { get; set; }
        public float FiveLapRefuelRequired { get; set; }
        public float FiveLapLapsOfFuelRemaining { get; set; }


        public Lap CurrentLap { get; set; }
        public double LapsRemainingInRace { get; set; }
        public bool IsInService { get; set; }
        public bool HasBegunService { get; set; }
        public TimeSpan AverageLapTime { get; set; }
        public bool HasCompletedService { get; set; }
        public float AvgFuelPerLap { get; set; }
        public bool IsRollingStart { get; set; }
        public bool HasResetToPits { get; set; }
        public bool IsRaceStart { get; set; }
        public TimeSpan LeaderAvgLapTime { get; set; }
        public TimeSpan LeaderTimeToCompleteLap { get; set; }
        public TimeSpan EstLapTime { get; set; }
        public int CurrentSessionNumber { get; set; }
        public int LeaderIdx { get; set; }
        public int PlayerIdx { get; set; }
        public bool IsOnPitRoad { get; set; }
        public TrackSurfaces TrackSurface { get; set; }
    }
}
