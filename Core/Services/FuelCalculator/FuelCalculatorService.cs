using Core.Events;
using Core.Models;
using Core.Services.FuelCalculator.LapServices;
using Core.Services.FuelCalculator.PitServices;
using Core.Services.FuelCalculator.Strategies;
using Core.Utilities.Sessions;
using Core.Utilities.Telemetries;
using iRacingSdkWrapper;
using iRacingSdkWrapper.Bitfields;
using SharpOverlay.Strategies;
using System.Collections.ObjectModel;

namespace Core.Services.FuelCalculator
{
    public class FuelCalculatorService : IFuelService
    {
        private const double _fuelCutOff = 0.3;

        private readonly FuelRepository _repository;

        private readonly ISessionParser _sessionParser;
        private readonly ITelemetryParser _telemetryParser;

        private readonly List<IFuelStrategy> _strategyList;

        private readonly LapTracker _lapTracker;
        private readonly LapCountCalculator _lapCountCalculator;
        private readonly LapAnalyzer _lapAnalyzer;

        private readonly PitManager _pitManager;
        private readonly PitTimeTracker _pitTimeTracker;
        // private readonly FinishLineLocator _finishLineLocator;
        private int _lapsRemainingInRace;
        private bool _isRaceStart;

        public SimReader SimReader { get; }

        public FuelCalculatorService() : this(new SimReader()) { }

        public FuelCalculatorService(SimReader reader)
        {
            SimReader = reader;
            _sessionParser = new SessionParser();
            _telemetryParser = new TelemetryParser();
            _lapTracker = new LapTracker();
            _lapCountCalculator = new LapCountCalculator();
            _lapAnalyzer = new LapAnalyzer();
            _pitManager = new PitManager();
            _pitTimeTracker = new PitTimeTracker();
            // _finishLineLocator = new FinishLineLocator();

            _repository = new FuelRepository();

            _strategyList = new List<IFuelStrategy>
            {
                new FullRaceStrategy(_fuelCutOff),
                new LastLapStrategy(_fuelCutOff),
                new FiveLapStrategy(_fuelCutOff)
            };

            SimReader.OnConnected += ExecuteOnConnected;
            SimReader.OnDisconnected += ExecuteOnDisconnected;
            SimReader.OnTelemetryUpdated += ExecuteOnTelemetryEvent;
            SimReader.OnSessionUpdated += ExecuteOnSessionEvent;
        }

        private void ExecuteOnDisconnected(object? sender, EventArgs args)
        {
            int trackId = _sessionParser.TrackId;
            int carID = _sessionParser.CarId;

            if (_sessionParser.SessionType == SessionType.Race || _repository.Get(trackId, carID) == null)
            {
                var fullStrat = _strategyList.First();

                var view = fullStrat.GetView();

                var fuelConsump = view.FuelConsumption;

                var lapCount = _lapTracker.GetCompletedLapsCount();
                var lapTime = _lapAnalyzer.GetLapTime(_telemetryParser.PlayerCarIdx);

                var pitStopTime = _pitTimeTracker.GetAvgPitStopTime();

                var newData = new AddFuelHistoryDTO()
                {
                    CarId = carID,
                    Consumption = fuelConsump,
                    LapCount = lapCount,
                    TrackId = trackId,
                    LapTime = lapTime,
                    PitStopTime = pitStopTime,
                };

                _repository.AddOrUpdate(newData);
                _repository.Save();
            }

            Clear();
            RaiseEvent();
        }

        private void RaiseEvent()
        {
            FuelUpdated(this, new FuelEventArgs(GetViewModel(new TelemetryOutputDTO())));
        }

        private void ExecuteOnConnected(object? sender, EventArgs args)
        {
            RaiseEvent();
        }

        public event EventHandler<FuelEventArgs> FuelUpdated = null!;

        private void Clear()
        {
            _lapsRemainingInRace = 0;

            _sessionParser.Clear();
            _telemetryParser.Clear();
            _lapTracker.Clear();
            _lapAnalyzer.Clear();
            _pitManager.Clear();
            // _finishLineLocator.Clear();

            _strategyList.ForEach(s => s.Clear());
        }

        private void ExecuteOnTelemetryEvent(object? sender, TelemetryEventArgs e)
        {
            var telemetryOutput = e.TelemetryOutput;

            ProcessTelemetryEvent(telemetryOutput);
        }

