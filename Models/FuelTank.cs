namespace SharpOverlay.Models
{
    public class FuelTank
    {
        public FuelTank(float fuelLevel, float fuelPct, float capacity)
        {
            FuelLevel = fuelLevel;
            FuelPct = fuelPct;
            Capacity = capacity;
        }

        public float FuelLevel { get; set; }
        public float FuelPct { get; set; }
        public float Capacity { get; private set; }
        public float FuelConsumed { get; private set; }
        public float AverageConsumptionPerHour { get; private set; }

        public float GetRaceAvgConsumption(int raceLaps)
            => FuelConsumed / raceLaps;

        public void UpdateFuel(float newFuelLevel, float fuelPct)
        {
            float consumption = FuelLevel - newFuelLevel;

            if (consumption > 0)
            {
                FuelConsumed += consumption;
            }

            FuelLevel = newFuelLevel;
        }
    }
}
