using Core.Models;
using Core.Services.FuelCalculator.Strategies;

namespace SharpOverlay.Strategies
{
    public class FiveLapStrategy : CoreStrategy
    {
        private const string _name = "5L";

        public FiveLapStrategy(double fuelCutOff)
            : base(_name, fuelCutOff)
        {
        }

        protected override double GetAverageFuelConsumption(List<Lap> lapsCompleted)
            => lapsCompleted.Count > 5 ? lapsCompleted.TakeLast(5).Average(l => l.FuelUsed) : base.GetAverageFuelConsumption(lapsCompleted);
    }
}