        private void ProcessTelemetryEvent(TelemetryOutputDTO telemetryOutput)
        {
            _telemetryParser.ParseCurrentSessionNumber(telemetryOutput);
            _telemetryParser.ParsePlayerCarIdx(telemetryOutput);
            _telemetryParser.ParsePlayerCarClassId(telemetryOutput);
            _telemetryParser.ParsePlayerPctOnTrack(telemetryOutput);
            _telemetryParser.ParsePositionCarIdxInPlayerClass(telemetryOutput, _sessionParser.PaceCarIdx);
            _telemetryParser.ParsePositionCarIdxForWholeRace(telemetryOutput, _sessionParser.PaceCarIdx);
            _telemetryParser.ParseCarIdxOnTrack(telemetryOutput);

            if (IsSessionStateValid(telemetryOutput.SessionState))
            {
                var driversDict = _sessionParser.Drivers;
                var driversLastLapTime = TelemetryParser.GetDriversLastLapTime(_sessionParser.PaceCarIdx, telemetryOutput.CarIdxLastLapTime);
                var lapsCompletedByCarIdx = telemetryOutput.CarIdxLapCompleted;

                _lapAnalyzer.CollectAllDriversLaps(driversDict, driversLastLapTime, lapsCompletedByCarIdx);

                RunFuelCalculations(telemetryOutput);
            }
            else if (IsSessionStateInvalid(telemetryOutput.SessionState))
            {
                Clear();
            }
            else if (!telemetryOutput.IsOnTrack
                && telemetryOutput.IsReplayPlaying
                && _lapTracker.GetCurrentLap() is not null)
            {
                _lapTracker.ResetCurrentLap();
            }

            _strategyList.ForEach(s => s.UpdateLapsOfFuelRemaining(telemetryOutput.FuelLevel));

            FuelUpdated(this, new FuelEventArgs(GetViewModel(telemetryOutput)));
        }

        private bool IsSessionStateValid(SessionStates sessionState)
        {
            return sessionState == SessionStates.Racing
                            || sessionState == SessionStates.GetInCar
                            || sessionState == SessionStates.ParadeLaps
                            || sessionState == SessionStates.Checkered;
        }

        private bool IsSessionStateInvalid(SessionStates sessionState)
        {
            return _telemetryParser.HasSwitchedSessions
                            || sessionState == SessionStates.CoolDown
                            || sessionState == SessionStates.Invalid
                            || sessionState == SessionStates.Warmup;
        }

        private void ExecuteOnSessionEvent(object? sender, SdkWrapper.SessionUpdatedEventArgs eventArgs)
        {
            var sessionInfo = eventArgs.SessionInfo;

            _sessionParser.ParseEventType(sessionInfo);

            _sessionParser.ParseSectors(sessionInfo);

            _sessionParser.ParseLapsInSession(sessionInfo, _telemetryParser.CurrentSessionNumber);
            _sessionParser.ParseCurrentSessionType(sessionInfo, _telemetryParser.CurrentSessionNumber);
            _sessionParser.ParseStartType(sessionInfo);
            _sessionParser.ParseDrivers(sessionInfo);
            _sessionParser.ParsePaceCarIdx(sessionInfo);
            _sessionParser.ParseSessions(sessionInfo);
            _sessionParser.ParseRaceType(sessionInfo);

            _sessionParser.ParseCarId(sessionInfo);
            _sessionParser.ParseTrackId(sessionInfo);
        }

