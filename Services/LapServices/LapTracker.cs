using SharpOverlay.Models;
using System;
using System.Collections.Generic;

namespace SharpOverlay.Services.LapServices
{
    public class LapTracker : ILapTracker
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

        public List<Lap> GetCompletedLaps()
            => _completedLaps;

        public int GetCompletedLapsCount()
            => _completedLaps.Count;

        public void Clear()
        {
            _completedLaps.Clear();
            ResetCurrentLap();
        }
    }
}
