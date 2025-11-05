using iRacingSdkWrapper.JsonModels;

namespace Core.Models
{
    public class PlayerDTO
    {
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

        public double DriverCarFuelKgPerLtr { get; set; }

        public double DriverCarFuelMaxLtr { get; set; }

        public double DriverCarMaxFuelPct { get; set; }

        public int DriverCarGearNumForward { get; set; }

        public int DriverCarGearNeutral { get; set; }

        public int DriverCarGearReverse { get; set; }

        public float DriverCarSLFirstRPM { get; set; }

        public float DriverCarSLShiftRPM { get; set; }

        public float DriverCarSLLastRPM { get; set; }

        public float DriverCarSLBlinkRPM { get; set; }

        public string DriverCarVersion { get; set; }

        public float? DriverPitTrkPct { get; set; }

        public float DriverCarEstLapTime { get; set; }

        public string DriverSetupName { get; set; }

        public int DriverSetupIsModified { get; set; }

        public string DriverSetupLoadTypeName { get; set; }

        public int DriverSetupPassedTech { get; set; }

        public int DriverIncidentCount { get; set; }

        // public List<Racer> Drivers { get; set; }

        public PlayerDTO(PlayerRacer player)
        {
            DriverCarIdx = player.DriverCarIdx;
            DriverUserID = player.DriverUserID;
            PaceCarIdx = player.PaceCarIdx;
            DriverHeadPosX = player.DriverHeadPosX;
            DriverHeadPosY = player.DriverHeadPosY;
            DriverHeadPosZ = player.DriverHeadPosZ;
            DriverCarIsElectric = player.DriverCarIsElectric;
            DriverCarIdleRPM = player.DriverCarIdleRPM;
            DriverCarRedLine = player.DriverCarRedLine;
            DriverCarEngCylinderCount = player.DriverCarEngCylinderCount;
            DriverCarFuelKgPerLtr = player.DriverCarFuelKgPerLtr;
            DriverCarFuelMaxLtr = player.DriverCarFuelMaxLtr;
            DriverCarMaxFuelPct = player.DriverCarMaxFuelPct;
            DriverCarGearNumForward = player.DriverCarGearNumForward;
            DriverCarGearNeutral = player.DriverCarGearNeutral;
            DriverCarGearReverse = player.DriverCarGearReverse;
            DriverCarSLFirstRPM = player.DriverCarSLFirstRPM;
            DriverCarSLShiftRPM = player.DriverCarSLShiftRPM;
            DriverCarSLLastRPM = player.DriverCarSLLastRPM;
            DriverCarSLBlinkRPM = player.DriverCarSLBlinkRPM;
            DriverCarVersion = player.DriverCarVersion;
            DriverPitTrkPct = player.DriverPitTrkPct;
            DriverCarEstLapTime = player.DriverCarEstLapTime;
            DriverSetupName = player.DriverSetupName;
            DriverSetupIsModified = player.DriverSetupIsModified;
            DriverSetupLoadTypeName = player.DriverSetupLoadTypeName;
            DriverSetupPassedTech = player.DriverSetupPassedTech;
            DriverIncidentCount = player.DriverIncidentCount;
        }

        public PlayerDTO()
        {
        }
    }
}


