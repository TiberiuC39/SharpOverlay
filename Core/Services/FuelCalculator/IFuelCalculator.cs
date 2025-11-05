using Core.Events;

namespace Core.Services.FuelCalculator
{
    public interface IFuelService : IDisposable
    {
        ISimReader SimReader { get; }

        event EventHandler<FuelEventArgs> FuelUpdated;
    }
}
