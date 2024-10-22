using SharpOverlay.Models;
using System.Collections.Generic;
using System.Linq;

namespace SharpOverlay.Strategies
{
    public class LastLapStrategy : CoreStrategy
    {
        private const string _name = "LAST";

        public LastLapStrategy(double fuelCutOff)
            :base(_name, fuelCutOff)
        {            
        }

        protected override double GetAverageFuelConsumption(Dictionary<int, Lap> lapsCompleted)
            => lapsCompleted.Values.Last().FuelUsed;
    }
}
