namespace Core.Services.FuelCalculator.PitServices
{
    public class PitTimeTracker
    {
        private TimeSpan _pitDuration = TimeSpan.Zero;
        private TimeSpan _timeAtPitStart = TimeSpan.Zero;
        private List<TimeSpan> _pitStopDurations = new List<TimeSpan>();

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

                _pitStopDurations.Add(_pitDuration);
            }

            IsTrackingTime = false;
        }

        public TimeSpan GetAvgPitStopTime()
        {
            if (_pitStopDurations.Count > 0)
            {
                return TimeSpan.FromSeconds(_pitStopDurations.Average(t => t.TotalSeconds));
            }

            return TimeSpan.Zero;
        }

        public void Reset()
        {
            _timeAtPitStart = TimeSpan.Zero;
            IsTrackingTime = false;
        }
    }
}
