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
    }
}
