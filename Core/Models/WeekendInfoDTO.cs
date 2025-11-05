using iRacingSdkWrapper.JsonModels;

namespace Core.Models
{
    public class WeekendInfoDTO
    {
        public string TrackName { get; set; }

        public int TrackID { get; set; }

        public double TrackLength { get; set; }

        public double TrackLengthOfficial { get; set; }

        public string TrackDisplayName { get; set; }

        public string TrackDisplayShortName { get; set; }

        public string TrackConfigName { get; set; }

        public string TrackCity { get; set; }

        public string TrackCountry { get; set; }

        public double TrackAltitude { get; set; }

        public double TrackLatitude { get; set; }

        public double TrackLongitude { get; set; }

        public double TrackNorthOffset { get; set; }

        public int TrackNumTurns { get; set; }

        public double TrackPitSpeedLimit { get; set; }

        public string TrackType { get; set; }

        public string TrackDirection { get; set; }

        public string TrackWeatherType { get; set; }

        public string TrackSkies { get; set; }

        public double TrackSurfaceTemp { get; set; }

        public double TrackAirTemp { get; set; }

        public double TrackAirPressure { get; set; }

        public double TrackWindVel { get; set; }

        public double TrackWindDir { get; set; }

        public double TrackRelativeHumidity { get; set; }

        public double TrackFogLevel { get; set; }

        public double TrackPrecipitation { get; set; }

        public double TrackCleanup { get; set; }

        public int TrackDynamicTrack { get; set; }

        public string TrackVersion { get; set; }

        public int SeriesID { get; set; }

        public int SeasonID { get; set; }

        public int SessionID { get; set; }

        public int SubSessionID { get; set; }

        public int LeagueID { get; set; }

        public int Official { get; set; }

        public int RaceWeek { get; set; }

        public string EventType { get; set; }

        public string Category { get; set; }

        public string SimMode { get; set; }

        public int TeamRacing { get; set; }

        public int MinDrivers { get; set; }

        public int MaxDrivers { get; set; }

        public string DCRuleSet { get; set; }

        public int QualifierMustStartRace { get; set; }

        public int NumCarClasses { get; set; }

        public int NumCarTypes { get; set; }

        public int HeatRacing { get; set; }

        public WeekendOptionsDTO WeekendOptions { get; set; } = new WeekendOptionsDTO();

        public WeekendInfoDTO(WeekendInfo weekendInfo)
        {
            TrackName = weekendInfo.TrackName;
            TrackID = weekendInfo.TrackID;
            TrackLength = weekendInfo.TrackLength;
            TrackLengthOfficial = weekendInfo.TrackLengthOfficial;
            TrackDisplayName = weekendInfo.TrackDisplayName;
            TrackDisplayShortName = weekendInfo.TrackDisplayShortName;
            TrackConfigName = weekendInfo.TrackConfigName;
            TrackCity = weekendInfo.TrackCity;
            TrackCountry = weekendInfo.TrackCountry;
            TrackAltitude = weekendInfo.TrackAltitude;
            TrackLatitude = weekendInfo.TrackLatitude;
            TrackLongitude = weekendInfo.TrackLongitude;
            TrackNorthOffset = weekendInfo.TrackNorthOffset;
            TrackNumTurns = weekendInfo.TrackNumTurns;
            TrackPitSpeedLimit = weekendInfo.TrackPitSpeedLimit;
            TrackType = weekendInfo.TrackType;
            TrackDirection = weekendInfo.TrackDirection;
            TrackWeatherType = weekendInfo.TrackWeatherType;
            TrackSkies = weekendInfo.TrackSkies;
            TrackSurfaceTemp = weekendInfo.TrackSurfaceTemp;
            TrackAirTemp = weekendInfo.TrackAirTemp;
            TrackAirPressure = weekendInfo.TrackAirPressure;
            TrackWindVel = weekendInfo.TrackWindVel;
            TrackWindDir = weekendInfo.TrackWindDir;
            TrackRelativeHumidity = weekendInfo.TrackRelativeHumidity;
            TrackFogLevel = weekendInfo.TrackFogLevel;
            TrackPrecipitation = weekendInfo.TrackPrecipitation;
            TrackCleanup = weekendInfo.TrackCleanup;
            TrackDynamicTrack = weekendInfo.TrackDynamicTrack;
            TrackVersion = weekendInfo.TrackVersion;
            SeriesID = weekendInfo.SeriesID;
            SeasonID = weekendInfo.SeasonID;
            SessionID = weekendInfo.SessionID;
            SubSessionID = weekendInfo.SubSessionID;
            LeagueID = weekendInfo.LeagueID;
            Official = weekendInfo.Official;
            RaceWeek = weekendInfo.RaceWeek;
            EventType = weekendInfo.EventType;
            Category = weekendInfo.Category;
            SimMode = weekendInfo.SimMode;
            TeamRacing = weekendInfo.TeamRacing;
            MinDrivers = weekendInfo.MinDrivers;
            MaxDrivers = weekendInfo.MaxDrivers;
            DCRuleSet = weekendInfo.DCRuleSet;
            QualifierMustStartRace = weekendInfo.QualifierMustStartRace;
            NumCarClasses = weekendInfo.NumCarClasses;
            NumCarTypes = weekendInfo.NumCarTypes;
            HeatRacing = weekendInfo.HeatRacing;

            if (weekendInfo.WeekendOptions is not null)
            {
                WeekendOptions = new WeekendOptionsDTO(weekendInfo.WeekendOptions);
            }
        }

