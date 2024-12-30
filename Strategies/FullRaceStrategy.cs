using SharpOverlay.Models;
using System.Collections.Generic;
using System.Linq;

namespace SharpOverlay.Strategies
{
    public class FullRaceStrategy : CoreStrategy
    {
        private const string _name = "FULL";

        public FullRaceStrategy(double fuelCutOff)
            : base(_name, fuelCutOff)
        {            
        }

        protected override double GetAverageFuelConsumption(List<Lap> lapsCompleted)
            => lapsCompleted.Count > 1 ? lapsCompleted.Skip(1).Average(l => l.FuelUsed) : base.GetAverageFuelConsumption(lapsCompleted);
    }
}
