namespace SharpOverlay.Models
{
    public class FuelViewModel
    {
        public int LapsCompleted { get; set; }
        public int TotalRaceLaps { get; set; }
        public float CurrentFuelLevel { get; set; }
        public float ConsumedFuel { get; set; }
        public float AverageFuelConsumption { get; set; }
        public double RefuelRequired { get; set; }
        public double LapsOfFuelRemaining { get; set; }
        public float FiveLapAverage { get; set; }
        public float FiveLapRefuelRequired { get; set; }
        public float FiveLapLapsOfFuelRemaining { get; set; }
    }
}
