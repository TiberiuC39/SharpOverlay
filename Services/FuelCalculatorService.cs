using iRacingSdkWrapper;
using iRacingSdkWrapper.JsonModels;
using SharpOverlay.Models;
using SharpOverlay.Services.LapServices;
using SharpOverlay.Strategies;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Documents;

namespace SharpOverlay.Services
{
    public class FuelCalculatorService : IFuelCalculator
    {
        private const int _defaultTickRate = 2;
        private const double _fuelCutOff = 0.3;

        private readonly iRacingDataService _iRacingService;
        private readonly ISessionParser _sessionParser;
        private readonly ITelemetryParser _telemetryParser;

        private readonly List<IFuelStrategy> _strategyList;

        private readonly ILapTracker _lapTracker;
        private readonly ILapTimeCalculator _lapTimeCalculator;
        private readonly ILapCountCalculator _lapCountCalculator;
        private readonly ILapDataAnalyzer _lapAnalyzer;

        private readonly IPitManager _pitManager;

        private readonly Dictionary<int, List<Lap>> _driverLaps = [];
        private double _lapsRemainingInRace;
        private bool _isRaceStart;
        private TrackSurfaces _trackSurface;
        private int _leaderIdx;
        private int _playerIdx;
        private bool _isOnPitRoad;

        public bool IsInCar { get; private set; }
        

        public FuelCalculatorService(int tickRate = _defaultTickRate)
        {
            _iRacingService = new iRacingDataService();
            _sessionParser = new SessionParser();
            _telemetryParser = new TelemetryParser();
            _lapTracker = new LapTracker();
            _lapTimeCalculator = new LapTimeCalculator();
            _lapCountCalculator = new LapCountCalculator();
            _lapAnalyzer = new LapDataAnalyzer();
            _pitManager = new PitManager();

            _strategyList = new List<IFuelStrategy>
            {
                new FullRaceStrategy(_fuelCutOff),
                new LastLapStrategy(_fuelCutOff),
                new FiveLapStrategy(_fuelCutOff)
            };
            
            _iRacingService.HookUpToConnectedEvent(ExecuteOnConnected);
            _iRacingService.HookUpToSessionEvent(ExecuteOnSessionEvent);
            _iRacingService.HookUpToTelemetryEvent(ExecuteOnTelemetryEvent);
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
            _telemetryParser.ParsePositionCarIdxInPlayerClass(telemetry, _sessionParser.SessionType);
            _telemetryParser.ParseFuelLevel(telemetry);
            _telemetryParser.ParseCarIdxOnTrack(telemetry);

            _lapAnalyzer.CollectAllDriversLaps(_sessionParser.Drivers,
                TelemetryParser.GetDriversLastLapTime(telemetry, _sessionParser.SessionType),
                telemetry.CarIdxLapCompleted.Value);

            int leaderIdx = _lapAnalyzer.FindLeaderIdxInClass(_telemetryParser.PositionCarIdxInClass);
            var averageLapTime = GetAverageLapTime(leaderIdx);

            var timeRemaining = _telemetryParser.GetTimeRemaining(telemetry);

            CalculateLapsRemaining(leaderIdx, timeRemaining, averageLapTime);

            _strategyList.ForEach(s => s.UpdateRefuel((double) _telemetryParser.FuelLevel, _sessionParser.SessionLaps));
        }

        public event EventHandler<FuelEventArgs> FuelUpdated = null!;

        private void Reset()
        {
            Clear();
            Initialize();
        }

        private void Clear()
        {
            _driverLaps.Clear();
            _lapsRemainingInRace = 0;
            
            _sessionParser.Clear();
            _telemetryParser.Clear();
            _lapTracker.Clear();

            _strategyList.ForEach(s => s.Clear());

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
            _telemetryParser.ParseCarIdxOnTrack(telemetry);

            if (IsSessionStateValid())
            {
                _lapAnalyzer.CollectAllDriversLaps(_sessionParser.Drivers,
                    TelemetryParser.GetDriversLastLapTime(telemetry, _sessionParser.SessionType),
                    telemetry.CarIdxLapCompleted.Value);

                ProcessTelemetryUpdate(telemetry);
            }
            else if (IsSessionStateInvalid())
            {
                Reset();
            }
            else if (!IsInCar
                && telemetry.IsReplayPlaying.Value
                && _lapTracker.GetCurrentLap() is not null)
            {
                _lapTracker.ResetCurrentLap();
            }

            _strategyList.ForEach(s => s.UpdateLapsOfFuelRemaining(_telemetryParser.FuelLevel));

            FuelUpdated(this, new FuelEventArgs(GetViewModel()));
        }

