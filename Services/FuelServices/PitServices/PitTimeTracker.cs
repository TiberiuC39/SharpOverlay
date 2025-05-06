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

        public void Start(TimeSpan timeLeft)
        {
            _timeAtPitStart = timeLeft;
            IsTrackingTime = true;
        }

        public void Stop(TimeSpan timeLeft)
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
