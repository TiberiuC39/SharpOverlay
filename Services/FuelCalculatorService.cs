using iRacingSdkWrapper;
using SharpOverlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace SharpOverlay.Services
{
    public class FuelCalculatorService : IFuelCalculator, INotifyPropertyChanged
    {
        private const int DefaultTickRate = 2;

        private readonly iRacingDataService _iRacingService;
        private readonly ISessionParser _sessionParser;
        private readonly ITelemetryParser _telemetryParser;

        private readonly Dictionary<int, Lap> _completedLaps = [];
        private readonly Dictionary<int, List<Lap>> _driverLaps = [];
        private Lap? _currentLap;
        private double _lapsRemainingInRace;
        private bool _isInService;
        private bool _hasBegunService;
        private TimeSpan _averageLapTime;
        private bool _hasCompletedService;
        private float _avgFuelPerLap;
        private double _refuelRequired;
        private float _fiveLapAverage;
        private double _fiveLapRefuelRequired;
        private bool _hasResetToPits;
        private bool _isRaceStart;
        private TimeSpan _leaderAvgLapTime;
        private TimeSpan _leaderTimeToCompleteLap;
        private TrackSurfaces _trackSurface;
        private int _leaderIdx;
        private int _playerIdx;
        private bool _isOnPitRoad;

        private bool _isInCar;
        public bool IsInCar
        {
            get => _isInCar;
            private set
            {
                if (_isInCar != value)
                {
                    _isInCar = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsInCar)));
                }
            }
        }

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

            _sessionParser.ParseCurrentSessionType(sessionInfo, _telemetryParser.CurrentSessionNumber);
            _sessionParser.ParseStartType(sessionInfo);
            _sessionParser.ParseDrivers(sessionInfo);
            _sessionParser.ParseLapsInSession(sessionInfo, _telemetryParser.CurrentSessionNumber);

            _telemetryParser.ParsePlayerCarIdx(telemetry);
            _telemetryParser.ParsePlayerCarClassId(telemetry);
            _telemetryParser.ParsePositionCarIdxInPlayerClass(telemetry);
            _telemetryParser.ParseFuelLevel(telemetry);

            CalculateOtherDriversLaps(telemetry);

            FindLeaderIdxInClass();
            CalculateLeaderLapTime(telemetry);

            var timeRemaining = _telemetryParser.GetTimeRemaining(telemetry);

            CalculateLapsRemaining(timeRemaining);

            CalculateRefuel(_telemetryParser.FuelLevel);
        }

        public event EventHandler<FuelEventArgs> FuelUpdated = null!;
        public event PropertyChangedEventHandler? PropertyChanged;

        private void Reset()
        {
            Clear();
            Initialize();
        }

        private void Clear()
        {
            _completedLaps.Clear();
            _driverLaps.Clear();
            _currentLap = null;
            _lapsRemainingInRace = 0;
            _hasBegunService = false;
            _isInService = false;
            _refuelRequired = 0;
            _fiveLapAverage = 0;
            _fiveLapRefuelRequired = 0;
            
            _sessionParser.Clear();
            _telemetryParser.Clear();

            FuelUpdated.Invoke(this, new FuelEventArgs(GetViewModel()));
        }

        private void ExecuteOnTelemetryEvent(object? sender, SdkWrapper.TelemetryUpdatedEventArgs eventArgs)
        {
            var telemetry = eventArgs.TelemetryInfo;

            _telemetryParser.ParseCurrentSessionNumber(telemetry);
            _telemetryParser.ParseSessionState(telemetry);

            _trackSurface = telemetry.PlayerTrackSurface.Value;

            IsInCar = telemetry.IsOnTrack.Value;

            _telemetryParser.ParseFuelLevel(telemetry);
            _telemetryParser.ParsePositionCarIdxInPlayerClass(telemetry, _sessionParser.SessionType);
            _telemetryParser.ParsePlayerPctOnTrack(telemetry);

            if (IsSessionStateValid())
            {
                CalculateOtherDriversLaps(telemetry);
                FindLeaderIdxInClass();
                CalculateLeaderLapTime(telemetry);

                ProcessTelemetryUpdate(telemetry);
            }
            else if (IsSessionStateInvalid())
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

        private bool IsSessionStateValid()
        {
            return (_telemetryParser.SessionState == SessionStates.Racing
                            || _telemetryParser.SessionState == SessionStates.GetInCar
                            || _telemetryParser.SessionState == SessionStates.CoolDown
                            || _telemetryParser.SessionState == SessionStates.ParadeLaps
                            || _telemetryParser.SessionState == SessionStates.Checkered);
                            //&& telemetry.IsOnTrack.Value;
        }

        private bool IsSessionStateInvalid()
        {
            return _telemetryParser.HasSwitchedSessions
                            || _telemetryParser.SessionState == SessionStates.Invalid
                            || _telemetryParser.SessionState == SessionStates.Warmup;
                            //|| telemetry.SessionState.Value == SessionStates.GetInCar;
        }

        private void ExecuteOnSessionEvent(object? sender, SdkWrapper.SessionUpdatedEventArgs eventArgs)
        {
            var sessionInfo = eventArgs.SessionInfo;

            _sessionParser.ParseLapsInSession(sessionInfo, _telemetryParser.CurrentSessionNumber);
            _sessionParser.ParseCurrentSessionType(sessionInfo, _telemetryParser.CurrentSessionNumber);

            _sessionParser.ParseDrivers(sessionInfo);
            _sessionParser.ParseSessions(sessionInfo);
        }

        private void CalculateOtherDriversLaps(TelemetryInfo telemetry)
        {
            var driversLastLapTimes = TelemetryParser.GetDriversLastLapTime(telemetry);
            foreach ((int idx, _) in _sessionParser.Drivers)
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

            if (leaderLapDistance < 0)
            {
                leaderLapDistance = 0;
            }

            if (validLeaderLaps.Any())
            {
                _leaderAvgLapTime = TimeSpan.FromSeconds(validLeaderLaps.Average(l => l.Time.TotalSeconds));

                if (leaderLapDistance >= 0)
                {
                    _leaderTimeToCompleteLap = (1 - leaderLapDistance) * _leaderAvgLapTime;
                }
            }
            else
            {
                _leaderAvgLapTime = _sessionParser.GetBestLapTime(_leaderIdx, _telemetryParser.CurrentSessionNumber);
                _leaderTimeToCompleteLap = (1 - leaderLapDistance) * _leaderAvgLapTime;
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

            UpdatePitServiceStatus(isReceivingPitService);

            _hasResetToPits = HasResetToPits(telemetry);

            var currentSession = _sessionParser.Sessions[_telemetryParser.CurrentSessionNumber];

            float currentFuelLevel = _telemetryParser.FuelLevel;

            if (_currentLap is null)
            {
                StartNewLap(currentLapNumber, currentFuelLevel);

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
                _currentLap.StartingFuel = currentFuelLevel;

                CalculateRefuel(currentFuelLevel);
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
                StartNewLap(currentLapNumber, currentFuelLevel);

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

                StartNewLap(currentLapNumber, currentFuelLevel);

                CalculateFuelAndLapData(timeRemaining);
            }
        }        

        private bool HasResetToPits(TelemetryInfo telemetry)
        {
            return telemetry.EnterExitReset.Value == 0;
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
            var sessionType = _sessionParser.SessionType;

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

                if (_sessionParser.SessionType == SessionType.Qualifying)
                {
                    completedLaps--;
                }

                _lapsRemainingInRace = _sessionParser.SessionLaps - completedLaps;
            }
            else if (_sessionParser.SessionType == SessionType.Practice && _averageLapTime > TimeSpan.Zero)
            {
                TimeSpan playerTimeToCompleteLap = (1 - _telemetryParser.PlayerPctOnTrack) * _averageLapTime;

                _lapsRemainingInRace = Math.Ceiling((timeRemainingInSession - playerTimeToCompleteLap) / _averageLapTime) + 1;
            }
            else if (_leaderAvgLapTime > TimeSpan.Zero)
            {
                _lapsRemainingInRace = Math.Ceiling((timeRemainingInSession - _leaderTimeToCompleteLap) / _leaderAvgLapTime) + 1;
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

        private void CalculateRefuel(float fuelLeftInCar)
        {
            float fuelCutoff = 0.3f;

            if (_lapsRemainingInRace > 0)
            {
                if (_avgFuelPerLap > 0)
                {
                    double fuelRequiredToEndTheRace = _lapsRemainingInRace * _avgFuelPerLap;

                    _refuelRequired = fuelRequiredToEndTheRace - fuelLeftInCar;

                    double fuelAtEndOfRace = fuelLeftInCar - fuelRequiredToEndTheRace;

                    if (fuelAtEndOfRace > 0 && fuelAtEndOfRace < fuelCutoff)
                    {
                        double difference = fuelCutoff - fuelAtEndOfRace;
                        _refuelRequired += difference;
                    }
                }

                if (_completedLaps.Count > 5)
                {
                    double fiveLapFuelRequired = _lapsRemainingInRace * _fiveLapAverage;

                    _fiveLapRefuelRequired = fiveLapFuelRequired - fuelLeftInCar;

                    double fiveLapEndOfRaceFuel = fuelLeftInCar - fiveLapFuelRequired;

                    if (fiveLapEndOfRaceFuel > 0 && fiveLapEndOfRaceFuel < fuelCutoff)
                    {
                        double fiveLapDiff = fuelCutoff - fiveLapEndOfRaceFuel;
                        _fiveLapRefuelRequired += fiveLapDiff;
                    }
                }
                else
                {
                    _fiveLapRefuelRequired = _refuelRequired;
                }
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
            _currentLap!.EndingFuel = _telemetryParser.FuelLevel;

            TimeSpan lapTime = TimeSpan.FromSeconds(lastLapTime);

            if (_averageLapTime > TimeSpan.Zero && lapTime / _averageLapTime > 1.3)
            {
                lapTime = TimeSpan.FromSeconds(-1);
            }

            _currentLap.Time = lapTime;
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
                var lastLap = _completedLaps.Last().Value;
                double fuelRequiredToEndTheRace = _lapsRemainingInRace * lastLap.FuelUsed;

                double lastLapRefuelRequired = fuelRequiredToEndTheRace - lastLap.EndingFuel;

                double fuelAtEndOfRace = lastLap.EndingFuel - fuelRequiredToEndTheRace;

                if (fuelAtEndOfRace > 0 && fuelAtEndOfRace < 0.3f)
                {
                    double difference = 0.3f - fuelAtEndOfRace;
                    lastLapRefuelRequired += difference;
                }

                return new FuelViewModel()
                {
                    ConsumedFuel = _completedLaps.Sum(l => l.Value.FuelUsed),
                    AverageFuelConsumption = _avgFuelPerLap,
                    LapsCompleted = _completedLaps.Count,
                    CurrentFuelLevel = _telemetryParser.FuelLevel,
                    LapsOfFuelRemaining = _telemetryParser.FuelLevel / _avgFuelPerLap,
                    RaceLapsRemaining = _lapsRemainingInRace,
                    RefuelRequired = _refuelRequired,

                    LastLapConsumption = lastLap.FuelUsed,
                    LastLapRefuelRequired = lastLapRefuelRequired,
                    LastLapLapsOfFuelRemaining = lastLap.EndingFuel / lastLap.FuelUsed,

                    FiveLapAverage = _fiveLapAverage,
                    FiveLapRefuelRequired = _fiveLapRefuelRequired,
                    FiveLapLapsOfFuelRemaining = _telemetryParser.FuelLevel / _fiveLapAverage,

                    IsInService = _isInService,
                    HasBegunService = _hasBegunService,
                    HasCompletedService = _hasCompletedService,
                    HasResetToPits = _hasResetToPits,
                    IsRollingStart = _sessionParser.StartType == StartType.Rolling,
                    IsRaceStart = _isRaceStart,
                    LeaderAvgLapTime = _leaderAvgLapTime,
                    LeaderTimeToCompleteLap = _leaderTimeToCompleteLap,
                    LeaderIdx = _leaderIdx,
                    CurrentSessionNumber = _telemetryParser.CurrentSessionNumber,
                    CurrentLap = _currentLap,
                    PlayerIdx = _playerIdx,
                    IsOnPitRoad = _isOnPitRoad,
                    TrackSurface = _trackSurface,
                    SessionState = _telemetryParser.SessionState
                };
            }

            return new FuelViewModel()
            {
                CurrentFuelLevel = _telemetryParser.FuelLevel,
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
                CurrentSessionNumber = _telemetryParser.CurrentSessionNumber,
                CurrentLap = _currentLap,
                PlayerIdx = _playerIdx,
                IsOnPitRoad = _isOnPitRoad,
                TrackSurface = _trackSurface,
                SessionState = _telemetryParser.SessionState
            };
        }
    }
}