        private bool IsSessionStateValid()
        {
            return (_telemetryParser.SessionState == SessionStates.Racing
                            || _telemetryParser.SessionState == SessionStates.GetInCar
                            || _telemetryParser.SessionState == SessionStates.CoolDown
                            || _telemetryParser.SessionState == SessionStates.ParadeLaps
                            || _telemetryParser.SessionState == SessionStates.Checkered);
        }

        private bool IsSessionStateInvalid()
        {
            return _telemetryParser.HasSwitchedSessions
                            || _telemetryParser.SessionState == SessionStates.Invalid
                            || _telemetryParser.SessionState == SessionStates.Warmup;
        }

        private void ExecuteOnSessionEvent(object? sender, SdkWrapper.SessionUpdatedEventArgs eventArgs)
        {
            var sessionInfo = eventArgs.SessionInfo;

            _sessionParser.ParseLapsInSession(sessionInfo, _telemetryParser.CurrentSessionNumber);
            _sessionParser.ParseCurrentSessionType(sessionInfo, _telemetryParser.CurrentSessionNumber);

            _sessionParser.ParseDrivers(sessionInfo);
            _sessionParser.ParseSessions(sessionInfo);
        }

        private void ProcessTelemetryUpdate(TelemetryInfo telemetry)
        {
            int currentLapNumber = telemetry.Lap.Value;
            bool isOnPitRoad = telemetry.IsOnPitRoad.Value;
            bool isReceivingPitService = telemetry.IsPitstopActive.Value;

            _pitManager.UpdatePitRoadStatus(isOnPitRoad);
            _pitManager.UpdatePitServiceStatus(isReceivingPitService);

            double currentFuelLevel = _telemetryParser.FuelLevel;

            var currentLap = _lapTracker.GetCurrentLap();
            int enterExitResetButton = telemetry.EnterExitReset.Value;

            if (currentLap is null)
            {
                _lapTracker.StartNewLap(currentLapNumber, currentFuelLevel);

                var sessionState = _telemetryParser.SessionState;
                var sessionType = _sessionParser.SessionType;

                if (IsRaceStart(sessionState, currentLapNumber, sessionType))
                {
                    _isRaceStart = true;
                }
            }
            else if (_isRaceStart && currentLapNumber == 2) // Simulator flickers quickly to lap 2 after the going through start finish line on lap 0 to 1
            {
                _isRaceStart = false;
            }
            else if (_pitManager.HasResetToPits(enterExitResetButton))
            {
                if (currentLapNumber == 0)
                {
                    currentLap.Number++;
                }
                currentLap.StartingFuel = currentFuelLevel;

                _strategyList.ForEach(s => s.UpdateRefuel(currentLap.StartingFuel, (int) _lapsRemainingInRace));
            }
            else if (_pitManager.HasBegunService())
            {
                var lastLapTime = TimeSpan.FromSeconds(telemetry.LapLastLapTime.Value);
                double timeRemaining = telemetry.SessionTimeRemain.Value;

                _lapTracker.CompleteCurrentLap(_telemetryParser.FuelLevel, lastLapTime);
                CalculateFuelAndLapData(timeRemaining);

                _pitManager.ResetBegunServiceStatus();
            }
            else if (_pitManager.HasFinishedService())
            {
                int lastLapnumber = _lapTracker.GetCompletedLaps().Last().Number;

                if (lastLapnumber == currentLapNumber)
                {
                    currentLapNumber++;
                }

                _lapTracker.StartNewLap(currentLapNumber, currentFuelLevel);

                _strategyList.ForEach(s => s.UpdateRefuel(currentLap.StartingFuel, (int)_lapsRemainingInRace));

                _pitManager.ResetFinishedServiceStatus();
            }
            else if (IsCrossingFinishLine(currentLapNumber, currentLap.Number) && !_isOnPitRoad && telemetry.SessionState.Value != SessionStates.ParadeLaps)
            {
                var lastLapTime = TimeSpan.FromSeconds(telemetry.LapLastLapTime.Value);
                double timeRemaining = telemetry.SessionTimeRemain.Value;

                _lapTracker.CompleteCurrentLap(_telemetryParser.FuelLevel, lastLapTime);
                _lapTracker.StartNewLap(currentLapNumber, currentFuelLevel);

                CalculateFuelAndLapData(timeRemaining);
            }
        }

