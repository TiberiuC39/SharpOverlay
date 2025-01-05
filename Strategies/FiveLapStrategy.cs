using SharpOverlay.Models;
using System.Collections.Generic;
using System.Linq;

namespace SharpOverlay.Strategies
{
    public class FiveLapStrategy : CoreStrategy
    {
        private const string _name = "5L";

        public FiveLapStrategy(double fuelCutOff)
            :base(_name, fuelCutOff)
        {            
        }

        protected override double GetAverageFuelConsumption(List<Lap> lapsCompleted)
            => lapsCompleted.Count > 5 ? lapsCompleted.TakeLast(5).Average(l => l.FuelUsed) : base.GetAverageFuelConsumption(lapsCompleted);
    }
}