        public WeekendInfoDTO()
        {
            WeekendOptions = new WeekendOptionsDTO();
        }

        public class WeekendOptionsDTO
        {
            public int NumStarters { get; set; }

            public string StartingGrid { get; set; }

            public string QualifyScoring { get; set; }

            public string CourseCautions { get; set; }

            public int StandingStart { get; set; }

            public int ShortParadeLap { get; set; }

            public string Restarts { get; set; }

            public string WeatherType { get; set; }

            public string Skies { get; set; }

            public string WindDirection { get; set; }

            public double WindSpeed { get; set; }

            public double WeatherTemp { get; set; }

            public double RelativeHumidity { get; set; }

            public double FogLevel { get; set; }

            public string TimeOfDay { get; set; }

            public string Date { get; set; }

            public int EarthRotationSpeedupFactor { get; set; }

            public int Unofficial { get; set; }

            public string CommercialMode { get; set; }

            public string NightMode { get; set; }

            public int IsFixedSetup { get; set; }

            public string StrictLapsChecking { get; set; }

            public int HasOpenRegistration { get; set; }

            public int HardcoreLevel { get; set; }

            public int NumJokerLaps { get; set; }

            public string IncidentLimit { get; set; }

            public string FastRepairsLimit { get; set; }

            public int GreenWhiteCheckeredLimit { get; set; }

            public WeekendOptionsDTO(WeekendOptions weekendOptions)
            {
                NumStarters = weekendOptions.NumStarters;
                StartingGrid = weekendOptions.StartingGrid;
                QualifyScoring = weekendOptions.QualifyScoring;
                CourseCautions = weekendOptions.CourseCautions;
                StandingStart = weekendOptions.StandingStart;
                ShortParadeLap = weekendOptions.ShortParadeLap;
                Restarts = weekendOptions.Restarts;
                WeatherType = weekendOptions.WeatherType;
                Skies = weekendOptions.Skies;
                WindDirection = weekendOptions.WindDirection;
                WindSpeed = weekendOptions.WindSpeed;
                WeatherTemp = weekendOptions.WeatherTemp;
                RelativeHumidity = weekendOptions.RelativeHumidity;
                FogLevel = weekendOptions.FogLevel;
                TimeOfDay = weekendOptions.TimeOfDay;
                Date = weekendOptions.Date;
                EarthRotationSpeedupFactor = weekendOptions.EarthRotationSpeedupFactor;
                Unofficial = weekendOptions.Unofficial;
                CommercialMode = weekendOptions.CommercialMode;
                NightMode = weekendOptions.NightMode;
                IsFixedSetup = weekendOptions.IsFixedSetup;
                StrictLapsChecking = weekendOptions.StrictLapsChecking;
                HasOpenRegistration = weekendOptions.HasOpenRegistration;
                HardcoreLevel = weekendOptions.HardcoreLevel;
                NumJokerLaps = weekendOptions.NumJokerLaps;
                IncidentLimit = weekendOptions.IncidentLimit;
                FastRepairsLimit = weekendOptions.FastRepairsLimit;
                GreenWhiteCheckeredLimit = weekendOptions.GreenWhiteCheckeredLimit;
            }

            public WeekendOptionsDTO()
            {
            }
        }

    }
}

