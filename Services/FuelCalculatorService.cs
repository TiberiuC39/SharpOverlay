using iRacingSdkWrapper;
using SharpOverlay.Models;
using System;
using System.Linq;

namespace SharpOverlay.Services
{
    public class FuelCalculatorService
    {
        private readonly iRacingTelemetryService _iRacingService;
        private float _fuelConsumed;
        private float _maxFuelCapacity;
        private float _averageConsumption;
        private int _lapsCompleted;
        private float _fuelLevel;
        private int _refuelRequired;
        private double _lapsOfFuelRemaining;

        public FuelCalculatorService()
        { 
            _iRacingService = new iRacingTelemetryService();
            _iRacingService.HookUpToTelemetryEvent(ExecuteOnTelemetryEvent);
        }

        public void HookUpToSdkEvent(EventHandler<SdkWrapper.TelemetryUpdatedEventArgs> handler)
        {
            _iRacingService.HookUpToTelemetryEvent(handler);
        }

        public FuelViewModel GetViewModel()
        {
            return new FuelViewModel()
            {
                ConsumedFuel = _fuelConsumed,
                LapsCompleted = _lapsCompleted,
                CurrentFuelLevel = _fuelLevel,
                AverageFuelConsumption = _averageConsumption
            };
        }

        private void ExecuteOnTelemetryEvent(object? sender, SdkWrapper.TelemetryUpdatedEventArgs eventArgs)
        {
            CalculateFuelDataToDisplay();
        }

        private void CalculateFuelDataToDisplay()
        {
            var telemetry = _iRacingService.GetTelemetry();
            var raceData = _iRacingService.GetRaceData();

            if (raceData != null && telemetry != null)
            {
                if (_maxFuelCapacity == default)
                {
                    _maxFuelCapacity = GetMaxFuelCapacityAfterHandicap(raceData);
                }

                UpdateFuelConsumed(telemetry);
                UpdateLapsCompleted(telemetry);
                UpdateAverageConsumption();
            }
        }

        private DriverPosition GetCurrentPosition(RaceDataOutput session)
        {
            var currentSession = session.Sessions.Last();
            var playerCarIdx = session.Driver.CarIdx;

            if (currentSession.ResultsPositions.Any())
            {
                return currentSession.ResultsPositions.Last(p => p.CarIdx == playerCarIdx);
            }

            return new DriverPosition { CarIdx = playerCarIdx };
        }

        private void UpdateLapsCompleted(TelemetryOutput telemetry)
        {
            _lapsCompleted = telemetry.LapCompleted.Value;
        }

        private void UpdateRefuelingRequired(RaceDataOutput session)
        {
            throw new NotImplementedException();
        }

        private void UpdateAverageConsumption()
        {
            if (_lapsCompleted <= 0)
            {
                _averageConsumption = _fuelConsumed;

            }
            else
            {
                _averageConsumption = _fuelConsumed / _lapsCompleted;
            }
        }

        private void UpdateFuelConsumed(TelemetryOutput telemetry)
        {
            float newFuelLevel = telemetry.FuelLevel.Value;
            float difference = _fuelLevel - newFuelLevel;

            _fuelLevel = newFuelLevel;

            if (difference > 0)
            {
                _fuelConsumed += difference;
            }
        }

        private float GetMaxFuelCapacityAfterHandicap(RaceDataOutput session)
        {
            var fuelHandicap = session.Driver.DriverCarMaxFuelPct;
            var maxFuelCapacity = session.Driver.DriverCarFuelMaxLtr;

            return fuelHandicap * maxFuelCapacity;
        }
    }
}
