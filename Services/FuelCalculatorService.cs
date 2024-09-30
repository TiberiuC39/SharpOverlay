using iRacingSdkWrapper;
using SharpOverlay.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpOverlay.Services
{
    public class FuelCalculatorService : IFuelCalculator
    {
        private const int DefaultTickRate = 2;

        private readonly iRacingDataService _iRacingService;

        private Dictionary<int, Lap> _completedLaps;
        private Dictionary<int, List<Lap>> _driverLaps;
        private Lap? _currentLap;
        private float _currentFuelLevel;
        private double _lapsRemainingInRace;
        private bool _isInService;
        private bool _hasBegunService;
        private TimeSpan _averageLapTime;
        private bool _hasCompletedService;
        private float _avgFuelPerLap;
        private double _refuelRequired;
        private float _fiveLapAverage;
        private float _fiveLapRefuelRequired;
        private bool _isRollingStart;
        private bool _hasResetToPits;
        private bool _isRaceStart;
        private TimeSpan _leaderAvgLapTime;
        private TimeSpan _leaderTimeToCompleteLap;
        private TimeSpan _estLapTime;
        private int _currentSessionNumber;
        private int _leaderIdx;
        private int _sessionLaps;

        public FuelCalculatorService(int tickRate = DefaultTickRate)
        {
            _iRacingService = new iRacingDataService();

            _iRacingService.HookUpToTelemetryEvent(ExecuteOnTelemetryEvent);
            _iRacingService.HookUpToConnectedEvent(ExecuteOnConnected);
            _iRacingService.HookUpToSessionEvent(ExecuteOnSessionEvent);

            _completedLaps = new Dictionary<int, Lap>();
            _driverLaps = new Dictionary<int, List<Lap>>();
        }

        private void ExecuteOnConnected(object? sender, EventArgs args)
        {
            var sessionInfo = _iRacingService.GetSessionInfo();

            _isRollingStart = sessionInfo.WeekendInfo.WeekendOptions.StandingStart == 0;

            _driverLaps = sessionInfo.Drivers.ToDictionary(d => d.CarIdx, d => new List<Lap>());
        }

        public event EventHandler<FuelEventArgs> FuelUpdated = null!;

        private void Clear()
        {
            _completedLaps.Clear();
            _currentLap = null;
            _currentFuelLevel = 0;
            _lapsRemainingInRace = 0;
            _hasBegunService = false;
            _isInService = false;
            _refuelRequired = 0;
            _isRollingStart = false;
        }

        private void ExecuteOnTelemetryEvent(object? sender, SdkWrapper.TelemetryUpdatedEventArgs eventArgs)
        {
            _currentSessionNumber = eventArgs.TelemetryInfo.SessionNum.Value;
            ProcessTelemetryUpdate(eventArgs.TelemetryInfo);
            CalculateOtherDriversLaps(eventArgs.TelemetryInfo);

            FuelUpdated(this, new FuelEventArgs(GetViewModel()));
        }

        private void ExecuteOnSessionEvent(object? sender, SdkWrapper.SessionUpdatedEventArgs eventArgs)
        {
            _sessionLaps = eventArgs.SessionInfo.Sessions.Last().SessionNumLapsToAvg;
        }

        private void CalculateOtherDriversLaps(TelemetryInfo telemetry)
        {
            var driversLastLapTimes = telemetry.CarIdxLastLapTime.Value;
            var driversPositions = telemetry.CarIdxPosition.Value;
            _leaderIdx = telemetry.CarIdxClassPosition.Value.FirstOrDefault(i => i == 1);

            foreach ((int idx, var laps) in _driverLaps)
            {
                if (telemetry.CarIdxLapCompleted.Value[idx] > laps.Count)
                {
                    laps.Add(new Lap()
                    {
                        Time = TimeSpan.FromSeconds(driversLastLapTimes[idx])
                    });
                }                
            }

            var validLeaderLaps = _driverLaps[_leaderIdx].Where(l => l.Time > TimeSpan.Zero);

            if (validLeaderLaps.Any())
            {
                _leaderAvgLapTime = TimeSpan.FromSeconds(validLeaderLaps.Average(l => l.Time.TotalSeconds));
                var leaderLapDistance = telemetry.CarIdxLapDistPct.Value[_leaderIdx];

                if (leaderLapDistance >= 0)
                {
                    _leaderTimeToCompleteLap = (1 - leaderLapDistance) * _leaderAvgLapTime;
                }                
            }

            _estLapTime = TimeSpan.FromSeconds(telemetry.CarIdxEstTime.Value[telemetry.PlayerCarIdx.Value]);
        }

        private void ProcessTelemetryUpdate(TelemetryInfo telemetry)
        {
            if (IsSessionStateValid(telemetry))
            {
                bool isOnPitRoad = telemetry.IsOnPitRoad.Value;
                int currentLapNumber = telemetry.Lap.Value;
                bool isReceivingPitService = telemetry.IsPitstopActive.Value;

                _currentFuelLevel = telemetry.FuelLevel.Value;

                UpdatePitServiceStatus(isReceivingPitService);

                _hasResetToPits = HasResetToPits(telemetry);

                if (_currentLap is null)
                {
                    StartNewLap(currentLapNumber, _currentFuelLevel);

                    if (telemetry.SessionState.Value == SessionStates.Racing && currentLapNumber == 0)
                    {
                        _isRaceStart = true;
                    }
                }
                else if (_isRaceStart && currentLapNumber == 2)
                {
                    _isRaceStart = false;
                }
                else if (_hasResetToPits)
                {
                    _currentLap.StartingFuel = _currentFuelLevel;

                    CalculateRefuel(_currentFuelLevel);
                }
                else if (_hasBegunService)
                {
                    double lastLapTime = telemetry.LapLastLapTime.Value;
                    double timeRemaining = telemetry.SessionTimeRemain.Value;

                    CompleteCurrentLap(lastLapTime, timeRemaining);

                    _hasBegunService = false;
                }
                else if (_hasCompletedService)
                {
                    StartNewLap(currentLapNumber, _currentFuelLevel);

                    if (currentLapNumber == _completedLaps.Last().Value.Number)
                    {
                        _currentLap.Number++;
                    }

                    _hasCompletedService = false;

                    CalculateRefuel(_currentLap.StartingFuel);
                }
                else if (IsCrossingFinishLine(currentLapNumber) && !isOnPitRoad)
                {
                    double lastLapTime = telemetry.LapLastLapTime.Value;
                    double timeRemaining = telemetry.SessionTimeRemain.Value;

                    CompleteCurrentLap(lastLapTime, timeRemaining);

                    StartNewLap(currentLapNumber, _currentFuelLevel);

                    CalculateFuelAndLapData(timeRemaining);                    
                }
            }
            else if (telemetry.SessionState.Value == SessionStates.Invalid
                || telemetry.SessionState.Value == SessionStates.CoolDown
                || telemetry.SessionNum.Value != _currentSessionNumber
                || telemetry.SessionState.Value == SessionStates.Warmup
                || telemetry.SessionState.Value == SessionStates.GetInCar)
            {
                Clear();
            }
            else if (!telemetry.IsOnTrack.Value
                && telemetry.IsReplayPlaying.Value
                && _currentLap is not null)
            {
                _currentLap = null;
            }
        }        

        private bool HasResetToPits(TelemetryInfo telemetry)
        {
            return telemetry.EnterExitReset.Value == 0;
        }

        private static bool IsSessionStateValid(TelemetryInfo telemetry)
        {
            return (telemetry.SessionState.Value != SessionStates.Invalid
                            || telemetry.SessionState.Value != SessionStates.Checkered
                            || telemetry.SessionState.Value != SessionStates.CoolDown)
                            && telemetry.IsOnTrack.Value;
        }

        private void CalculateFuelAndLapData(double timeRemaining)
        {
            CalculateAverageTime();

            var timeRemainingInSession = TimeSpan.FromSeconds(timeRemaining);

            CalculateLapsRemaining(timeRemainingInSession);

            CalculateAverageFuelConsumption();

            CalculateRefuel(_currentLap!.StartingFuel);
        }

        private void CalculateAverageFuelConsumption()
        {
            IEnumerable<Lap> validLaps;

            if (_isRollingStart && _completedLaps.Count > 1)
            {
                validLaps = _completedLaps.Values.Skip(1);
            }
            else
            {
                validLaps = _completedLaps.Values;
            }
            
            validLaps = validLaps.Where(l => l.Time >= TimeSpan.Zero);

            if (validLaps.Any())
            {
                _avgFuelPerLap = validLaps.Average(l => l.FuelUsed);

                if (validLaps.Count() > 4)
                {
                    _fiveLapAverage = validLaps.TakeLast(5).Average(l => l.FuelUsed);
                }
            }
            else if (_completedLaps.Any())
            {
                _avgFuelPerLap = _completedLaps.Values.Average(l => l.FuelUsed);
                _fiveLapAverage = _avgFuelPerLap;
            }
        }

        private void UpdatePitServiceStatus(bool isReceivingPitService)
        {
            if (isReceivingPitService && !_isInService)
            {
                _hasBegunService = true;
                _isInService = true;
            }
            else if (!isReceivingPitService && _isInService)
            {
                _isInService = false;
                _hasCompletedService = true;
            }            
        }

        private bool IsCrossingFinishLine(int currentLapNumber)
            => currentLapNumber > _currentLap!.Number;

        private void CalculateLapsRemaining(TimeSpan timeRemainingInSession)
        {
            if (_leaderAvgLapTime > TimeSpan.Zero)
            {
                _lapsRemainingInRace = Math.Ceiling((timeRemainingInSession - _leaderTimeToCompleteLap) / _leaderAvgLapTime);
            }
            else if (_averageLapTime > TimeSpan.Zero)
            {
                _lapsRemainingInRace = Math.Ceiling(timeRemainingInSession / _averageLapTime);
            }            
            else
            {
                _lapsRemainingInRace = Math.Ceiling(timeRemainingInSession / _estLapTime);
            }
        }

        private void CalculateAverageTime()
        {
            var validLaps = _completedLaps.Where(l => l.Value.Time > TimeSpan.Zero).ToList();

            if (validLaps.Any())
            {
                _averageLapTime = TimeSpan.FromSeconds(validLaps
                    .Average(l => l.Value.Time.TotalSeconds));
            }
        }

        private void StartNewLap(int currentLap, float startingFuel)
        {
            var newLap = new Lap()
            {
                Number = currentLap,
                StartingFuel = startingFuel
            };

            _currentLap = newLap;
        }

        private void CompleteCurrentLap(double lastLapTime, double sessionTimeRemain)
        {
            _currentLap!.EndingFuel = _currentFuelLevel;
            _currentLap.Time = TimeSpan.FromSeconds(lastLapTime);
            _currentLap.FuelUsed = _currentLap.StartingFuel - _currentLap.EndingFuel;

            _completedLaps.Add(_currentLap.Number, _currentLap);

            CalculateFuelAndLapData(sessionTimeRemain);
        }

        private FuelViewModel GetViewModel()
        {
            if (_completedLaps.Count > 1)
            {
                return new FuelViewModel()
                {
                    ConsumedFuel = _completedLaps.Sum(l => l.Value.FuelUsed),
                    AverageFuelConsumption = _avgFuelPerLap,
                    LapsCompleted = _completedLaps.Count,
                    CurrentFuelLevel = _currentFuelLevel,
                    LapsOfFuelRemaining = _currentFuelLevel / _avgFuelPerLap,
                    RaceLapsRemaining = _lapsRemainingInRace,
                    RefuelRequired = _refuelRequired,
                    FiveLapAverage = _fiveLapAverage,
                    FiveLapRefuelRequired = _fiveLapRefuelRequired,
                    FiveLapLapsOfFuelRemaining = _currentFuelLevel / _fiveLapAverage
                };
            }

            return new FuelViewModel()
            {
                CurrentFuelLevel = _currentFuelLevel,
                LapsCompleted = _completedLaps.Count,
                RaceLapsRemaining = _lapsRemainingInRace
            };
        }

        private void CalculateRefuel(float fuelLeftInCar)
        {
            float fuelCutoff = 0.3f;

            if (_avgFuelPerLap > 0)
            {
                _refuelRequired = _lapsRemainingInRace * _avgFuelPerLap - fuelLeftInCar + fuelCutoff;
            }

            if (_completedLaps.Count > 5)
            {
                _fiveLapRefuelRequired = (float)(_lapsRemainingInRace * _fiveLapAverage - fuelLeftInCar) + fuelCutoff;
            }
            else
            {
                _fiveLapRefuelRequired = (float) _refuelRequired;
            }
        }
    }
}
