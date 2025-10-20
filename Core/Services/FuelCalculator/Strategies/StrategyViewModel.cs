namespace Core.Services.FuelCalculator.Strategies
{
    public class StrategyViewModel
    {
        public string Name { get; set; } = null!;
        public double FuelConsumption { get; set; }
        public double LapsOfFuelRemaining { get; set; }
        public double RefuelAmount { get; set; }
        public bool DoesRequireRefueling => RefuelAmount > 0;
    }
}
