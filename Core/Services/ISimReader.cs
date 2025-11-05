using Core.Events;
using Core.Models;

namespace Core.Services
{
    public interface ISimReader : IDisposable
    {
        bool ReadNextFrame();

        void AdjustTickRate(int newTickRate);

        SessionOutputDTO GetSessionInfo();

        TelemetryOutputDTO GetTelemetryInfo();

        event EventHandler? OnConnected;
        event EventHandler? OnDisconnected;
        event EventHandler<TelemetryEventArgs>? OnTelemetryUpdated;
        event EventHandler<SessionEventArgs>? OnSessionUpdated;
    }
}
