using Windows.Media.Audio;

namespace SharpOverlay.Models
{
    public class FuelViewModel
    {
        public float CurrentFuelLevel { get; private set; }
        public float ConsumedFuel { get; private set; }
        public float AverageFuelConsumption { get; private set; }
        public float FuelConsumptionPerHour { get; private set; }

        public static FuelViewModel Create(Car car)
        {
            return new FuelViewModel
            {
                CurrentFuelLevel = car._fuel.FuelLevel,
                ConsumedFuel = car._fuel.FuelConsumed,
                AverageFuelConsumption = 0,
                FuelConsumptionPerHour = car.FuelConsumptionPerHour,
            };
        }
    }
}
