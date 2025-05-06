using System;
using System.Collections.Generic;

namespace SharpOverlay.Services.FuelServices.LapServices
{
    public class TimeKeeper
    {
        private readonly List<TimeSpan> _lapTimes = new List<TimeSpan>();

        private TimeSpan _previousTimeAtLine = TimeSpan.Zero;
        private TimeSpan _currentTimeAtLine = TimeSpan.Zero;

        public TimeSpan GetLapTime()
        {
            var lapTime = _previousTimeAtLine - _currentTimeAtLine;

            if (lapTime > TimeSpan.Zero)
                return lapTime;

            return TimeSpan.Zero;
        }

        public void MarkTime(TimeSpan time)
        {
            _previousTimeAtLine = _currentTimeAtLine;
            _currentTimeAtLine = time;
        }
    }
}
