using iRacingSdkWrapper;
using SharpOverlay.Models;
using SharpOverlay.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpOverlay.Services.Spotter
{
    public class BarSpotterService : IClear
    {
        private const int _carLengthInM = 5;
        private const int _outOfFrameOffset = 1;
        private readonly List<Driver> _drivers = [];
        private Driver _me = new Driver();
        private double _trackLengthInM;
        private Driver? _closest;
        private double _centerOffset = 1;

        public BarSpotterService(SimReader simReader)
        {
            simReader.OnTelemetryUpdated += OnTelemetry;
            simReader.OnSessionUpdated += OnSession;
        }

        public void Clear()
        {
            _drivers.Clear();
            _me = new Driver();
            _trackLengthInM = 0;
            _closest = null;
            _centerOffset = _outOfFrameOffset;
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
            _drivers.Clear();

            foreach (var racer in e.SessionInfo.Drivers)
            {
                var driver = new Driver()
                {
                    CarIdx = racer.CarIdx,
                    Name = racer.UserName
                };

                if (driver.CarIdx == e.SessionInfo.Player.DriverCarIdx)
                {
                    _me = driver;
                }
                else if (driver.Name != "Pace Car")
                {
                    _drivers.Add(driver);
                }
            }
        }

        private void OnTelemetry(object? sender, SdkWrapper.TelemetryUpdatedEventArgs e)
        {
            var driverTrackPct = e.TelemetryInfo.CarIdxLapDistPct.Value;

            CalculateRelativeDistanceForAllDrivers(driverTrackPct);

            _closest = FindClosest();

            var distancePerPercentOfTrack = _trackLengthInM / 100;

            _centerOffset = CalculateOffset(_closest.RelativeLapDistancePct, distancePerPercentOfTrack);
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
            var closest = _drivers.MinBy(d => Math.Abs(d.RelativeLapDistancePct));
            
            return closest ?? new Driver()
            {
                RelativeLapDistancePct = 2
            };
        }

        private void CalculateRelativeDistanceForAllDrivers(float[] driverTrackPct)
        {
            _me.LapDistancePct = driverTrackPct[_me.CarIdx];

            foreach (var driver in _drivers)
            {
                driver.LapDistancePct = driverTrackPct[driver.CarIdx];
                driver.RelativeLapDistancePct = driver.LapDistancePct - _me.LapDistancePct;
            }
        }

        public double CenterOffset()
        {
            return _centerOffset * 100;
        }
    }
}
