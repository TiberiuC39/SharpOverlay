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

        protected override double GetAverageFuelConsumption(Dictionary<int, Lap> lapsCompleted)
            => lapsCompleted.Values.TakeLast(5).Average(l => l.FuelUsed);
    }
}
