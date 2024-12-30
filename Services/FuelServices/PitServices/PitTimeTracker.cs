using System;

namespace SharpOverlay.Services.FuelServices.PitServices
{
    public class PitTimeTracker
    {
        private TimeSpan _pitDuration = TimeSpan.Zero;
        private TimeSpan _timeAtPitStart = TimeSpan.Zero;

        public bool IsTrackingTime { get; private set; }

        public TimeSpan GetPitDuration()
            => _pitDuration;

        public void StartPitDurationTracking(TimeSpan timeLeft)
        {
            _timeAtPitStart = timeLeft;
            IsTrackingTime = true;
        }

        public void StopPitDurationTracking(TimeSpan timeLeft)
        {
            if (_timeAtPitStart > TimeSpan.Zero)
            {
                _pitDuration = _timeAtPitStart - timeLeft;
                _timeAtPitStart = TimeSpan.Zero;
            }

            IsTrackingTime = false;
        }
    }
}
