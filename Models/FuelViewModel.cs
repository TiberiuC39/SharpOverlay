namespace SharpOverlay.Models
{
    public class FuelViewModel
    {
        public int LapsCompleted { get; set; }
        public int TotalRaceLaps { get; set; }
        public float CurrentFuelLevel { get; set; }
        public float ConsumedFuel { get; set; }
        public float AverageFuelConsumption { get; set; }
        public float FuelConsumptionPerHour { get; set; }
        public float RefuelRequired { get; set; }
    }
}