        private void RunFuelCalculations(TelemetryOutputDTO telemetryOutput)
        {
            var trackSurface = telemetryOutput.TrackSurface;

            if (trackSurface == TrackSurfaces.AproachingPits && !telemetryOutput.IsOnPitRoad && telemetryOutput.PlayerTrackDistPct < 0.5)
            {
                trackSurface = TrackSurfaces.OnTrack; // WARN: EXITING PITS GETS REPORTED AS APPROACHING PITS ?????????????????????????
            }

            _pitManager.SetPitRoadStatus(telemetryOutput.IsOnPitRoad, trackSurface);
            _pitManager.SetPitServiceStatus(telemetryOutput.IsReceivingService);

            var currentLap = _lapTracker.GetCurrentLap();

            if (trackSurface == TrackSurfaces.AproachingPits && !_pitTimeTracker.IsTrackingTime && !telemetryOutput.IsOnPitRoad)
            {
                _pitTimeTracker.Start(telemetryOutput.SessionTimeRemaining);
                if (currentLap is not null)
                {
                    currentLap.IsInLap = true;
                }
            }
            else if (_pitTimeTracker.IsTrackingTime && telemetryOutput.PlayerTrackDistPct < 0.5
                    && _sessionParser.Sectors[1].StartPct - telemetryOutput.PlayerTrackDistPct < 0)
            {
                _pitTimeTracker.Stop(telemetryOutput.SessionTimeRemaining);

                if (currentLap is not null)
                {
                    currentLap.IsInLap = false; // TODO: Find more elegant way?
                    currentLap.IsOutLap = true;
                }
            }

            if (currentLap is null)
            {
                if (telemetryOutput.FuelLevel == 0 || _sessionParser.TrackId == 0)
                {
                    return;
                }

                int trackId = _sessionParser.TrackId;
                int carId = _sessionParser.CarId;

                var entry = _repository.Get(trackId, carId);

                if (entry is not null && entry.Consumption > 0)
                {
                    _lapTracker.StartNewLap(telemetryOutput.CurrentLapNumber - 1, telemetryOutput.FuelLevel + entry.Consumption);
                    _lapTracker.CompleteCurrentLap(telemetryOutput.FuelLevel, TimeSpan.FromSeconds(entry.LapTime));

                    _lapsRemainingInRace = _lapCountCalculator.CalculateLapsRemaining(telemetryOutput.PlayerTrackDistPct, telemetryOutput.SessionTimeRemaining, TimeSpan.FromSeconds(entry.LapTime));

                    foreach (var strategy in _strategyList)
                    {
                        strategy.Calculate(_lapTracker.GetPlayerLaps(), _lapsRemainingInRace);
                    }
                }

                _lapTracker.StartNewLap(telemetryOutput.CurrentLapNumber, telemetryOutput.FuelLevel);
                currentLap = _lapTracker.GetCurrentLap()!;
                currentLap.IsOutLap = true;

                var sessionState = telemetryOutput.SessionState;
                var sessionType = _sessionParser.SessionType;

                if (IsRaceStart(sessionState, telemetryOutput.CurrentLapNumber, sessionType))
                {
                    _isRaceStart = true;
                }
            }                                                                // WARN:
            else if (_isRaceStart && telemetryOutput.CurrentLapNumber == 2) // Simulator flickers quickly to lap 2 in Race
                                                                             // after the going through start finish line on lap 0 to 1
            {
                _isRaceStart = false;
            }
            else if (_pitManager.HasFinishedService())
            {
                int lapNumber = telemetryOutput.CurrentLapNumber;

                if (telemetryOutput.PlayerTrackDistPct > 0.5)
                {
                    lapNumber++;
                }

                _lapTracker.StartNewLap(lapNumber, telemetryOutput.FuelLevel);

                currentLap = _lapTracker.GetCurrentLap()!;
                currentLap.IsOutLap = true;

                _strategyList.ForEach(s => s.UpdateRefuel(currentLap.StartingFuel, _lapsRemainingInRace));

                _pitManager.ResetFinishedServiceStatus();
            }
            else if (_pitManager.HasBegunService())
            {
                if (telemetryOutput.SessionFlag != SessionFlags.Repair)
                {
                    _lapTracker.CompleteCurrentLap(telemetryOutput.FuelLevel, telemetryOutput.LastLapTime);
                }

                CalculateFuelAndLapData(telemetryOutput);

                _pitManager.ResetBegunServiceStatus();
            }
            else if (_pitManager.IsResettingToPits(telemetryOutput.EnterExitResetButton))
            {
                currentLap.StartingFuel = telemetryOutput.FuelLevel;

                _pitManager.HasResetToPits = true;
                currentLap.IsInLap = false;
                currentLap.IsOutLap = true;

                _strategyList.ForEach(s => s.UpdateRefuel(currentLap.StartingFuel, _lapsRemainingInRace));
            }
            else if (IsCrossingFinishLine(telemetryOutput.CurrentLapNumber, currentLap.Number)
                && telemetryOutput.SessionState != SessionStates.ParadeLaps)
            {
                if (!_pitManager.IsOnPitRoad())
                {
                    if (currentLap.Number != 0)
                    {
                        _lapTracker.CompleteCurrentLap(telemetryOutput.FuelLevel, telemetryOutput.LastLapTime);
                    }

                    _lapTracker.StartNewLap(telemetryOutput.CurrentLapNumber, telemetryOutput.FuelLevel);
                }
                else if (_pitManager.HasResetToPits)
                {
                    currentLap.Number = telemetryOutput.CurrentLapNumber;
                    _pitManager.HasResetToPits = false;
                    _pitTimeTracker.Reset();
                }

                CalculateFuelAndLapData(telemetryOutput);
            }
        }

        private static bool IsRaceStart(SessionStates sessionState, int currentLapNumber, SessionType sessionType)
        {
            return (sessionState == SessionStates.Racing || sessionState == SessionStates.ParadeLaps)
                                && currentLapNumber == 0 && sessionType == SessionType.Race;
        }

        private static bool IsCrossingFinishLine(int currentLapNumberTelemetry, int currentLapNumberTracked)
            => currentLapNumberTelemetry > currentLapNumberTracked;

