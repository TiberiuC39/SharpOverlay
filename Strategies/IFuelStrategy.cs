using SharpOverlay.Models;
using System.Collections.Generic;

namespace SharpOverlay.Strategies
{
    public interface IFuelStrategy
    {
        string Name { get; }
        double FuelConsumption { get; }
        double LapsOfFuelRemaining { get; }
        double RefuelRequired { get; }
        double FuelAtEnd { get; }

        void Calculate(Dictionary<int, Lap> lapsCompleted, int sessionLapsRemaining);
    }
}
