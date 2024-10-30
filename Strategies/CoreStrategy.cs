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

        private string Name { get; }

        private double FuelConsumption { get; set; }

        private double LapsOfFuelRemaining { get; set; }

        private double RefuelRequired { get; set; }

        private double FuelAtEnd { get; set; }

        public void Calculate(List<Lap> lapsCompleted, int sessionLapsRemaining)
        {
            FuelConsumption = GetAverageFuelConsumption(lapsCompleted);

            Lap lastLap = lapsCompleted.Last();

            double currentFuelLevel = lastLap.EndingFuel;

            UpdateRefuel(currentFuelLevel, sessionLapsRemaining);
        }

        public void UpdateRefuel(double currentFuelLevel, int sessionLapsRemaining)
        {
            if (sessionLapsRemaining == 0)
            {
                RefuelRequired = 0;
            }
            else if (FuelConsumption > 0)
            {
                double fuelRequired = sessionLapsRemaining * FuelConsumption;

                FuelAtEnd = currentFuelLevel - fuelRequired;

                RefuelRequired = fuelRequired - currentFuelLevel;

                if (FuelAtEnd > 0 && FuelAtEnd < _fuelCutOff)
                {
                    double difference = _fuelCutOff - FuelAtEnd;
                    RefuelRequired += difference;
                }
            }

            UpdateLapsOfFuelRemaining(currentFuelLevel);
        }

        public StrategyViewModel GetView()
            => new StrategyViewModel()
            {
                Name = Name,
                FuelAtEnd = FuelAtEnd,
                RefuelAmount = RefuelRequired,
                LapsOfFuelRemaining = LapsOfFuelRemaining,
                FuelConsumption = FuelConsumption
            };

        protected virtual double GetAverageFuelConsumption(List<Lap> lapsCompleted)
            => lapsCompleted.Count > 1 ? lapsCompleted.Skip(1).Average(l => l.FuelUsed) : default;

        public void UpdateLapsOfFuelRemaining(double currentFuelLevel)
        {
            if (FuelConsumption > 0)
            {
                LapsOfFuelRemaining = currentFuelLevel / FuelConsumption;
            }
        }

        public void Clear()
        {
            FuelAtEnd = 0;
            RefuelRequired = 0;
            LapsOfFuelRemaining = 0;
            FuelConsumption = 0;
        }
    }
}
