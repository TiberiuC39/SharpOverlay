using System;

namespace SharpOverlay.Services
{
    public interface IFuelCalculator
    {
        event EventHandler<FuelEventArgs> FuelUpdated;
    }
}
