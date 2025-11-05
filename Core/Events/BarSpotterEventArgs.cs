using static Core.Models.Enums;

namespace Core.Events
{
    public class BarSpotterEventArgs : EventArgs
    {
        public double OffsetPct { get; }
        public Spotter CarPos { get; }

        public BarSpotterEventArgs(double offset, Spotter carPos)
        {
            OffsetPct = offset;
            CarPos = carPos;
        }
    }
}
