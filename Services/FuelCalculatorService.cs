using iRacingSdkWrapper;
using SharpOverlay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SharpOverlay.Services
{
    public class FuelCalculatorService : IFuelCalculator
    {
        private const int DefaultTickRate = 2;

        private readonly iRacingDataService _iRacingService;
        private readonly SessionParser _sessionParser;
        private readonly TelemetryParser _telemetryParser;

        private Dictionary<int, Lap> _completedLaps = [];
        private Dictionary<int, List<Lap>> _driverLaps = [];
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
        private bool _hasResetToPits;
        private bool _isRaceStart;
        private TimeSpan _leaderAvgLapTime;
        private TimeSpan _leaderTimeToCompleteLap;
        private int _currentSessionNumber;
        private TrackSurfaces _trackSurface;
        private int _leaderIdx;
        private int _playerIdx;
        private bool _isOnPitRoad;

        public FuelCalculatorService(int tickRate = DefaultTickRate)
        {
            _iRacingService = new iRacingDataService();
            _sessionParser = new SessionParser();
            _telemetryParser = new TelemetryParser();

            _iRacingService.HookUpToTelemetryEvent(ExecuteOnTelemetryEvent);
            _iRacingService.HookUpToConnectedEvent(ExecuteOnConnected);
            _iRacingService.HookUpToSessionEvent(ExecuteOnSessionEvent);            
            _iRacingService.HookUpToDisconnectedEvent(ExecuteOnDisconnected);
        }

        private void ExecuteOnDisconnected(object? sender, EventArgs args)
        {
            Clear();

            FuelUpdated(this, new FuelEventArgs(GetViewModel()));
        }

        private void ExecuteOnConnected(object? sender, EventArgs args)
        {
            Initialize();
        }

        private void Initialize()
        {
            var sessionInfo = _iRacingService.GetSessionInfo();
            var telemetry = _iRacingService.GetTelemetryInfo();

            _telemetryParser.ParseCurrentSessionNumber(telemetry);
            _sessionParser.ParseSessions(sessionInfo);

            if (_sessionParser.Sessions.Count == 0)
            {
                Thread.Sleep(5000);
                sessionInfo = _iRacingService.GetSessionInfo();
                _sessionParser.ParseSessions(sessionInfo);
            }

            _sessionParser.ParseStartType(sessionInfo);
            _sessionParser.ParseDrivers(sessionInfo);
            _sessionParser.ParseLapsInSession(sessionInfo, _currentSessionNumber);

            _telemetryParser.ParsePlayerCarIdx(telemetry);
            _telemetryParser.ParsePlayerCarClassId(telemetry);

            CalculateOtherDriversLaps(telemetry);

            FindLeaderIdxInClass();
            CalculateLeaderLapTime(telemetry);

            var timeRemaining = _telemetryParser.GetTimeRemaining(telemetry);

            if (timeRemaining.TotalSeconds > 0)
            {
                CalculateLapsRemaining(timeRemaining);
            }
        }

        public event EventHandler<FuelEventArgs> FuelUpdated = null!;

        private void Reset()
        {
            _completedLaps.Clear();
            _driverLaps.Clear();
            _currentLap = null;
            _currentFuelLevel = 0;
            _lapsRemainingInRace = 0;
            _hasBegunService = false;
            _isInService = false;
            _refuelRequired = 0;
            _fiveLapAverage = 0;
            _fiveLapRefuelRequired = 0;

            Initialize();
        }

        private void Clear()
        {
            Reset();

            _sessionParser.Clear();
            _telemetryParser.Clear();
        }

        private void ExecuteOnTelemetryEvent(object? sender, SdkWrapper.TelemetryUpdatedEventArgs eventArgs)
        {
            var telemetry = eventArgs.TelemetryInfo;
            _trackSurface = telemetry.PlayerTrackSurface.Value;

            _telemetryParser.ParseCurrentSessionNumber(telemetry);
            _telemetryParser.ParsePositionCarIdxInPlayerClass(telemetry, _sessionParser.SessionType);

            CalculateOtherDriversLaps(telemetry);

            FindLeaderIdxInClass();
            CalculateLeaderLapTime(telemetry);

            if (IsSessionStateValid(telemetry))
            {
                ProcessTelemetryUpdate(telemetry);
            }
            else if (IsSessionStateInvalid(telemetry))
            {
                Reset();
            }
            else if (!telemetry.IsOnTrack.Value
                && telemetry.IsReplayPlaying.Value
                && _currentLap is not null)
            {
                _currentLap = null;
            }

            FuelUpdated(this, new FuelEventArgs(GetViewModel()));
        }

        private bool IsSessionStateInvalid(TelemetryInfo telemetry)
        {
            return telemetry.SessionState.Value == SessionStates.Invalid
                            || telemetry.SessionState.Value == SessionStates.CoolDown
                            || telemetry.SessionNum.Value != _currentSessionNumber
                            || telemetry.SessionState.Value == SessionStates.Warmup;
                            //|| telemetry.SessionState.Value == SessionStates.GetInCar;
        }

        private void ExecuteOnSessionEvent(object? sender, SdkWrapper.SessionUpdatedEventArgs eventArgs)
        {
            var sessionInfo = eventArgs.SessionInfo;
            
            if (_sessionParser.SessionLaps == 0)
            {
                _sessionParser.ParseLapsInSession(sessionInfo, _telemetryParser.CurrentSessionNumber);
            }

            _sessionParser.ParseDrivers(sessionInfo);
            _sessionParser.ParseSessions(sessionInfo);
        }

        private void CalculateOtherDriversLaps(TelemetryInfo telemetry)
        {
            var driversLastLapTimes = TelemetryParser.GetDriversLastLapTime(telemetry);

            foreach ((int idx, var driver) in _sessionParser.Drivers)
            {
                if (!_driverLaps.ContainsKey(idx))
                {
                    _driverLaps.Add(idx, new List<Lap>());
                }

                var laps = _driverLaps[idx];
                int? lastLapNumber = laps.LastOrDefault()?.Number;

                if (telemetry.CarIdxLapCompleted.Value[idx] > (lastLapNumber ?? 0))
                {
                    laps.Add(new Lap()
                    {
                        Number = telemetry.CarIdxLapCompleted.Value[idx],
                        Time = driversLastLapTimes[idx]
                    });
                }
            }            
        }

        private void CalculateLeaderLapTime(TelemetryInfo telemetry)
        {
            IEnumerable<Lap> validLeaderLaps = new List<Lap>();

            if (_driverLaps.Any(d => d.Key == _leaderIdx))
            {
                validLeaderLaps = _driverLaps[_leaderIdx].Where(l => l.Time > TimeSpan.Zero);
            }

            var leaderLapDistance = telemetry.CarIdxLapDistPct.Value[_leaderIdx];

            if (validLeaderLaps.Any())
            {
                _leaderAvgLapTime = TimeSpan.FromSeconds(validLeaderLaps.Average(l => l.Time.TotalSeconds));

                if (leaderLapDistance >= 0)
                {
                    _leaderTimeToCompleteLap = (1 - leaderLapDistance) * _leaderAvgLapTime;
                }
            }
            else if (_leaderAvgLapTime <= TimeSpan.Zero)
            {
                _leaderAvgLapTime = _sessionParser.GetBestLapTime(_leaderIdx);
                _leaderTimeToCompleteLap = (1 - leaderLapDistance) * _leaderAvgLapTime;
            }
            else
            {
                _leaderAvgLapTime = TimeSpan.Zero;
                _leaderTimeToCompleteLap = TimeSpan.Zero;
            }
        }

        private void FindLeaderIdxInClass()
        {
            var positionsIdxInClass = _telemetryParser.PositionCarIdxInClass;

            const int invalidLeaderPosition = -1;

            int leaderInClassPosition = positionsIdxInClass.Keys.Count > 0 ? positionsIdxInClass.Keys.Min() : invalidLeaderPosition;

            if (leaderInClassPosition > invalidLeaderPosition)
            {
                _leaderIdx = positionsIdxInClass[leaderInClassPosition];
            }
        }

        private void ProcessTelemetryUpdate(TelemetryInfo telemetry)
        {
            _isOnPitRoad = telemetry.IsOnPitRoad.Value;
            int currentLapNumber = telemetry.Lap.Value;
            bool isReceivingPitService = telemetry.IsPitstopActive.Value;

            _currentFuelLevel = telemetry.FuelLevel.Value;

            UpdatePitServiceStatus(isReceivingPitService);

            _hasResetToPits = HasResetToPits(telemetry);

            var currentSession = _sessionParser.Sessions[_telemetryParser.CurrentSessionNumber];

            if (_currentLap is null)
            {
                StartNewLap(currentLapNumber, _currentFuelLevel);

                if (telemetry.SessionState.Value == SessionStates.Racing
                    && currentLapNumber == 0 && currentSession.SessionType == "Race")
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
            else if (IsCrossingFinishLine(currentLapNumber) && !_isOnPitRoad && telemetry.SessionState.Value != SessionStates.ParadeLaps)
            {
                double lastLapTime = telemetry.LapLastLapTime.Value;
                double timeRemaining = telemetry.SessionTimeRemain.Value;

                CompleteCurrentLap(lastLapTime, timeRemaining);

                StartNewLap(currentLapNumber, _currentFuelLevel);

                CalculateFuelAndLapData(timeRemaining);
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
            var currentSession = _sessionParser.Sessions[_telemetryParser.CurrentSessionNumber];

            string sessionType = currentSession.SessionType;

            IEnumerable<Lap> validLaps;
            var startType = _sessionParser.StartType;

            if (_completedLaps.Count > 1)
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
                else
                {
                    _fiveLapAverage = _avgFuelPerLap;
                }
            }
            else if (startType == StartType.Rolling && _completedLaps.Count > 1)
            {
                _avgFuelPerLap = _completedLaps.Values.Skip(1).Average(l => l.FuelUsed);
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
            if (_sessionParser.SessionLaps > 0)
            {
                int completedLaps = _completedLaps.Count;

                var startType = _sessionParser.StartType;

                if (startType == StartType.Rolling)
                {
                    completedLaps--;
                }

                _lapsRemainingInRace = _sessionParser.SessionLaps - completedLaps;
            }
            else if (_leaderAvgLapTime > TimeSpan.Zero)
            {
                _lapsRemainingInRace = Math.Ceiling((timeRemainingInSession - _leaderTimeToCompleteLap) / _leaderAvgLapTime) + 1;
            }
            else if (_averageLapTime > TimeSpan.Zero)
            {
                _lapsRemainingInRace = Math.Ceiling(timeRemainingInSession / _averageLapTime) + 1;
            }            
        }

        private void CalculateAverageTime()
        {
            var validLaps = _completedLaps.Where(l => l.Value.Time > TimeSpan.Zero);

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

            if (_completedLaps.ContainsKey(_currentLap.Number))
            {
                int nextLapNumber = _completedLaps.Last().Value.Number + 1;

                _currentLap.Number = nextLapNumber;
            }

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
                    FiveLapLapsOfFuelRemaining = _currentFuelLevel / _fiveLapAverage,

                    IsInService = _isInService,
                    HasBegunService = _hasBegunService,
                    HasCompletedService = _hasCompletedService,
                    HasResetToPits = _hasResetToPits,
                    IsRollingStart = _sessionParser.StartType == StartType.Rolling,
                    IsRaceStart = _isRaceStart,
                    LeaderAvgLapTime = _leaderAvgLapTime,
                    LeaderTimeToCompleteLap = _leaderTimeToCompleteLap,
                    LeaderIdx = _leaderIdx,
                    CurrentSessionNumber = _currentSessionNumber,
                    CurrentLap = _currentLap,
                    PlayerIdx = _playerIdx,
                    IsOnPitRoad = _isOnPitRoad,
                    TrackSurface = _trackSurface
                };
            }

            return new FuelViewModel()
            {
                CurrentFuelLevel = _currentFuelLevel,
                LapsCompleted = _completedLaps.Count,
                RaceLapsRemaining = _lapsRemainingInRace,

                IsInService = _isInService,
                HasBegunService = _hasBegunService,
                HasCompletedService = _hasCompletedService,
                HasResetToPits = _hasResetToPits,
                IsRollingStart = _sessionParser.StartType == StartType.Rolling,
                IsRaceStart = _isRaceStart,
                LeaderAvgLapTime = _leaderAvgLapTime,
                LeaderTimeToCompleteLap = _leaderTimeToCompleteLap,
                LeaderIdx = _leaderIdx,
                CurrentSessionNumber = _currentSessionNumber,
                CurrentLap = _currentLap,
                PlayerIdx = _playerIdx,
                IsOnPitRoad = _isOnPitRoad,
                TrackSurface = _trackSurface
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
