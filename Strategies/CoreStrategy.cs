using SharpOverlay.Models;
using System.Collections.Generic;
using System.Linq;

namespace SharpOverlay.Strategies
{
    public abstract class CoreStrategy : IFuelStrategy
    {
        private readonly double _fuelCutOff;
        protected CoreStrategy(string name, double fuelCutOff)
        {
            Name = name;
            _fuelCutOff = fuelCutOff;
        }

        public string Name { get; }

        public double FuelConsumption { get; private set; }

        public double LapsOfFuelRemaining { get; private set; }

        public double RefuelRequired { get; private set; }

        public double FuelAtEnd { get; private set; }

        public void Calculate(Dictionary<int, Lap> lapsCompleted, int sessionLapsRemaining)
        {
            FuelConsumption = GetAverageFuelConsumption(lapsCompleted);

            double fuelRequired = sessionLapsRemaining * FuelConsumption;          

            Lap lastLap = lapsCompleted.Last().Value;

            FuelAtEnd = lastLap.EndingFuel - fuelRequired;

            RefuelRequired = fuelRequired - lastLap.EndingFuel;

            if (FuelAtEnd > 0 && FuelAtEnd < _fuelCutOff)
            {
                double difference = _fuelCutOff - FuelAtEnd;
                RefuelRequired += difference;
            }
        }

        protected virtual double GetAverageFuelConsumption(Dictionary<int, Lap> lapsCompleted)
            => lapsCompleted.Values.Average(l => l.FuelUsed);
    }
}
