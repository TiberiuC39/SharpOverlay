using iRacingSdkWrapper;
using iRacingSdkWrapper.Bitfields;
using SharpOverlay.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpOverlay.Services
{
    public class FuelCalculatorService
    {
        private readonly iRacingTelemetryService _iRacingService;
        private List<Lap> _completedLaps;
        private Lap _currentLap;
        private float _currentFuelLevel;
        private double _lapsRemainingInRace;
        private int _raceTotalLaps;
        private bool _hasEnteredPitRoad;
        private bool _isPitActive;
        private bool _hasBeenServiced;
        private bool _isRaceStart;

        public FuelCalculatorService()
        {
            _iRacingService = new iRacingTelemetryService();
            _iRacingService.HookUpToTelemetryEvent(ExecuteOnTelemetryEvent);

            _completedLaps = new List<Lap>();
        }

        public void HookUpToSdkEvent(EventHandler<SdkWrapper.TelemetryUpdatedEventArgs> handler)
        {
            _iRacingService.HookUpToTelemetryEvent(handler);
        }

        private void ExecuteOnTelemetryEvent(object? sender, SdkWrapper.TelemetryUpdatedEventArgs eventArgs)
        {
            var telemetry = _iRacingService.GetTelemetry();

            if (telemetry.SessionState.Value == SessionStates.Racing && telemetry.IsOnTrack.Value)
            {
                bool isOnPitRoad = telemetry.IsOnPitRoad.Value;

                UpdateSelfTrackedPitEntryStatus(isOnPitRoad);

                bool isReceivingPitService = telemetry.IsPitstopActive.Value;

                UpdatePitServiceStatus(isReceivingPitService);

                int currentLapNumber = telemetry.Lap.Value;

                if (_hasEnteredPitRoad && _isPitActive && !_hasBeenServiced)
                {
                    CompleteCurrentLap(telemetry);

                    _hasBeenServiced = true;
                }
                else if (_hasEnteredPitRoad && _isPitActive && !isReceivingPitService && _hasBeenServiced)
                {
                    StartNewLap(telemetry);
                    _isPitActive = false;
                }
                else if (_currentLap is null)
                {
                    StartNewLap(telemetry);
                }
                else if (currentLapNumber < 0)
                {
                    _isRaceStart = true;
                }
                else if (_isRaceStart && currentLapNumber > _currentLap.Number)
                {
                    _isRaceStart = false;
                }
                else if (currentLapNumber > _currentLap.Number)
                {
                    CompleteCurrentLap(telemetry);
                    StartNewLap(telemetry);
                }

                _currentFuelLevel = telemetry.FuelLevel.Value;
                _lapsRemainingInRace = telemetry.SessionLapsRemaining.Value;
                _raceTotalLaps = telemetry.RaceLaps.Value;
            }
        }

        private void UpdatePitServiceStatus(bool isReceivingPitService)
        {
            if (isReceivingPitService && !_isPitActive)
            {
                _isPitActive = true;
            }
            else if (_hasEnteredPitRoad && _isPitActive && !isReceivingPitService)
            {
                _isPitActive = false;
            }
        }

        private void UpdateSelfTrackedPitEntryStatus(bool isOnPitRoad)
        {
            if (isOnPitRoad && !_hasEnteredPitRoad)
            {
                _hasEnteredPitRoad = true;
            }
            else if (!isOnPitRoad && _hasEnteredPitRoad)
            {
                _hasEnteredPitRoad = false;
            }
        }

        private void StartNewLap(TelemetryOutput telemetry)
        {
            int currentLap = telemetry.Lap.Value;
            float startingFuel = telemetry.FuelLevel.Value;

            if (_currentLap is null)
            {
                currentLap++;
            }

            var newLap = new Lap()
            {
                Number = currentLap,
                StartingFuel = startingFuel
            };

            _currentLap = newLap;
        }

        private void CompleteCurrentLap(TelemetryOutput telemetry)
        {
            _currentLap.EndingFuel = telemetry.FuelLevel.Value;
            _currentLap.Time = TimeSpan.FromSeconds(telemetry.LapLastLapTime.Value);
            _currentLap.FuelUsed = _currentLap.StartingFuel - _currentLap.EndingFuel;

            _completedLaps.Add(_currentLap);
        }

        public FuelViewModel GetViewModel()
        {
            if (_completedLaps.Any())
            {
                var averageFuelConsumption = _completedLaps.Average(l => l.FuelUsed);
                double refuelRequired = _lapsRemainingInRace * averageFuelConsumption;

                return new FuelViewModel()
                {
                    ConsumedFuel = _completedLaps.Sum(l => l.FuelUsed),
                    AverageFuelConsumption = averageFuelConsumption,
                    LapsCompleted = _completedLaps.Count,
                    CurrentFuelLevel = _currentFuelLevel,
                    LapsOfFuelRemaining = _currentFuelLevel / averageFuelConsumption,
                    TotalRaceLaps = _raceTotalLaps,
                    RefuelRequired = refuelRequired
                };
            }

            return new FuelViewModel();
        }
    }
}
