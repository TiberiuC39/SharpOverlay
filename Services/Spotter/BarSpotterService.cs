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
            _centerOffset = 1;
        }

        private void OnSession(object? sender, SdkWrapper.SessionUpdatedEventArgs e)
        {
            if (_trackLengthInM == 0)
            {
                _trackLengthInM = e.SessionInfo.WeekendInfo.TrackLength * 1000;
            }

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
            var driverLapPct = e.TelemetryInfo.CarIdxLapDistPct.Value;

            _me.LapDistance = driverLapPct[_me.CarIdx];

            foreach (var driver in _drivers)
            {
                driver.LapDistance = driverLapPct[driver.CarIdx];
                driver.RelativeLapDistance = driver.LapDistance - _me.LapDistance;
            }

            _closest = _drivers.MinBy(d => Math.Abs(d.RelativeLapDistance)) ?? new Driver()
            {
                RelativeLapDistance = 2
            };

            var distancePerPercentOfTrack = _trackLengthInM / 100;

            var distanceToClosestInM = _closest.RelativeLapDistance * distancePerPercentOfTrack;
            var absoluteDistanceToClosest = Math.Abs(distanceToClosestInM);

            if (absoluteDistanceToClosest <= _carLengthInM)
            {
                _centerOffset = distanceToClosestInM / _carLengthInM;
            }
            else
            {
                _centerOffset = 1;
            }            
        }

        public double CenterOffset()
        {
            return _centerOffset * 100;
        }
    }
}
