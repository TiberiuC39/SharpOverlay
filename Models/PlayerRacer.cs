using iRacingSdkWrapper;

namespace SharpOverlay.Models
{
    public class PlayerRacer : Racer
    {
        public PlayerRacer(YamlQuery playerInfo, YamlQuery racerInfo)
            : base(racerInfo)
        {
            ParseYaml(playerInfo);
        }        

        public int DriverCarIdx { get; set; }
        public int DriverUserID { get; set; }
        public int PaceCarIdx { get; set; }
        public float DriverHeadPosX { get; set; }
        public float DriverHeadPosY { get; set; }
        public float DriverHeadPosZ { get; set; }
        public int DriverCarIsElectric { get; set; }
        public float DriverCarIdleRPM { get; set; }
        public float DriverCarRedLine { get; set; }
        public int DriverCarEngCylinderCount { get; set; }
        public float DriverCarFuelKgPerLtr { get; set; }
        public float DriverCarFuelMaxLtr { get; set; }
        public float DriverCarMaxFuelPct { get; set; }
        public int DriverCarGearNumForward { get; set; }
        public int DriverCarGearNeutral { get; set; }
        public int DriverCarGearReverse { get; set; }
        public float DriverCarSLFirstRPM { get; set; }
        public float DriverCarSLShiftRPM { get; set; }
        public float DriverCarSLLastRPM { get; set; }
        public float DriverCarSLBlinkRPM { get; set; }
        public string DriverCarVersion { get; set; }
        public float DriverPitTrkPct { get; set; }
        public float DriverCarEstLapTime { get; set; }
        public string DriverSetupName { get; set; }
        public int DriverSetupIsModified { get; set; }
        public string DriverSetupLoadTypeName { get; set; }
        public int DriverSetupPassedTech { get; set; }
        public int DriverIncidentCount { get; set; }

        private void ParseYaml(YamlQuery yaml)
        {
            DriverCarIdx = int.Parse(yaml[nameof(DriverCarIdx)].Value);
            DriverUserID = int.Parse(yaml[nameof(DriverUserID)].Value);
            PaceCarIdx = int.Parse(yaml[nameof(PaceCarIdx)].Value);
            DriverHeadPosX = float.Parse(yaml[nameof(DriverHeadPosX)].Value);
            DriverHeadPosY = float.Parse(yaml[nameof(DriverHeadPosY)].Value);
            DriverHeadPosZ = float.Parse(yaml[nameof(DriverHeadPosZ)].Value);
            DriverCarIsElectric = int.Parse(yaml[nameof(DriverCarIsElectric)].Value);
            DriverCarIdleRPM = float.Parse(yaml[nameof(DriverCarIdleRPM)].Value);
            DriverCarRedLine = float.Parse(yaml[nameof(DriverCarRedLine)].Value);
            DriverCarEngCylinderCount = int.Parse(yaml[nameof(DriverCarEngCylinderCount)].Value);
            DriverCarFuelKgPerLtr = float.Parse(yaml[nameof(DriverCarFuelKgPerLtr)].Value);
            DriverCarFuelMaxLtr = float.Parse(yaml[nameof(DriverCarFuelMaxLtr)].Value);
            DriverCarMaxFuelPct = float.Parse(yaml[nameof(DriverCarMaxFuelPct)].Value);
            DriverCarGearNumForward = int.Parse(yaml[nameof(DriverCarGearNumForward)].Value);
            DriverCarGearNeutral = int.Parse(yaml[nameof(DriverCarGearNeutral)].Value);
            DriverCarGearReverse = int.Parse(yaml[nameof(DriverCarGearReverse)].Value);
            DriverCarSLFirstRPM = float.Parse(yaml[nameof(DriverCarSLFirstRPM)].Value);
            DriverCarSLShiftRPM = float.Parse(yaml[nameof(DriverCarSLShiftRPM)].Value);
            DriverCarSLLastRPM = float.Parse(yaml[nameof(DriverCarSLLastRPM)].Value);
            DriverCarSLBlinkRPM = float.Parse(yaml[nameof(DriverCarSLBlinkRPM)].Value);
            DriverCarVersion = yaml[nameof(DriverCarVersion)].Value;
            //DriverPitTrkPct = float.Parse(yaml[nameof(DriverPitTrkPct)].Value ?? "0");                Shows only in pit
            DriverCarEstLapTime = float.Parse(yaml[nameof(DriverCarEstLapTime)].Value);
            DriverSetupName = yaml[nameof(DriverSetupName)].Value;
            DriverSetupIsModified = int.Parse(yaml[nameof(DriverSetupIsModified)].Value);
            DriverSetupLoadTypeName = yaml[nameof(DriverSetupLoadTypeName)].Value;
            DriverSetupPassedTech = int.Parse(yaml[nameof(DriverSetupPassedTech)].Value);
            DriverIncidentCount = int.Parse(yaml[nameof(DriverIncidentCount)].Value);
        }
    }
}
