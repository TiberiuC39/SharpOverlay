using iRacingSdkWrapper;
using iRacingSdkWrapper.JsonModels;
using SharpOverlay.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SharpOverlay.Utilities.Sessions
{
    public class SessionParser : ISessionParser
    {
        public List<Session> Sessions { get; private set; } = [];
        public Dictionary<int, Racer> Drivers { get; private set; } = [];
        public StartType StartType { get; private set; }
        public SessionType SessionType { get; private set; }
        public int SessionLaps { get; private set; }
        public int PaceCarIdx { get; private set; }
        public bool IsMultiClassRace { get; private set; }
        public List<Sector> Sectors { get; private set; }
        public int TrackId { get; private set; }
        public int CarId { get; private set; }

        public bool IsSetupChanged { get; private set; }

        public void ParseSectors(SessionInfo sessionInfo)
        {
            Sectors = sessionInfo.Sectors;
        }

        public void ParseDrivers(SessionInfo sessionInfo)
        {
            var drivers = sessionInfo.Drivers.ToDictionary(d => d.CarIdx, d => d);

            Drivers = drivers;
        }

        public void ParsePaceCarIdx(SessionInfo sessionInfo)
        {
            PaceCarIdx = sessionInfo.Player.PaceCarIdx;
        }

        public void ParseSessions(SessionInfo sessionInfo)
        {
            //SaveSessionInfoToJsonString(sessionInfo);

            Sessions = sessionInfo.Sessions;
        }


        public TimeSpan GetBestLapTime(int carIdx, int currentSessionNumber = default)
        {
            if (Sessions.Count > 0)
            {
                var currentSession = Sessions[currentSessionNumber];

                if (currentSession.ResultsPositions != null)
                {
                    var resultEntry = currentSession.ResultsPositions.Where(r => r.CarIdx == carIdx).FirstOrDefault();

                    if (resultEntry != null)
                    {
                        var bestTime = TimeSpan.FromSeconds(resultEntry.FastestTime);

                        return bestTime;
                    }
                }
            }

            return TimeSpan.FromSeconds(-1);
        }

        public void ParseStartType(SessionInfo sessionInfo)
        {
            int standingStartValue = sessionInfo.WeekendInfo.WeekendOptions.StandingStart;

            switch (standingStartValue)
            {
                case 0:
                    StartType = StartType.Rolling;
                    break;
                case 1:
                    StartType = StartType.Standing;
                    break;
                default:
                    StartType = StartType.Unknown;
                    break;
            }
        }

        public void ParseCurrentSessionType(SessionInfo sessionInfo, int currentSessionNumber = default)
        {
            string session = sessionInfo.Sessions[currentSessionNumber].SessionType;

            if (Enum.TryParse(session, out SessionType sessionType))
            {
                SessionType = sessionType;
            }
        }

        public void ParseLapsInSession(SessionInfo sessionInfo, int currentSessionNumber = default)
        {
            var currentSession = sessionInfo.Sessions[currentSessionNumber];

            string currentSessionLaps = currentSession.SessionLaps;

            const int unlimitedLaps = -1;

            if (int.TryParse(currentSessionLaps, out int lapsInCurrentSession))
            {
                SessionLaps = lapsInCurrentSession;
            }
            else
            {
                SessionLaps = unlimitedLaps;
            }
        }

        private void SaveSessionInfoToJsonString(SessionInfo sessionInfo)
        {
            var jsonString = JsonSerializer.Serialize(sessionInfo, new JsonSerializerOptions()
            {
                WriteIndented = true
            });

            File.WriteAllText($"../../../{sessionInfo.WeekendInfo.TrackName + sessionInfo.WeekendInfo.WeekendOptions.Date}.txt", jsonString);
        }

        public void Clear()
        {
            Sessions.Clear();
            Drivers.Clear();
            SessionLaps = -1;
            SessionType = SessionType.Invalid;
            StartType = StartType.Unknown;
        }

        public void ParseRaceType(SessionInfo sessionInfo)
        {
            IsMultiClassRace = sessionInfo.WeekendInfo.NumCarClasses > 1;
        }

        public void ParseTrackId(SessionInfo sessionInfo)
        {
            TrackId = sessionInfo.WeekendInfo.TrackID;
        }

        public void ParseCarId(SessionInfo sessionInfo)
        {
            var driver = sessionInfo.Drivers.FirstOrDefault(d => d.CarIdx == sessionInfo.Player.DriverCarIdx);

            if (driver != null)
            {
                CarId = driver.CarID;
            }
        }
    }
}
