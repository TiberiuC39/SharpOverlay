using SharpOverlay.Models;
using System;
using System.Collections.Generic;

namespace SharpOverlay.Services.FuelServices.LapServices
{
    public class LapTracker : IClear
    {
        private readonly List<Lap> _completedLaps = [];
        private Lap? _currentLap;

        public void StartNewLap(int lapNumber, double startingFuelLevel)
        {
            var newLap = new Lap(lapNumber, startingFuelLevel);

            _currentLap = newLap;
        }

        public void CompleteCurrentLap(double endingFuelLevel, TimeSpan lapTime)
        {
            _currentLap!.EndingFuel = endingFuelLevel;

            _currentLap.Time = lapTime;
            _currentLap.FuelUsed = _currentLap.StartingFuel - _currentLap.EndingFuel;

            _completedLaps.Add(_currentLap);
        }

        public Lap? GetCurrentLap()
            => _currentLap;

        public void ResetCurrentLap()
        {
            _currentLap = null;
        }

        public List<Lap> GetPlayerLaps()
            => _completedLaps;

        public int GetCompletedLapsCount()
            => _completedLaps.Count - 1;

        public void Clear()
        {
            _completedLaps.Clear();
            ResetCurrentLap();
        }

        public void StartWithHistory(int lapNumber, FuelModel entry)
        {
            StartNewLap(lapNumber, entry.Consumption);
            CompleteCurrentLap(0, TimeSpan.FromSeconds(entry.LapTime));
        }
    }
}
