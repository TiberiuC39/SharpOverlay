using iRacingSdkWrapper;
using SharpOverlay.Utilities;
using System;

namespace SharpOverlay.Models
{
    public class WeekendOptions
    {
        public WeekendOptions(YamlQuery query)
        {
            ParseWeekendOptions(query);
        }

        public int NumStarters { get; private set; }
        public string StartingGrid { get; private set; }
        public string QualifyScoring { get; private set; }
        public string CourseCautions { get; private set; }
        public int StandingStart { get; private set; }
        public int ShortParadeLap { get; private set; }
        public string Restarts { get; private set; }
        public string WeatherType { get; private set; }
        public string Skies { get; private set; }
        public string WindDirection { get; private set; }
        public double WindSpeed { get; private set; }
        public double WeatherTemp { get; private set; }
        public double RelativeHumidity { get; private set; }
        public double FogLevel { get; private set; }
        public string TimeOfDay { get; private set; }
        public string Date { get; private set; }
        public int EarthRotationSpeedupFactor { get; private set; }
        public int Unofficial { get; private set; }
        public string CommercialMode { get; private set; }
        public string NightMode { get; private set; }
        public int IsFixedSetup { get; private set; }
        public string StrictLapsChecking { get; private set; }
        public int HasOpenRegistration { get; private set; }
        public int HardcoreLevel { get; private set; }
        public int NumJokerLaps { get; private set; }
        public string IncidentLimit { get; private set; }
        public string FastRepairsLimit { get; private set; }
        public int GreenWhiteCheckeredLimit { get; private set; }

        private void ParseWeekendOptions(YamlQuery query)
        {
            NumStarters = int.Parse(query[nameof(NumStarters)].Value);
            StartingGrid = query[nameof(StartingGrid)].Value;
            QualifyScoring = query[nameof(QualifyScoring)].Value;
            CourseCautions = query[nameof(CourseCautions)].Value;
            StandingStart = int.Parse(query[nameof(StandingStart)].Value);
            ShortParadeLap = int.Parse(query[nameof(ShortParadeLap)].Value);
            Restarts = query[nameof(Restarts)].Value;
            WeatherType = query[nameof(WeatherType)].Value;
            Skies = query[nameof(Skies)].Value;
            WindDirection = query[nameof(WindDirection)].Value;
            WindSpeed = double.Parse(StringCleaner.ExtractNumbers(query[nameof(WindSpeed)].Value));
            WeatherTemp = double.Parse(StringCleaner.ExtractNumbers(query[nameof(WeatherTemp)].Value));
            RelativeHumidity = double.Parse(StringCleaner.ExtractNumbers(query[nameof(RelativeHumidity)].Value));
            FogLevel = double.Parse(StringCleaner.ExtractNumbers(query[nameof(FogLevel)].Value));
            TimeOfDay = query[nameof(TimeOfDay)].Value;
            Date = query[nameof(Date)].Value;
            EarthRotationSpeedupFactor = int.Parse(query[nameof(EarthRotationSpeedupFactor)].Value);
            Unofficial = int.Parse(query[nameof(Unofficial)].Value);
            CommercialMode = query[nameof(CommercialMode)].Value;
            NightMode = query[nameof(NightMode)].Value;
            IsFixedSetup = int.Parse(query[nameof(IsFixedSetup)].Value);
            StrictLapsChecking = query[nameof(StrictLapsChecking)].Value;
            HasOpenRegistration = int.Parse(query[nameof(HasOpenRegistration)].Value);
            HardcoreLevel = int.Parse(query[nameof(HardcoreLevel)].Value);
            NumJokerLaps = int.Parse(query[nameof(NumJokerLaps)].Value);
            IncidentLimit = query[nameof(IncidentLimit)].Value;
            FastRepairsLimit = query[nameof(FastRepairsLimit)].Value;
            GreenWhiteCheckeredLimit = int.Parse(query[nameof(GreenWhiteCheckeredLimit)].Value);
        }
    }
}