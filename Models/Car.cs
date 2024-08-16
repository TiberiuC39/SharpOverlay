using iRacingSdkWrapper;
using NuGet.Versioning;
using System.Text.RegularExpressions;

namespace SharpOverlay.Models
{
    public class Car
    {
        public FuelTank _fuel;
        private TrackTelemetry _track;

        public Car(TelemetryOutput telemetryOutput, YamlQuery query, int playerCarIdx)
        {
            var fuelLevel = telemetryOutput.FuelLevel.Value;
            var fuelPct = telemetryOutput.FuelLevelPct.Value;

            float defaultFuelCapacity = float.Parse(query["DriverCarFuelMaxLtr"].Value);
            float fuelRestriction = float.Parse(query["DriverCarMaxFuelPct"].Value);

            float allowedFuelCapacity = defaultFuelCapacity * fuelRestriction;

            _fuel = new FuelTank(fuelLevel, fuelPct, allowedFuelCapacity);

            ParseCarSpecification(query["Drivers"]["CarIdx", playerCarIdx]);

            UpdateTelemetry(telemetryOutput);
        }

        public int CarIdx { get; private set; }
        public string CarNumber { get; private set; }
        public int CarNumberRaw { get; private set; }
        public string CarPath { get; private set; }
        public int CarClassID { get; private set; }
        public int CarID { get; private set; }
        public int CarIsPaceCar { get; private set; }
        public int CarIsAI { get; private set; }
        public int CarIsElectric { get; private set; }
        public string CarScreenName { get; private set; }
        public string CarScreenNameShort { get; private set; }
        public string CarClassShortName { get; private set; }
        public int CarClassRelSpeed { get; private set; }
        public int CarClassLicenseLevel { get; private set; }
        public double CarClassMaxFuelPct { get; private set; }
        public double CarClassWeightPenalty { get; private set; }
        public double CarClassPowerAdjust { get; private set; }
        public double CarClassDryTireSetLimit { get; private set; }
        public string CarClassColor { get; private set; }
        public double CarClassEstLapTime { get; private set; }
        public bool IsPitstopActive { get; private set; }
        public float FuelConsumptionPerHour { get; private set; }

        public void UpdateTelemetry(TelemetryOutput telemetryOutput)
        {
            var fuelLevel = telemetryOutput.FuelLevel.Value;
            var fuelPct = telemetryOutput.FuelLevelPct.Value;

            IsPitstopActive = telemetryOutput.IsPitstopActive.Value;
            FuelConsumptionPerHour = telemetryOutput.FuelUsePerHour.Value;

            _fuel.UpdateFuel(fuelLevel, fuelPct);
        }

        private void ParseCarSpecification(YamlQuery query)
        {
            CarNumber = query[nameof(CarNumber)].Value;
            CarNumberRaw = int.Parse(query[nameof(CarNumberRaw)].Value);
            CarPath = query[nameof(CarPath)].Value;
            CarClassID = int.Parse(query[nameof(CarClassID)].Value);
            CarID = int.Parse(query[nameof(CarID)].Value    );
            CarIsPaceCar = int.Parse(query[nameof(CarIsPaceCar)].Value);
            CarIsAI = int.Parse(query[nameof(CarIsAI)].Value);
            CarIsElectric = int.Parse(query[nameof(CarIsElectric)].Value);
            CarScreenName = query[nameof(CarScreenName)].Value;
            CarScreenNameShort = query[nameof(CarScreenNameShort)].Value;
            CarClassShortName = query[nameof(CarClassShortName)].Value;
            CarClassRelSpeed = int.Parse(query[nameof(CarClassRelSpeed)].Value);
            CarClassLicenseLevel = int.Parse(query[nameof(CarClassLicenseLevel)].Value);
            CarClassMaxFuelPct = ParseCarClassRestriction(query[nameof(CarClassMaxFuelPct)].Value);
            CarClassWeightPenalty = ParseCarClassRestriction(query[nameof(CarClassWeightPenalty)].Value);
            CarClassPowerAdjust = ParseCarClassRestriction(query[nameof(CarClassPowerAdjust)].Value);
            CarClassDryTireSetLimit = ParseCarClassRestriction(query[nameof(CarClassDryTireSetLimit)].Value);
            CarClassColor = query[nameof(CarClassColor)].Value;
            CarClassEstLapTime = double.Parse(query[nameof(CarClassEstLapTime)].Value);
        }

        private double ParseCarClassRestriction(string value)
        {
            string cleanValue = Regex.Replace(value, @"[^\d.,-]", string.Empty);

            return double.Parse(cleanValue);
        }
    }
}
