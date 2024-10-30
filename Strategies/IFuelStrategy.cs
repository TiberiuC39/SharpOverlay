using SharpOverlay.Models;
using SharpOverlay.Services;
using System.Collections.Generic;

namespace SharpOverlay.Strategies
{
    public interface IFuelStrategy : IClear
    {
        void Calculate(List<Lap> lapsCompleted, int sessionLapsRemaining);
        void UpdateRefuel(double currentFuelLevel, int sessionLapsRemaining);
        void UpdateLapsOfFuelRemaining(double currentFuelLevel);
        StrategyViewModel GetView();
    }
}
