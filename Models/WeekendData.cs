using iRacingSdkWrapper;
using SharpOverlay.Utilities;

namespace SharpOverlay.Models
{
    public class WeekendData
    {
        public WeekendData(YamlQuery query)
        {
            ParseWeekendData(query);
            ParseWeekendOptions(query["WeekendOptions"]);
        }        

        public string TrackName { get; private set; }
        public int TrackID { get; private set; }
        public double TrackLength { get; private set; }
        public double TrackLengthOfficial { get; private set; }
        public string TrackDisplayName { get; private set; }
        public string TrackDisplayShortName { get; private set; }
        public string TrackConfigName { get; private set; }
        public string TrackCity { get; private set; }
        public string TrackCountry { get; private set; }
        public double TrackAltitude { get; private set; }
        public double TrackLatitude { get; private set; }
        public double TrackLongitude { get; private set; }
        public double TrackNorthOffset { get; private set; }
        public int TrackNumTurns { get; private set; }
        public double TrackPitSpeedLimit { get; private set; }
        public string TrackType { get; private set; }
        public string TrackDirection { get; private set; }
        public string TrackWeatherType { get; private set; }
        public string TrackSkies { get; private set; }
        public double TrackSurfaceTemp { get; private set; }
        public double TrackAirTemp { get; private set; }
        public double TrackAirPressure { get; private set; }
        public double TrackWindVel { get; private set; }
        public double TrackWindDir { get; private set; }
        public double TrackRelativeHumidity { get; private set; }
        public double TrackFogLevel { get; private set; }
        public double TrackPrecipitation { get; private set; }
        public int TrackCleanup { get; private set; }
        public int TrackDynamicTrack { get; private set; }
        public string TrackVersion { get; private set; }
        public int SeriesID { get; private set; }
        public int SeasonID { get; private set; }
        public long SessionID { get; private set; }
        public long SubSessionID { get; private set; }
        public int LeagueID { get; private set; }
        public int Official { get; private set; }
        public int RaceWeek { get; private set; }
        public string EventType { get; private set; }
        public string Category { get; private set; }
        public string SimMode { get; private set; }
        public int TeamRacing { get; private set; }
        public int MinDrivers { get; private set; }
        public int MaxDrivers { get; private set; }
        public string DCRuleSet { get; private set; }
        public int QualifierMustStartRace { get; private set; }
        public int NumCarClasses { get; private set; }
        public int NumCarTypes { get; private set; }
        public int HeatRacing { get; private set; }
        public WeekendOptions WeekendOptions { get; private set; }

        private void ParseWeekendData(YamlQuery query)
        {
            TrackName = query[nameof(TrackName)].Value;
            TrackID = int.Parse(query[nameof(TrackID)].Value);
            TrackLength = double.Parse(StringCleaner.ExtractNumbers(query[nameof(TrackLength)].Value));
            TrackLengthOfficial = double.Parse(StringCleaner.ExtractNumbers(query[nameof(TrackLengthOfficial)].Value));
            TrackDisplayName = query[nameof(TrackDisplayName)].Value;
            TrackDisplayShortName = query[nameof(TrackDisplayShortName)].Value;
            TrackConfigName = query[nameof(TrackConfigName)].Value;
            TrackCity = query[nameof(TrackCity)].Value;
            TrackCountry = query[nameof(TrackCountry)].Value;
            TrackAltitude = double.Parse(StringCleaner.ExtractNumbers(query[nameof(TrackAltitude)].Value));
            TrackLatitude = double.Parse(StringCleaner.ExtractNumbers(query[nameof(TrackLatitude)].Value));
            TrackLongitude = double.Parse(StringCleaner.ExtractNumbers(query[nameof(TrackLongitude)].Value));
            TrackNorthOffset = double.Parse(StringCleaner.ExtractNumbers(query[nameof(TrackNorthOffset)].Value));
            TrackNumTurns = int.Parse(query[nameof(TrackNumTurns)].Value);
            TrackPitSpeedLimit = double.Parse(StringCleaner.ExtractNumbers(query[nameof(TrackPitSpeedLimit)].Value));
            TrackType = query[nameof(TrackType)].Value;
            TrackDirection = query[nameof(TrackDirection)].Value;
            TrackWeatherType = query[nameof(TrackWeatherType)].Value;
            TrackSkies = query[nameof(TrackSkies)].Value;
            TrackSurfaceTemp = double.Parse(StringCleaner.ExtractNumbers(query[nameof(TrackSurfaceTemp)].Value));
            TrackAirTemp = double.Parse(StringCleaner.ExtractNumbers(query[nameof(TrackAirTemp)].Value));
            TrackAirPressure = double.Parse(StringCleaner.ExtractNumbers(query[nameof(TrackAirPressure)].Value));
            TrackWindVel = double.Parse(StringCleaner.ExtractNumbers(query[nameof(TrackWindVel)].Value));
            TrackWindDir = double.Parse(StringCleaner.ExtractNumbers(query[nameof(TrackWindDir)].Value));
            TrackRelativeHumidity = double.Parse(StringCleaner.ExtractNumbers(query[nameof(TrackRelativeHumidity)].Value));
            TrackFogLevel = double.Parse(StringCleaner.ExtractNumbers(query[nameof(TrackFogLevel)].Value));
            TrackPrecipitation = double.Parse(StringCleaner.ExtractNumbers(query[nameof(TrackPrecipitation)].Value));
            TrackCleanup = int.Parse(query[nameof(TrackCleanup)].Value);
            TrackDynamicTrack = int.Parse(query[nameof(TrackDynamicTrack)].Value);
            TrackVersion = query[nameof(TrackVersion)].Value;
            SeriesID = int.Parse(query[nameof(SeriesID)].Value);
            SeasonID = int.Parse(query[nameof(SeasonID)].Value);
            SessionID = long.Parse(query[nameof(SessionID)].Value);
            SubSessionID = long.Parse(query[nameof(SubSessionID)].Value);
            LeagueID = int.Parse(query[nameof(LeagueID)].Value);
            Official = int.Parse(query[nameof(Official)].Value);
            RaceWeek = int.Parse(query[nameof(RaceWeek)].Value);
            EventType = query[nameof(EventType)].Value;
            Category = query[nameof(Category)].Value;
            SimMode = query[nameof(SimMode)].Value;
            TeamRacing = int.Parse(query[nameof(TeamRacing)].Value);
            MinDrivers = int.Parse(query[nameof(MinDrivers)].Value);
            MaxDrivers = int.Parse(query[nameof(MaxDrivers)].Value);
            DCRuleSet = query[nameof(DCRuleSet)].Value;
            QualifierMustStartRace = int.Parse(query[nameof(QualifierMustStartRace)].Value);
            NumCarClasses = int.Parse(query[nameof(NumCarClasses)].Value);
            NumCarTypes = int.Parse(query[nameof(NumCarTypes)].Value);
            HeatRacing = int.Parse(query[nameof(HeatRacing)].Value);
        }

        private void ParseWeekendOptions(YamlQuery query)
        {
            WeekendOptions = new WeekendOptions(query);
        }
    }
}