        private void CalculateFuelAndLapData(TelemetryOutputDTO telemetryOutput)
        {
            //pitstop duration
            //history

            int leaderIdx = FindLeader();

            if (_sessionParser.SessionLaps > 0)
            {
                _lapsRemainingInRace = _lapCountCalculator.CalculateLapsRemaining(_sessionParser.SessionLaps, telemetryOutput.CarIdxLapCompleted[leaderIdx]);
            }
            else if (leaderIdx >= 0)
            {
                var leaderAverageLapTime = _lapAnalyzer.GetLapTime(leaderIdx);

                if (leaderAverageLapTime > TimeSpan.Zero)
                {
                    var timeRemainingInSession = telemetryOutput.SessionTimeRemaining;

                    // _telemetryParser.CarIdxLastPitLap.TryGetValue(leaderIdx, out int lastPitLapNumber);

                    // if (lastPitLapNumber != 0) {
                    //     timeRemainingInSession -= _pitTimeTracker.GetAvgPitStopTime();
                    // }

                    if (_sessionParser.IsMultiClassRace)
                    {
                        var raceLeaderPctOnTrack = _telemetryParser.CarIdxPctOnTrack[leaderIdx];
                        var playerAverageLapTime = _lapAnalyzer.GetLapTime(_telemetryParser.PlayerCarIdx);

                        _lapsRemainingInRace = _lapCountCalculator.CalculateLapsRemainingMultiClass(timeRemainingInSession,
                            raceLeaderPctOnTrack, telemetryOutput.PlayerTrackDistPct, leaderAverageLapTime, playerAverageLapTime, telemetryOutput.SessionFlag);
                    }
                    else
                    {
                        _lapsRemainingInRace = _lapCountCalculator.CalculateLapsRemaining(_telemetryParser.CarIdxPctOnTrack[leaderIdx], timeRemainingInSession, leaderAverageLapTime);
                    }
                }
                else
                {
                    int trackId = _sessionParser.TrackId;
                    int carId = _sessionParser.CarId;

                    var entry = _repository.Get(trackId, carId);

                    if (entry is not null)
                    {
                        _lapsRemainingInRace = _lapCountCalculator.CalculateLapsRemaining(telemetryOutput.PlayerTrackDistPct, telemetryOutput.SessionTimeRemaining, TimeSpan.FromSeconds(entry.LapTime));
                    }
                }
            }

            foreach (var strategy in _strategyList)
            {
                strategy.Calculate(_lapTracker.GetPlayerLaps(), _lapsRemainingInRace);
            }
        }

        private int FindLeader()
        {
            int leaderIdx;

            if (_sessionParser.SessionType != SessionType.Race)
            {
                leaderIdx = _telemetryParser.PlayerCarIdx;
            }
            else if (_sessionParser.IsMultiClassRace)
            {
                leaderIdx = _lapAnalyzer.GetLeaderIdx(_telemetryParser.PositionCarIdxInRace);
            }
            else
            {
                leaderIdx = _lapAnalyzer.GetLeaderIdx(_telemetryParser.PositionCarIdxInClass);
            }

            return leaderIdx;
        }

        private FuelViewModel GetViewModel(TelemetryOutputDTO telemetryOutput)
        {
            var strategies = new ObservableCollection<StrategyViewModel>
            {
                _strategyList[0].GetView(),
                _strategyList[1].GetView(),
                _strategyList[2].GetView()
            };

            var completedLaps = _lapTracker.GetPlayerLaps();

            return new FuelViewModel()
            {
                Strategies = strategies,

                ConsumedFuel = completedLaps.Sum(l => l.FuelUsed),
                CurrentFuelLevel = telemetryOutput.FuelLevel,
                LapsCompleted = completedLaps.Count,
                RaceLapsRemaining = _lapsRemainingInRace,

                AvgPitDuration = _pitTimeTracker.GetAvgPitStopTime(),
                PitDuration = _pitTimeTracker.GetPitDuration(),
                IsTrackingPit = _pitTimeTracker.IsTrackingTime,

                HasResetToPits = _pitManager.IsResettingToPits(telemetryOutput.EnterExitResetButton),
                IsRollingStart = _sessionParser.StartType == StartType.Rolling,
                SessionFlag = telemetryOutput.SessionFlag,
                IsRaceStart = _isRaceStart,
                CurrentSessionNumber = _telemetryParser.CurrentSessionNumber,
                CurrentLap = _lapTracker.GetCurrentLap(),
                TrackSurface = telemetryOutput.TrackSurface,
                SessionState = telemetryOutput.SessionState,
                IsOnPitRoad = _pitManager.IsOnPitRoad(),
                HasBegunService = _pitManager.HasBegunService(),
                HasCompletedService = _pitManager.HasFinishedService(),
            };
        }
    }
}
