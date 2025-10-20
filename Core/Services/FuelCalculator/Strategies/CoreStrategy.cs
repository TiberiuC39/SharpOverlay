using Core.Models;

namespace Core.Services.FuelCalculator.Strategies
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

        public void Calculate(List<Lap> lapsCompleted, int sessionLapsRemaining)
        {
            FuelConsumption = GetAverageFuelConsumption(lapsCompleted);

            if (lapsCompleted.Count > 0)
            {
                Lap lastLap = lapsCompleted.Last();

                double currentFuelLevel = lastLap.EndingFuel;

                UpdateRefuel(currentFuelLevel, sessionLapsRemaining);
            }
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

                double fuelAtEnd = currentFuelLevel - fuelRequired;

                if (fuelAtEnd < _fuelCutOff)
                {
                    RefuelRequired = _fuelCutOff - fuelAtEnd;
                }
                else
                {
                    RefuelRequired = fuelRequired - currentFuelLevel;
                }
            }

            UpdateLapsOfFuelRemaining(currentFuelLevel);
        }

        public StrategyViewModel GetView()
            => new StrategyViewModel()
            {
                Name = Name,
                RefuelAmount = RefuelRequired,
                LapsOfFuelRemaining = LapsOfFuelRemaining,
                FuelConsumption = FuelConsumption
            };

        protected virtual double GetAverageFuelConsumption(List<Lap> lapsCompleted)
            => lapsCompleted.Count > 0 ? lapsCompleted.Last().FuelUsed : default;

        public void UpdateLapsOfFuelRemaining(double currentFuelLevel)
        {
            if (FuelConsumption > 0)
            {
                LapsOfFuelRemaining = (currentFuelLevel - _fuelCutOff) / FuelConsumption;
            }
            else
            {
                LapsOfFuelRemaining = 0;
            }
        }

        public void Clear()
        {
            RefuelRequired = 0;
            LapsOfFuelRemaining = 0;
            FuelConsumption = 0;
        }

        public bool RequiresRefueling()
            => RefuelRequired > 0;
    }
}
