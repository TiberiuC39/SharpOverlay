
using iRacingSdkWrapper;
using SharpOverlay.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpOverlay.Services
{
    public class iRacingTelemetryService
    {
        private readonly SdkWrapper _sdk;
        private TelemetryOutput? _telemetryOutput;
        private RaceDataOutput? _sessionOutput;

        public iRacingTelemetryService()
        {
            _sdk = iRacingDataService.Wrapper;

            _sdk.TelemetryUpdated += TelemetryUpdatedEvent;
            _sdk.SessionInfoUpdated += SessionUpdatedEvent;
            _sdk.Disconnected += ExecuteOnDisconnectedEvent;
        }

        public RaceDataOutput RequestSessionUpdate()
        {
            _sdk.RequestSessionInfoUpdate();

            return GetRaceData();
        }

        public TelemetryOutput GetTelemetry()
            => _telemetryOutput;

        public RaceDataOutput GetRaceData()
            => _sessionOutput;

        private void ExecuteOnDisconnectedEvent(object? sender, EventArgs e)
        {
            _telemetryOutput = null;
            _sessionOutput = null;
        }
        
        private async Task ReadRaceData(SessionInfo sessionInfo)
        {
            if (_sessionOutput == null)
            {
                _sessionOutput = new RaceDataOutput();
            }

            await ParseWeekendInfo(sessionInfo["WeekendInfo"]);
            await ParseSessions(sessionInfo["SessionInfo"]["Sessions"]);
            await ParsePlayer(sessionInfo["DriverInfo"]);
            await ParseRacers(sessionInfo["DriverInfo"]["Drivers"]);
        }

        private Task ParsePlayer(YamlQuery yamlQuery)
        {
            int driverCarIdx = int.Parse(yamlQuery["DriverCarIdx"].Value);
            YamlQuery driverQuery = yamlQuery["Drivers"]["CarIdx", driverCarIdx];

            _sessionOutput.Driver = new PlayerRacer(yamlQuery, driverQuery);

            return Task.CompletedTask;
        }

        private Task ParseSessions(YamlQuery yamlQuery)
        {
            var sessionList = new List<Session>();

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    var query = yamlQuery["SessionNum", i];

                    var session = new Session(query);

                    sessionList.Add(session);

                    if (session.SessionName == "RACE"
                        || session.SessionName == "TESTING")
                    {
                        break;
                    }
                }
                catch
                {
                    break;
                }                
            }

            _sessionOutput.Sessions = sessionList;

            return Task.CompletedTask;
        }

        private Task ParseWeekendInfo(YamlQuery yamlQuery)
        {
            _sessionOutput.WeekendData = new WeekendData(yamlQuery);

            return Task.CompletedTask;
        }

        private Task ReadTelemetry()
        {
            _telemetryOutput = new TelemetryOutput(_sdk.Sdk);

            return Task.CompletedTask;
        }

        public void HookUpToTelemetryEvent(EventHandler<SdkWrapper.TelemetryUpdatedEventArgs> action)
        {
            _sdk.TelemetryUpdated += action;
        }

        private async void TelemetryUpdatedEvent(object? sender, SdkWrapper.TelemetryUpdatedEventArgs telemetryEvent)
        {
            await ReadTelemetry();
        }

        private async void SessionUpdatedEvent(object? sender, SdkWrapper.SessionInfoUpdatedEventArgs sessionEvent)
        {
            await ReadRaceData(sessionEvent.SessionInfo);
        }        

        private Task ParseRacers(YamlQuery yaml)
        {
            Dictionary<int, Racer> racers = new();

            //_sessionOutput.WeekendData.MaxDrivers;
            //Does not retrieve the maximum drivers for session
            //I guess it's for team size?

            int maxDrivers = 50;

            for (int i = 0; i < maxDrivers; i++)
            {
                try
                {
                    YamlQuery? driverQuery = yaml["CarIdx", i];

                    var racer = new Racer(driverQuery);

                    racers.Add(racer.UserID, racer);
                }
                catch
                {
                    break;
                }
            }

            _sessionOutput.Racers = racers;

            return Task.CompletedTask;
        }        

        public Racer GetPlayer()
            => _sessionOutput.Driver;        
    }
}
