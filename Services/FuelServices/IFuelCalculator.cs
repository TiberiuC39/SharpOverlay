using System;

namespace SharpOverlay.Services.FuelServices
{
    public interface IFuelCalculator
    {
        event EventHandler<FuelEventArgs> FuelUpdated;
    }
}
