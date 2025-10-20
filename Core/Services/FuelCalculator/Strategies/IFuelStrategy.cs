using Core.Models;

namespace Core.Services.FuelCalculator.Strategies
{
    public interface IFuelStrategy : IClear
    {
        void Calculate(List<Lap> lapsCompleted, int sessionLapsRemaining);
        void UpdateRefuel(double currentFuelLevel, int sessionLapsRemaining);
        void UpdateLapsOfFuelRemaining(double currentFuelLevel);

        bool RequiresRefueling();

        StrategyViewModel GetView();
    }
}
