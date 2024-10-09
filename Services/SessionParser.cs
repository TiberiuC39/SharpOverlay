using iRacingSdkWrapper;
using iRacingSdkWrapper.JsonModels;
using SharpOverlay.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SharpOverlay.Services
{
    public class SessionParser
    {
        public List<Session> Sessions { get; private set; } = [];
        public Dictionary<int, Racer> Drivers { get; private set; } = [];
        public StartType StartType { get; private set; }
        public SessionType SessionType { get; private set; }
        public int SessionLaps { get; private set; }

        public void ParseDrivers(SessionInfo sessionInfo)
        {
            var drivers = sessionInfo.Drivers.ToDictionary(d => d.CarIdx, d => d);

            Drivers = drivers;
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

            if (int.TryParse(currentSessionLaps, out int lapsInCurrentSession))
            {
                SessionLaps = lapsInCurrentSession;
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
            SessionLaps = 0;
            SessionType = SessionType.Practice;
            StartType = StartType.Unknown;
        }
    }
}
