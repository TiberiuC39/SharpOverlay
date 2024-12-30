using iRacingSdkWrapper;
using iRacingSdkWrapper.Bitfields;
using SharpOverlay.Models;
using SharpOverlay.Services.Base;
using SharpOverlay.Services.FuelServices.LapServices;
using SharpOverlay.Services.FuelServices.PitServices;
using SharpOverlay.Strategies;
using SharpOverlay.Utilities.Sessions;
using SharpOverlay.Utilities.Telemetries;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SharpOverlay.Services.FuelServices
{
    public class FuelCalculatorService : IFuelCalculator
    {
        private const double _fuelCutOff = 0.3;
        private readonly ISessionParser _sessionParser;
        private readonly ITelemetryParser _telemetryParser;

        private readonly List<IFuelStrategy> _strategyList;

        private readonly LapTracker _lapTracker;
        private readonly LapCountCalculator _lapCountCalculator;
        private readonly LapAnalyzer _lapAnalyzer;

        private readonly PitManager _pitManager;
        private readonly PitTimeTracker _pitTimeTracker;
        private readonly FinishLineLocator _finishLineLocator;

        private int _lapsRemainingInRace;
        private bool _isRaceStart;

        public FuelCalculatorService(SimReader simReader)
        {
            _sessionParser = new SessionParser();
            _telemetryParser = new TelemetryParser();
            _lapTracker = new LapTracker();
            _lapCountCalculator = new LapCountCalculator();
            _lapAnalyzer = new LapAnalyzer();
            _pitManager = new PitManager();
            _pitTimeTracker = new PitTimeTracker();
            _finishLineLocator = new FinishLineLocator();

            _strategyList = new List<IFuelStrategy>
            {
                new FullRaceStrategy(_fuelCutOff),
                new LastLapStrategy(_fuelCutOff),
                new FiveLapStrategy(_fuelCutOff)
            };

            simReader.OnConnected += ExecuteOnConnected;
            simReader.OnDisconnected += ExecuteOnDisconnected;
            simReader.OnTelemetryUpdated += ExecuteOnTelemetryEvent;
            simReader.OnSessionUpdated += ExecuteOnSessionEvent;
        }

        private void ExecuteOnDisconnected(object? sender, EventArgs args)
        {
            Clear();
            RaiseEvent();
        }

        private void RaiseEvent()
        {
            FuelUpdated(this, new FuelEventArgs(GetViewModel(new SimulationOutputDTO())));
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
            _finishLineLocator.Clear();

            _strategyList.ForEach(s => s.Clear());
        }

        private void ExecuteOnTelemetryEvent(object? sender, SdkWrapper.TelemetryUpdatedEventArgs eventArgs)
        {
            var telemetry = eventArgs.TelemetryInfo;

            _telemetryParser.ParseCurrentSessionNumber(telemetry);
            _telemetryParser.ParsePlayerCarIdx(telemetry);
            _telemetryParser.ParsePlayerCarClassId(telemetry);
            _telemetryParser.ParsePlayerPctOnTrack(telemetry);
            _telemetryParser.ParsePositionCarIdxInPlayerClass(telemetry, _sessionParser.PaceCarIdx);
            _telemetryParser.ParsePositionCarIdxForWholeRace(telemetry, _sessionParser.PaceCarIdx);
            _telemetryParser.ParseCarIdxOnTrack(telemetry);

            var simulationOutput = new SimulationOutputDTO(telemetry);

            if (IsSessionStateValid(simulationOutput.SessionState))
            {
                var driversDict = _sessionParser.Drivers;
                var driversLastLapTime = TelemetryParser.GetDriversLastLapTime(_sessionParser.PaceCarIdx, simulationOutput.CarIdxLastLapTime);
                var lapsCompletedByCarIdx = simulationOutput.CarIdxLapCompleted;

                _lapAnalyzer.CollectAllDriversLaps(driversDict, driversLastLapTime, lapsCompletedByCarIdx);

                RunFuelCalculations(simulationOutput);
            }
            else if (IsSessionStateInvalid(simulationOutput.SessionState))
            {
                Clear();
            }
            else if (!simulationOutput.IsOnTrack
                && telemetry.IsReplayPlaying.Value
                && _lapTracker.GetCurrentLap() is not null)
            {
                _lapTracker.ResetCurrentLap();
            }

            _strategyList.ForEach(s => s.UpdateLapsOfFuelRemaining(simulationOutput.FuelLevel));

            FuelUpdated(this, new FuelEventArgs(GetViewModel(simulationOutput)));
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

            _sessionParser.ParseSectors(sessionInfo);

            _sessionParser.ParseLapsInSession(sessionInfo, _telemetryParser.CurrentSessionNumber);
            _sessionParser.ParseCurrentSessionType(sessionInfo, _telemetryParser.CurrentSessionNumber);
            _sessionParser.ParseStartType(sessionInfo);
            _sessionParser.ParseDrivers(sessionInfo);
            _sessionParser.ParsePaceCarIdx(sessionInfo);
            _sessionParser.ParseSessions(sessionInfo);
            _sessionParser.ParseRaceType(sessionInfo);
        }

        private void RunFuelCalculations(SimulationOutputDTO simulationOutput)
        {
            _pitManager.SetPitRoadStatus(simulationOutput.IsOnPitRoad, simulationOutput.TrackSurface);
            _pitManager.SetPitServiceStatus(simulationOutput.IsReceivingService);

            if (simulationOutput.TrackSurface == TrackSurfaces.AproachingPits && !_pitTimeTracker.IsTrackingTime)
            {
                _pitTimeTracker.StartPitDurationTracking(simulationOutput.SessionTimeRemaining);
            }
            else if (_pitTimeTracker.IsTrackingTime && _sessionParser.Sectors[1].StartPct - simulationOutput.PlayerTrackDistPct < 0.005)
            {
                _pitTimeTracker.StopPitDurationTracking(simulationOutput.SessionTimeRemaining);
            }

            var currentLap = _lapTracker.GetCurrentLap();

            if (currentLap is null)
            {
                _lapTracker.StartNewLap(simulationOutput.CurrentLapNumber, simulationOutput.FuelLevel);

                var sessionState = simulationOutput.SessionState;
                var sessionType = _sessionParser.SessionType;

                if (IsRaceStart(sessionState, simulationOutput.CurrentLapNumber, sessionType))
                {
                    _isRaceStart = true;
                }
            }
            else if (_isRaceStart && simulationOutput.CurrentLapNumber == 2) // Simulator flickers quickly to lap 2 in Race
                                                                             // after the going through start finish line on lap 0 to 1
            {
                _isRaceStart = false;
            }
            else if (_pitManager.HasResetToPits(simulationOutput.EnterExitResetButton) && simulationOutput.IsOnTrack)
            {
                currentLap.StartingFuel = simulationOutput.FuelLevel;

                _strategyList.ForEach(s => s.UpdateRefuel(currentLap.StartingFuel, _lapsRemainingInRace));
            }
            else if (_pitManager.HasBegunService())
            {
                if (simulationOutput.SessionFlag != SessionFlags.Repair)
                {
                    _lapTracker.CompleteCurrentLap(simulationOutput.FuelLevel, simulationOutput.LastLapTime);
                }

                CalculateFuelAndLapData(simulationOutput);

                _pitManager.ResetBegunServiceStatus();
            }
            else if (_pitManager.HasFinishedService())
            {
                int lastLapnumber = _lapTracker.GetPlayerLaps().Last().Number;

                _lapTracker.StartNewLap(++lastLapnumber, simulationOutput.FuelLevel);

                currentLap = _lapTracker.GetCurrentLap()!;

                _strategyList.ForEach(s => s.UpdateRefuel(currentLap.StartingFuel, _lapsRemainingInRace));

                _pitManager.ResetFinishedServiceStatus();
            }
            else if (IsCrossingFinishLine(simulationOutput.CurrentLapNumber, currentLap.Number)
                && simulationOutput.SessionState != SessionStates.ParadeLaps)
            {
                if (!_finishLineLocator.IsFinishLineKnown() && _pitManager.IsComingOutOfPits())
                {
                    _finishLineLocator.DetermineFinishLineLocation(simulationOutput.PlayerTrackDistPct);
                }

                if (_finishLineLocator.IsFinishLineAfterPits() && _pitManager.IsComingOutOfPits())
                {
                    currentLap.Number++;

                    _pitManager.ResetIsComingOutOfPits();
                }
                else if (!_pitManager.IsOnPitRoad())
                {
                    _lapTracker.CompleteCurrentLap(simulationOutput.FuelLevel, simulationOutput.LastLapTime);
                    _lapTracker.StartNewLap(simulationOutput.CurrentLapNumber, simulationOutput.FuelLevel);
                }

                CalculateFuelAndLapData(simulationOutput);
            }
        }

        private static bool IsRaceStart(SessionStates sessionState, int currentLapNumber, SessionType sessionType)
        {
            return (sessionState == SessionStates.Racing || sessionState == SessionStates.ParadeLaps)
                                && currentLapNumber == 0 && sessionType == SessionType.Race;
        }

        private static bool IsCrossingFinishLine(int currentLapNumberTelemetry, int currentLapNumberTracked)
            => currentLapNumberTelemetry > currentLapNumberTracked;

        private void CalculateFuelAndLapData(SimulationOutputDTO simulationOutput)
        {
            int leaderIdx = FindLeader();

            if (_sessionParser.SessionLaps > 0)
            {
                _lapsRemainingInRace = _lapCountCalculator.CalculateLapsRemaining(_sessionParser.SessionLaps, simulationOutput.CarIdxLapCompleted[leaderIdx]);
            }
            else
            {
                var leaderAverageLapTime = _lapAnalyzer.GetLapTime(leaderIdx);

                if (_sessionParser.IsMultiClassRace)
                {
                    var raceLeaderPctOnTrack = _telemetryParser.CarIdxPctOnTrack[leaderIdx];
                    var playerAverageLapTime = _lapAnalyzer.GetLapTime(_telemetryParser.PlayerCarIdx);

                    _lapsRemainingInRace = _lapCountCalculator.CalculateLapsRemainingMultiClass(simulationOutput.SessionTimeRemaining,
                        raceLeaderPctOnTrack, simulationOutput.PlayerTrackDistPct, leaderAverageLapTime, playerAverageLapTime);
                }
                else if (leaderIdx >= 0)
                {
                    _lapsRemainingInRace = _lapCountCalculator.CalculateLapsRemaining(_telemetryParser.CarIdxPctOnTrack[leaderIdx], simulationOutput.SessionTimeRemaining, leaderAverageLapTime);
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

        private FuelViewModel GetViewModel(SimulationOutputDTO simulationOutput)
        {
            var strategies = new ObservableCollection<StrategyViewModel>
            {
                _strategyList[0].GetView(),
                _strategyList[1].GetView(),
                _strategyList[2].GetView()
            };

            var completedLaps = _lapTracker.GetPlayerLaps();

            if (completedLaps.Count > 1)
            {
                return new FuelViewModel()
                {
                    Strategies = strategies,

                    ConsumedFuel = completedLaps.Sum(l => l.FuelUsed),
                    LapsCompleted = completedLaps.Count,
                    CurrentFuelLevel = simulationOutput.FuelLevel,
                    RaceLapsRemaining = _lapsRemainingInRace,

                    IsOpen = simulationOutput.IsOnTrack,

                    IsRollingStart = _sessionParser.StartType == StartType.Rolling,
                    IsRaceStart = _isRaceStart,
                    CurrentSessionNumber = _telemetryParser.CurrentSessionNumber,
                    CurrentLap = _lapTracker.GetCurrentLap(),
                    TrackSurface = simulationOutput.TrackSurface,
                    SessionState = simulationOutput.SessionState,
                    IsOnPitRoad = _pitManager.IsOnPitRoad(),
                    HasBegunService = _pitManager.HasBegunService(),
                    HasCompletedService = _pitManager.HasFinishedService(),
                };
            }

            return new FuelViewModel()
            {
                Strategies = strategies,

                ConsumedFuel = completedLaps.Sum(l => l.FuelUsed),
                CurrentFuelLevel = simulationOutput.FuelLevel,
                LapsCompleted = completedLaps.Count,
                RaceLapsRemaining = _lapsRemainingInRace,

                IsOpen = simulationOutput.IsOnTrack,

                IsRollingStart = _sessionParser.StartType == StartType.Rolling,
                IsRaceStart = _isRaceStart,
                CurrentSessionNumber = _telemetryParser.CurrentSessionNumber,
                CurrentLap = _lapTracker.GetCurrentLap(),
                TrackSurface = simulationOutput.TrackSurface,
                SessionState = simulationOutput.SessionState,
                IsOnPitRoad = _pitManager.IsOnPitRoad(),
                HasBegunService = _pitManager.HasBegunService(),
                HasCompletedService = _pitManager.HasFinishedService(),
            };
        }
    }
}
