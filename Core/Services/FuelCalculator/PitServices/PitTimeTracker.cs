namespace Core.Services.FuelCalculator.PitServices
{
    public class PitTimeTracker
    {
        private TimeSpan _pitDuration = TimeSpan.Zero;
        private TimeSpan _timeAtPitStart = TimeSpan.Zero;
        private readonly List<TimeSpan> _pitStopDurations = [];

        private TimeSpan _serviceStart = TimeSpan.Zero;
        private TimeSpan _serviceDuration = TimeSpan.Zero;
        private readonly List<TimeSpan> _serviceDurations = [];

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
                _pitDuration = _timeAtPitStart - timeLeft - _serviceDuration;
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

        public void StartService(TimeSpan timeLeft)
        {
            _serviceStart = timeLeft;
        }

        public void StopService(TimeSpan timeLeft)
        {
            if (_serviceStart > TimeSpan.Zero)
            {
                _serviceDuration = _serviceStart - timeLeft;
                _serviceStart = TimeSpan.Zero;

                _serviceDurations.Add(_serviceDuration);
            }
        }

        public void Reset()
        {
            _timeAtPitStart = TimeSpan.Zero;
            _serviceStart = TimeSpan.Zero;
            IsTrackingTime = false;
        }

        public TimeSpan GetServiceDuration()
        {
            if (_serviceDurations.Count > 0)
            {
                return _serviceDurations.Last();
            }

            return TimeSpan.Zero;
        }
    }
}
