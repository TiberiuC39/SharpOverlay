using iRacingSdkWrapper;
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
        private Lap? _currentLap;
        private float _currentFuelLevel;
        private double _lapsRemainingInRace;
        private int _raceTotalLaps;
        private bool _onPitRoad;
        private bool _isInService;
        private bool _hasBegunService;
        private bool _isRaceStart;
        private TimeSpan _averageLapTime;
        private bool _isComingOutOfPits;
        private bool _hasCompletedService;
        private bool _isEnteringPits;
        private bool _movedLapOnce;
        private float _avgFuelPerLap;
        private double _refuelRequired;

        public FuelCalculatorService()
        {
            _iRacingService = new iRacingTelemetryService();
            _iRacingService.HookUpToTelemetryEvent(ExecuteOnTelemetryEvent);

            _completedLaps = new List<Lap>();
        }

        private void Clear()
        {
            _completedLaps.Clear();
            _currentLap = null;
            _currentFuelLevel = 0;
            _lapsRemainingInRace = 0;
            _onPitRoad = false;
            _hasBegunService = false;
            _isInService = false;
            _isRaceStart = false;
            _isEnteringPits = false;
            _isComingOutOfPits = false;
            _movedLapOnce = false;
            _refuelRequired = 0;
        }

        public void HookUpToSdkEvent(EventHandler<SdkWrapper.TelemetryUpdatedEventArgs> handler)
        {
            _iRacingService.HookUpToTelemetryEvent(handler);
        }

        private void ExecuteOnTelemetryEvent(object? sender, SdkWrapper.TelemetryUpdatedEventArgs eventArgs)
        {
            var telemetry = _iRacingService.GetTelemetry();

            if ((telemetry.SessionState.Value != SessionStates.Invalid || telemetry.SessionState.Value != SessionStates.CoolDown)
                && telemetry.IsOnTrack.Value)
            {
                bool isOnPitRoad = telemetry.IsOnPitRoad.Value;
                int currentLapNumber = telemetry.Lap.Value;
                bool isReceivingPitService = telemetry.IsPitstopActive.Value;

                UpdatePitEntryStatus(isOnPitRoad);
                UpdatePitServiceStatus(isReceivingPitService);

                if (_currentLap is null)
                {
                    StartNewLap(telemetry);

                    if (IsRaceStart(currentLapNumber))
                    {
                        _isRaceStart = true;
                    }
                }                
                else if (_hasBegunService && !_hasCompletedService && !_movedLapOnce)
                {
                    CompleteCurrentLap(telemetry);

                    _movedLapOnce = true;
                }
                else if (_hasBegunService && _hasCompletedService && _movedLapOnce)
                {
                    StartNewLap(telemetry);
                    _currentLap.Number++;

                    _hasBegunService = false;
                    _hasCompletedService = false;

                    _isComingOutOfPits = true;

                    if (_avgFuelPerLap > 0)
                    {
                        _refuelRequired = CalculateRefuel(_currentLap.StartingFuel);
                    }
                }
                else if (IsCrossingFinishLine(currentLapNumber))
                {
                    if (_isRaceStart)
                    {
                        _isRaceStart = false;
                    }
                    else if (!_isComingOutOfPits && !_onPitRoad)
                    {
                        CompleteCurrentLap(telemetry);
                        StartNewLap(telemetry);
                    }
                    else if (_isComingOutOfPits && _movedLapOnce)
                    {
                        _isComingOutOfPits = false;
                        _movedLapOnce = false;
                    }                    

                    _averageLapTime = CalculateAverageTime();

                    if (_averageLapTime.TotalSeconds > 0)
                    {
                        var timeRemainingInSession = TimeSpan.FromSeconds(telemetry.SessionTimeRemain.Value);

                        CalculateLapsRemaining(timeRemainingInSession);
                    }

                    _avgFuelPerLap = CalculateAverageFuelConsumption();

                    if (_avgFuelPerLap > 0)
                    {
                        _refuelRequired = CalculateRefuel(_currentLap.StartingFuel);
                    }

                    _raceTotalLaps = telemetry.RaceLaps.Value;
                }

                _currentFuelLevel = telemetry.FuelLevel.Value;
            }
            else if (telemetry.SessionState.Value == SessionStates.Invalid)
            {
                Clear();
            }
            else if (!telemetry.IsOnTrack.Value && telemetry.IsReplayPlaying.Value && _currentLap is not null)
            {
                _currentLap = null;
            }
        }

        private float CalculateAverageFuelConsumption()
        {
            if (_completedLaps.Count > 1)
            {
                return _completedLaps.Skip(1).Average(l => l.FuelUsed);
            }

            return default;
        }

        private void UpdatePitServiceStatus(bool isReceivingPitService)
        {
            if (isReceivingPitService == true && _isInService == false)
            {
                _hasBegunService = true;
                _isInService = true;
                _hasCompletedService = false;
            }
            else if (isReceivingPitService == false && _isInService == true)
            {
                _isInService = false;
                _hasCompletedService = true;
            }            
        }

        private void UpdatePitEntryStatus(bool isOnPitRoad)
        {
            if (isOnPitRoad == true && _isEnteringPits == false)
            {
                _isEnteringPits = true;
            }
            else if (isOnPitRoad == false && _isEnteringPits == true)
            {
                _isEnteringPits = false;
            }
        }

        private bool IsCrossingFinishLine(int currentLapNumber)
            => currentLapNumber > _currentLap!.Number;

        private bool IsRaceStart(int currentLapNumber)
            => currentLapNumber < 0;        

        private void CalculateLapsRemaining(TimeSpan timeRemainingInSession)
        {
            _lapsRemainingInRace = Math.Ceiling(timeRemainingInSession / _averageLapTime);

            if (_lapsRemainingInRace > 1000)
            {
                _lapsRemainingInRace = 1000;
            }
        }

        private TimeSpan CalculateAverageTime()
        {
            if (_completedLaps.Count > 1 && _completedLaps.Any(l => l.Time >= TimeSpan.Zero))
            {
                var averageLapTime = TimeSpan.FromSeconds(_completedLaps
                    .Skip(1)
                .Where(l => l.Time >= TimeSpan.Zero)
                .Average(l => l.Time.TotalSeconds));
                
                return averageLapTime;
            }

            return TimeSpan.Zero;
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
            _currentLap!.EndingFuel = telemetry.FuelLevel.Value;
            _currentLap.Time = TimeSpan.FromSeconds(telemetry.LapLastLapTime.Value);
            _currentLap.FuelUsed = _currentLap.StartingFuel - _currentLap.EndingFuel;

            _completedLaps.Add(_currentLap);
        }

        public FuelViewModel GetViewModel()
        {
            float fiveLapAverage = 0;

            if (_completedLaps.Count > 5)
            {
                fiveLapAverage = _completedLaps.TakeLast(5).Average(l => l.FuelUsed);
            }

            if (_completedLaps.Count > 1)
            {
                return new FuelViewModel()
                {
                    ConsumedFuel = _completedLaps.Sum(l => l.FuelUsed),
                    AverageFuelConsumption = _avgFuelPerLap,
                    LapsCompleted = _completedLaps.Count,
                    CurrentFuelLevel = _currentFuelLevel,
                    LapsOfFuelRemaining = _currentFuelLevel / _avgFuelPerLap,
                    TotalRaceLaps = _raceTotalLaps,
                    RefuelRequired = _refuelRequired,
                    PitRoad = _onPitRoad,
                    PitActive = _isInService,
                    Serviced = _hasBegunService,
                    FiveLapAverage = fiveLapAverage
                };
            }

            return new FuelViewModel()
            {
                CurrentFuelLevel = _currentFuelLevel
            };
        }

        private double CalculateRefuel(float fuelLeftInCar)
        {
            var refuelRequired = _lapsRemainingInRace * _avgFuelPerLap - fuelLeftInCar;

            //TODO Calculate for fuel cutoff point            

            return refuelRequired;
        }
    }
}
