using Core.Events;
using Core.Models;
using iRacingSdkWrapper;

namespace Core.Services.Spotter
{
    public class BarSpotterService : IClear
    {
        private const int _carLengthInM = 5;
        private const int _outOfFrameOffset = 1;
        private readonly Dictionary<int, Driver> _drivers = [];
        private Driver _me = new();
        private double _trackLengthInM;
        private Driver? _closest;
        private double _offset = _outOfFrameOffset;
        public SimReader SimReader { get; }

        public BarSpotterService()
        {
            SimReader = new SimReader();
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

        private void OnSession(object? sender, SdkWrapper.SessionUpdatedEventArgs e)
        {
            if (_trackLengthInM == 0)
            {
                _trackLengthInM = e.SessionInfo.WeekendInfo.TrackLength * 1000;
            }

            ParseDrivers(e);
        }

        private void ParseDrivers(SdkWrapper.SessionUpdatedEventArgs e)
        {
            foreach (var racer in e.SessionInfo.Drivers)
            {
                if (!_drivers.TryGetValue(racer.CarIdx, out _))
                {
                    var driver = new Driver()
                    {
                        CarIdx = racer.CarIdx
                    };

                    if (driver.CarIdx == e.SessionInfo.Player.DriverCarIdx)
                    {
                        _me = driver;
                    }
                    else if (driver.CarIdx != e.SessionInfo.Player.PaceCarIdx)
                    {
                        _drivers.Add(driver.CarIdx, driver);
                    }
                }
            }
        }

        private void OnTelemetry(object? sender, TelemetryEventArgs e)
        {
            var driverTrackPct = e.TelemetryOutput.CarIdxTrackDistPct;
            var driverLapNumbers = e.TelemetryOutput.CarIdxLapCompleted;

            CalculateRelativeDistanceForAllDrivers(driverTrackPct, driverLapNumbers);

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
            var closest = _drivers.MinBy(d => Math.Abs(d.Value.RelativeLapDistancePct));

            return closest.Value ?? new Driver()
            {
                RelativeLapDistancePct = 2
            };
        }

        private void CalculateRelativeDistanceForAllDrivers(float[] driverTrackPct, int[] driverLapNumbers)
        {
            _me.LapDistancePct = driverTrackPct[_me.CarIdx];
            _me.CurrentLap = driverLapNumbers[_me.CarIdx];

            foreach ((int driverIdx, Driver driver) in _drivers)
            {
                driver.CurrentLap = driverLapNumbers[driverIdx];
                driver.LapDistancePct = driverTrackPct[driverIdx];

                if (_me.CurrentLap == driver.CurrentLap)
                {
                    driver.RelativeLapDistancePct = driver.LapDistancePct - _me.LapDistancePct;
                }
                else if (_me.CurrentLap > driver.CurrentLap)
                {
                    driver.RelativeLapDistancePct = driver.LapDistancePct - (_me.LapDistancePct + 1);
                }
                else
                {
                    driver.RelativeLapDistancePct = (driver.LapDistancePct + 1) - _me.LapDistancePct;
                }
            }
        }

        public double GetOffsetInPercentage()
        {
            return _offset * 100;
        }
    }
}