        private static bool IsRaceStart(SessionStates sessionState, int currentLapNumber, SessionType sessionType)
        {
            return (sessionState == SessionStates.Racing || sessionState == SessionStates.ParadeLaps)
                                && currentLapNumber == 0 && sessionType == SessionType.Race;
        }

        private static bool IsCrossingFinishLine(int currentLapNumberTelemetry, int currentLapNumberTracked)
            => currentLapNumberTelemetry > currentLapNumberTracked;

        private void CalculateFuelAndLapData(double timeRemaining)
        {
            int leaderIdx;

            if (_sessionParser.SessionType == SessionType.Practice)
            {
                leaderIdx = _playerIdx;
            }
            else
            {
                leaderIdx = _lapAnalyzer.FindLeaderIdxInClass(_telemetryParser.PositionCarIdxInClass);
            }

            var averageLapTime = GetAverageLapTime(leaderIdx);

            var timeRemainingInSession = TimeSpan.FromSeconds(timeRemaining);

            CalculateLapsRemaining(leaderIdx, timeRemainingInSession, averageLapTime);

            foreach (var strategy in _strategyList)
            {
                strategy.Calculate(_lapTracker.GetCompletedLaps(), (int) _lapsRemainingInRace);
            }
        }

        private void CalculateLapsRemaining(int leaderIdx, TimeSpan timeRemainingInSession, TimeSpan averageLapTime)
        {
            if (_sessionParser.SessionLaps > 0)
            {
                int completedLaps = _lapTracker.GetCompletedLapsCount();
                var startType = _sessionParser.StartType;
                var sessionType = _sessionParser.SessionType;

                _lapsRemainingInRace = _lapCountCalculator.CalculateLapsRemaining(_sessionParser.SessionLaps, completedLaps, sessionType, startType);
            }
            else if (averageLapTime > TimeSpan.Zero)
            {
                float leaderPctOnTrack = _telemetryParser.CarIdxPctOnTrack[leaderIdx];

                _lapsRemainingInRace = _lapCountCalculator.CalculateLapsRemaining(leaderPctOnTrack, timeRemainingInSession, averageLapTime);
            }
        }

        private TimeSpan GetAverageLapTime(int driverIdx)
        {
            if (driverIdx < 0)
            {
                return TimeSpan.Zero;
            }

            var driverLaps = _lapAnalyzer.GetDriversLaps()[driverIdx];

            return _lapTimeCalculator.CalculateLapTime(driverLaps);
        }

        private FuelViewModel GetViewModel()
        {
            var strategies = new ObservableCollection<StrategyViewModel>
            {
                _strategyList[0].GetView(),
                _strategyList[1].GetView(),
                _strategyList[2].GetView()
            };

            var completedLaps = _lapTracker.GetCompletedLaps();

            if (completedLaps.Count > 1)
            {
                return new FuelViewModel()
                {
                    Strategies = strategies,

                    ConsumedFuel = completedLaps.Sum(l => l.FuelUsed),
                    LapsCompleted = completedLaps.Count,
                    CurrentFuelLevel = _telemetryParser.FuelLevel,
                    RaceLapsRemaining = _lapsRemainingInRace,

                    IsRollingStart = _sessionParser.StartType == StartType.Rolling,
                    IsRaceStart = _isRaceStart,
                    LeaderIdx = _leaderIdx,
                    CurrentSessionNumber = _telemetryParser.CurrentSessionNumber,
                    CurrentLap = _lapTracker.GetCurrentLap(),
                    PlayerIdx = _playerIdx,
                    IsOnPitRoad = _isOnPitRoad,
                    TrackSurface = _trackSurface,
                    SessionState = _telemetryParser.SessionState
                };
            }

            return new FuelViewModel()
            {
                Strategies = strategies,

                CurrentFuelLevel = _telemetryParser.FuelLevel,
                LapsCompleted = completedLaps.Count,
                RaceLapsRemaining = _lapsRemainingInRace,

                IsRollingStart = _sessionParser.StartType == StartType.Rolling,
                IsRaceStart = _isRaceStart,
                LeaderIdx = _leaderIdx,
                CurrentSessionNumber = _telemetryParser.CurrentSessionNumber,
                CurrentLap = _lapTracker.GetCurrentLap(),
                PlayerIdx = _playerIdx,
                IsOnPitRoad = _isOnPitRoad,
                TrackSurface = _trackSurface,
                SessionState = _telemetryParser.SessionState
            };
        }
    }
}
