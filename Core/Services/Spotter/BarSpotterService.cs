using Core.Events;
using Core.Models;

namespace Core.Services.Spotter
{
    public class BarSpotterService : IClear, IDisposable
    {
        private const int _carLengthInM = 5;
        private const int _outOfFrameOffset = 1;
        private readonly Dictionary<int, Driver> _drivers = [];
        private Driver _me = new();
        private double _trackLengthInM;
        private Driver? _closest;
        private double _offset = _outOfFrameOffset;
        private bool _disposed;

        public ISimReader SimReader { get; private set; }

        public BarSpotterService() : this(new SimReader())
        {
            SimReader.OnTelemetryUpdated += OnTelemetry;
            SimReader.OnSessionUpdated += OnSession;
            SimReader.OnDisconnected += OnDisconnect;
        }

        public BarSpotterService(ISimReader simReader)
        {
            SimReader = simReader;
            SimReader.OnTelemetryUpdated += OnTelemetry;
            SimReader.OnSessionUpdated += OnSession;
            SimReader.OnDisconnected += OnDisconnect;
        }

        public event EventHandler<BarSpotterEventArgs>? OnBarUpdated;

        public void OnDisconnect(object? sender, EventArgs e)
        {
            Clear();
        }

        public void Clear()
        {
            _drivers.Clear();
            _me = new Driver();
            _trackLengthInM = 0;
            _closest = null;
            _offset = _outOfFrameOffset;
        }

        private void OnSession(object? sender, SessionEventArgs e)
        {
            if (_trackLengthInM == 0)
            {
                _trackLengthInM = e.SessionOutput.WeekendInfo.TrackLength * 1000;
            }

            ParseDrivers(e.SessionOutput);
        }

        private void ParseDrivers(SessionOutputDTO e)
        {
            foreach (var racer in e.Drivers)
            {
                if (!_drivers.TryGetValue(racer.CarIdx, out _))
                {
                    var driver = new Driver()
                    {
                        CarIdx = racer.CarIdx
                    };

                    if (driver.CarIdx == e.Player.DriverCarIdx)
                    {
                        _me = driver;
                    }
                    else if (driver.CarIdx != e.Player.PaceCarIdx)
                    {
                        _drivers.Add(driver.CarIdx, driver);
                    }
                }
            }
        }

        private void OnTelemetry(object? sender, TelemetryEventArgs e)
        {
            var driverTrackPct = e.TelemetryOutput.CarIdxTrackDistPct;

            CalculateRelativeDistanceForAllDrivers(driverTrackPct);

            _closest = FindClosest();

            var distancePerPercentOfTrack = _trackLengthInM / 100;

            _offset = CalculateOffset(_closest.RelativeLapDistancePct, distancePerPercentOfTrack);

            var centeredOffset = GetOffsetInPercentage();

            OnBarUpdated?.Invoke(this, new BarSpotterEventArgs(centeredOffset, e.TelemetryOutput.CarLeftRight));
        }

        private double CalculateOffset(float closestRelativePct, double distancePerPercentOfTrack)
        {
            var distanceToClosestInM = closestRelativePct * distancePerPercentOfTrack;
            var absoluteDistanceToClosest = Math.Abs(distanceToClosestInM);

            if (absoluteDistanceToClosest <= _carLengthInM)
            {
                return distanceToClosestInM / _carLengthInM;
            }

            return _outOfFrameOffset;
        }

        private Driver FindClosest()
        {
            Driver closest;
            if (_drivers.Count > 0)
            {
                closest = _drivers.MinBy(d => Math.Abs(d.Value.RelativeLapDistancePct)).Value;
            }
            else
            {
                closest = new Driver()
                {
                    RelativeLapDistancePct = 2
                };
            }

            return closest;
        }

        private void CalculateRelativeDistanceForAllDrivers(float[] driverTrackPct)
        {
            _me.LapDistancePct = driverTrackPct[_me.CarIdx];
            double twoCarLenghtsPctOfTrack = (_carLengthInM / _trackLengthInM) * 2;

            foreach ((int driverIdx, Driver driver) in _drivers)
            {
                driver.LapDistancePct = driverTrackPct[driverIdx];

                bool isAroundFinishLine = _me.LapDistancePct <= twoCarLenghtsPctOfTrack || _me.LapDistancePct >= 1 - twoCarLenghtsPctOfTrack;

                driver.RelativeLapDistancePct = driver.LapDistancePct - _me.LapDistancePct;

                if (isAroundFinishLine)
                {
                    if (_me.LapDistancePct <= twoCarLenghtsPctOfTrack && driver.LapDistancePct >= 1 - twoCarLenghtsPctOfTrack)
                    {
                        driver.RelativeLapDistancePct = driver.LapDistancePct - (_me.LapDistancePct + 1);
                    }
                    else if (_me.LapDistancePct >= 1 - twoCarLenghtsPctOfTrack && driver.LapDistancePct <= twoCarLenghtsPctOfTrack)
                    {
                        driver.RelativeLapDistancePct = (driver.LapDistancePct + 1) - _me.LapDistancePct;
                    }
                }
            }
        }

        public double GetOffsetInPercentage()
        {
            return _offset * 100;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (SimReader != null)
                {
                    SimReader.OnTelemetryUpdated -= OnTelemetry;
                    SimReader.OnSessionUpdated -= OnSession;
                    SimReader.OnDisconnected -= OnDisconnect;

                    (SimReader as IDisposable)?.Dispose();
                }
            }

            _disposed = true;
        }
    }
}
