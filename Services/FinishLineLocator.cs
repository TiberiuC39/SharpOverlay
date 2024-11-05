using SharpOverlay.Utilities.Enums;

namespace SharpOverlay.Services
{
    public class FinishLineLocator : IClear
    {
        private FinishLineLocation _finishLineLocation = FinishLineLocation.Unknown;

        public void Clear()
        {
            _finishLineLocation = FinishLineLocation.Unknown;
        }

        public void DetermineFinishLineLocation(float driverTrackPct)
        {
            if (driverTrackPct > 0.9)
            {
                _finishLineLocation = FinishLineLocation.AfterPitRoad;
            }
            else
            {
                _finishLineLocation = FinishLineLocation.AlongPitRoad;
            }
        }

        public bool IsFinishLineKnown()
            => _finishLineLocation != FinishLineLocation.Unknown;

        public bool IsFinishLineAfterPits()
            => _finishLineLocation == FinishLineLocation.AfterPitRoad;
    }
}
