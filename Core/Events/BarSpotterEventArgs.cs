using static Core.Models.Enums;

namespace Core.Events;

public class BarSpotterEventArgs : EventArgs
{
    public double Offset { get; }
    public Spotter CarPos { get; }

    public BarSpotterEventArgs(double offset, Spotter carPos)
    {
        Offset = offset;
        CarPos = carPos;
    }
}
