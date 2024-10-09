using iRacingSdkWrapper;
using iRacingSdkWrapper.Bitfields;

namespace SharpOverlay.Models
{
    public class TelemetryModel
    {
        public float MGUKDeployAdapt { get; set; }
        public float MGUKDeployFixed { get; set; }
        public float MGUKRegenGain { get; set; }
        public float EnergyBatteryToMGU { get; set; }
        public float EnergyBudgetBattToMGU { get; set; }
        public float EnergyERSBattery { get; set; }
        public float PowerMGUH { get; set; }
        public float PowerMGUK { get; set; }
        public float TorqueMGUK { get; set; }
        public int DrsStatus { get; set; }
        public int LapCompleted { get; set; }
        public double SessionTime { get; set; }
        public int SessionNum { get; set; }
        public SessionStates SessionState { get; set; }
        public int SessionUniqueID { get; set; }
        public SessionFlag SessionFlags { get; set; }
        public bool DriverMarker { get; set; }
        public bool IsReplayPlaying { get; set; }
        public int ReplayFrameNum { get; set; }
        public int[] CarIdxLap { get; set; }
        public int[] CarIdxLapCompleted { get; set; }
        public float[] CarIdxLapDistPct { get; set; }
        public TrackSurfaces[] CarIdxTrackSurface { get; set; }
        public float[] CarIdxSteer { get; set; }
        public float[] CarIdxRPM { get; set; }
        public int[] CarIdxGear { get; set; }
        public float[] CarIdxF2Time { get; set; }
        public float[] CarIdxEstTime { get; set; }
        public bool[] CarIdxOnPitRoad { get; set; }
        public int[] CarIdxPosition { get; set; }
        public int[] CarIdxClassPosition { get; set; }
        public float SteeringWheelAngle { get; set; }
        public float Throttle { get; set; }
        public float Brake { get; set; }
        public float Clutch { get; set; }
        public int Gear { get; set; }
        public float RPM { get; set; }
        public int Lap { get; set; }
        public float LapDist { get; set; }
        public float LapDistPct { get; set; }
        public int RaceLaps { get; set; }
        public float LongAccel { get; set; }
        public float LatAccel { get; set; }
        public float VertAccel { get; set; }
        public float RollRate { get; set; }
        public float PitchRate { get; set; }
        public float YawRate { get; set; }
        public float Speed { get; set; }
        public float VelocityX { get; set; }
        public float VelocityY { get; set; }
        public float VelocityZ { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public int CamCarIdx { get; set; }
        public int CamCameraNumber { get; set; }
        public int CamGroupNumber { get; set; }
        public CameraState CamCameraState { get; set; }
        public bool IsOnTrack { get; set; }
        public bool IsInGarage { get; set; }
        public float SteeringWheelTorque { get; set; }
        public float SteeringWheelPctTorque { get; set; }
        public float ShiftIndicatorPct { get; set; }
        public EngineWarning EngineWarnings { get; set; }
        public float FuelLevel { get; set; }
        public float FuelLevelPct { get; set; }
        public int ReplayPlaySpeed { get; set; }
        public bool ReplayPlaySlowMotion { get; set; }
        public double ReplaySessionTime { get; set; }
        public int ReplaySessionNum { get; set; }
        public float WaterTemp { get; set; }
        public float WaterLevel { get; set; }
        public float FuelPress { get; set; }
        public float OilTemp { get; set; }
        public float OilPress { get; set; }
        public float OilLevel { get; set; }
        public float Voltage { get; set; }
        public double SessionTimeRemain { get; set; }
        public int ReplayFrameNumEnd { get; set; }
        public float AirDensity { get; set; }
        public dynamic WeatherWindVel { get; private set; }
        public dynamic WeatherWindDir { get; private set; }
        public dynamic WeatherRelativeHumidity { get; private set; }
        public float AirPressure { get; set; }
        public float AirTemp { get; set; }
        public float FogLevel { get; set; }
        public int Skies { get; set; }
        public float TrackTemp { get; set; }
        public float TrackTempCrew { get; set; }
        public float RelativeHumidity { get; set; }
        public int WeatherType { get; set; }
        public float WindDir { get; set; }
        public float WindVel { get; set; }
        public int PlayerCarTeamIncidentCount { get; set; }
        public int PlayerCarMyIncidentCount { get; set; }
        public int PlayerCarDriverIncidentCount { get; set; }
        public TrackSurfaces PlayerTrackSurface { get; set; }
        public int PlayerCarIdx { get; set; }
        public int SessionLapsRemainingEx { get; set; }
        public int SessionLapsRemaining { get; set; }
        public float LapLastLapTime { get; set; }
        public float LapLastNLapTime { get; set; }
        public bool IsOnPitRoad { get; set; }
        public bool IsPitstopActive { get; set; }
        public double PitOptRepairLeft { get; set; }
        public double PitRepairLeft { get; set; }
        public float FuelUsePerHour { get; set; }
        public bool WeatherDeclaredWet { get; set; }
        public int TrackWetness { get; set; }
        public CarLeftRight CarLeftRight { get; set; }
        public int EnterExitReset { get; set; }
        public float[] CarIdxLastLapTime { get; set; }
        public double[] CarIdxBestLapTime { get; set; }
        public double[] CarClassEstLapTime { get; set; }
        public int[] CarIdxClass { get; set; }
    }
}
