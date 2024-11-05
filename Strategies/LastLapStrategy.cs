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

        protected override double GetAverageFuelConsumption(List<Lap> lapsCompleted)
            => lapsCompleted.Count > 0 ? lapsCompleted.Last().FuelUsed : 0;
    }
}
