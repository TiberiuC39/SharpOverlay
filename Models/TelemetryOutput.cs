using iRacingSdkWrapper.Bitfields;
using iRacingSdkWrapper;
using iRSDKSharp;
using System.Collections.Generic;

namespace SharpOverlay.Models
{
    public class TelemetryOutput
    {
        private readonly iRacingSDK sdk;

        public TelemetryValue<float> MGUKDeployAdapt => new TelemetryValue<float>(sdk, "dcMGUKDeployAdapt");

        public TelemetryValue<float> MGUKDeployFixed => new TelemetryValue<float>(sdk, "dcMGUKDeployFixed");

        public TelemetryValue<float> MGUKRegenGain => new TelemetryValue<float>(sdk, "dcMGUKRegenGain");

        public TelemetryValue<float> EnergyBatteryToMGU => new TelemetryValue<float>(sdk, "EnergyBatteryToMGU_KLap");

        public TelemetryValue<float> EnergyBudgetBattToMGU => new TelemetryValue<float>(sdk, "EnergyBudgetBattToMGU_KLap");

        public TelemetryValue<float> EnergyERSBattery => new TelemetryValue<float>(sdk, "EnergyERSBattery");

        public TelemetryValue<float> PowerMGUH => new TelemetryValue<float>(sdk, "PowerMGU_H");

        public TelemetryValue<float> PowerMGUK => new TelemetryValue<float>(sdk, "PowerMGU_K");

        public TelemetryValue<float> TorqueMGUK => new TelemetryValue<float>(sdk, "TorqueMGU_K");

        public TelemetryValue<int> DrsStatus => new TelemetryValue<int>(sdk, "DRS_Status");

        public TelemetryValue<int> LapCompleted => new TelemetryValue<int>(sdk, "LapCompleted");

        public TelemetryValue<double> SessionTime => new TelemetryValue<double>(sdk, "SessionTime");

        public TelemetryValue<int> SessionNum => new TelemetryValue<int>(sdk, "SessionNum");

        public TelemetryValue<SessionStates> SessionState => new TelemetryValue<SessionStates>(sdk, "SessionState");

        public TelemetryValue<int> SessionUniqueID => new TelemetryValue<int>(sdk, "SessionUniqueID");

        public TelemetryValue<SessionFlag> SessionFlags => new TelemetryValue<SessionFlag>(sdk, "SessionFlags");

        public TelemetryValue<double> SessionLapsRemaining
            => new TelemetryValue<double>(sdk, "SessionLapsRemain");

        public TelemetryValue<bool> DriverMarker => new TelemetryValue<bool>(sdk, "DriverMarker");

        public TelemetryValue<bool> IsReplayPlaying => new TelemetryValue<bool>(sdk, "IsReplayPlaying");

        public TelemetryValue<int> ReplayFrameNum => new TelemetryValue<int>(sdk, "ReplayFrameNum");

        public TelemetryValue<int[]> CarIdxLap => new TelemetryValue<int[]>(sdk, "CarIdxLap");

        public TelemetryValue<int[]> CarIdxLapCompleted => new TelemetryValue<int[]>(sdk, "CarIdxLapCompleted");

        public TelemetryValue<float[]> CarIdxLapDistPct => new TelemetryValue<float[]>(sdk, "CarIdxLapDistPct");

        public TelemetryValue<TrackSurfaces[]> CarIdxTrackSurface => new TelemetryValue<TrackSurfaces[]>(sdk, "CarIdxTrackSurface");

        public TelemetryValue<float[]> CarIdxSteer => new TelemetryValue<float[]>(sdk, "CarIdxSteer");

        public TelemetryValue<float[]> CarIdxRPM => new TelemetryValue<float[]>(sdk, "CarIdxRPM");

        public TelemetryValue<int[]> CarIdxGear => new TelemetryValue<int[]>(sdk, "CarIdxGear");

        public TelemetryValue<float[]> CarIdxF2Time => new TelemetryValue<float[]>(sdk, "CarIdxF2Time");

        public TelemetryValue<float[]> CarIdxEstTime => new TelemetryValue<float[]>(sdk, "CarIdxEstTime");

        public TelemetryValue<bool[]> CarIdxOnPitRoad => new TelemetryValue<bool[]>(sdk, "CarIdxOnPitRoad");

        public TelemetryValue<int[]> CarIdxPosition => new TelemetryValue<int[]>(sdk, "CarIdxPosition");

        public TelemetryValue<int[]> CarIdxClassPosition => new TelemetryValue<int[]>(sdk, "CarIdxClassPosition");

        public TelemetryValue<float> SteeringWheelAngle => new TelemetryValue<float>(sdk, "SteeringWheelAngle");

        public TelemetryValue<float> Throttle => new TelemetryValue<float>(sdk, "Throttle");

        public TelemetryValue<float> Brake => new TelemetryValue<float>(sdk, "Brake");

        public TelemetryValue<float> Clutch => new TelemetryValue<float>(sdk, "Clutch");

        public TelemetryValue<int> Gear => new TelemetryValue<int>(sdk, "Gear");

        public TelemetryValue<float> RPM => new TelemetryValue<float>(sdk, "RPM");

        public TelemetryValue<int> Lap => new TelemetryValue<int>(sdk, "Lap");

        public TelemetryValue<float> LapDist => new TelemetryValue<float>(sdk, "LapDist");

        public TelemetryValue<float> LapDistPct => new TelemetryValue<float>(sdk, "LapDistPct");

        public TelemetryValue<int> RaceLaps => new TelemetryValue<int>(sdk, "RaceLaps");

        public TelemetryValue<float> LapLastLapTime
            => new TelemetryValue<float>(sdk, "LapLastLapTime");

        public TelemetryValue<float> LapLastNLapTime
            => new TelemetryValue<float>(sdk, "LapLastNLapTime");

        public TelemetryValue<float> LongAccel => new TelemetryValue<float>(sdk, "LongAccel");

        public TelemetryValue<float> LatAccel => new TelemetryValue<float>(sdk, "LatAccel");

        public TelemetryValue<float> VertAccel => new TelemetryValue<float>(sdk, "VertAccel");

        public TelemetryValue<float> RollRate => new TelemetryValue<float>(sdk, "RollRate");

        public TelemetryValue<float> PitchRate => new TelemetryValue<float>(sdk, "PitchRate");

        public TelemetryValue<float> YawRate => new TelemetryValue<float>(sdk, "YawRate");

        public TelemetryValue<float> Speed => new TelemetryValue<float>(sdk, "Speed");

        public TelemetryValue<float> VelocityX => new TelemetryValue<float>(sdk, "VelocityX");

        public TelemetryValue<float> VelocityY => new TelemetryValue<float>(sdk, "VelocityY");

        public TelemetryValue<float> VelocityZ => new TelemetryValue<float>(sdk, "VelocityZ");

        public TelemetryValue<float> Yaw => new TelemetryValue<float>(sdk, "Yaw");

        public TelemetryValue<float> Pitch => new TelemetryValue<float>(sdk, "Pitch");

        public TelemetryValue<float> Roll => new TelemetryValue<float>(sdk, "Roll");

        public TelemetryValue<int> CamCarIdx => new TelemetryValue<int>(sdk, "CamCarIdx");

        public TelemetryValue<int> CamCameraNumber => new TelemetryValue<int>(sdk, "CamCameraNumber");

        public TelemetryValue<int> CamGroupNumber => new TelemetryValue<int>(sdk, "CamGroupNumber");

        public TelemetryValue<CameraState> CamCameraState => new TelemetryValue<CameraState>(sdk, "CamCameraState");

        public TelemetryValue<bool> IsOnTrack => new TelemetryValue<bool>(sdk, "IsOnTrack");

        public TelemetryValue<bool> IsInGarage => new TelemetryValue<bool>(sdk, "IsInGarage");

        public TelemetryValue<bool> IsOnPitRoad =>
            new TelemetryValue<bool>(sdk, "OnPitRoad");

        public TelemetryValue<bool> IsPitstopActive =>
            new TelemetryValue<bool>(sdk, "PitstopActive");

        public TelemetryValue<double> PitOptRepairLeft =>
            new TelemetryValue<double>(sdk, "PitOptRepairLeft");

        public TelemetryValue<double> PitRepairLeft =>
            new TelemetryValue<double>(sdk, "PitRepairLeft");

        public TelemetryValue<float> SteeringWheelTorque => new TelemetryValue<float>(sdk, "SteeringWheelTorque");

        public TelemetryValue<float> SteeringWheelPctTorque => new TelemetryValue<float>(sdk, "SteeringWheelPctTorque");

        public TelemetryValue<float> ShiftIndicatorPct => new TelemetryValue<float>(sdk, "ShiftIndicatorPct");

        public TelemetryValue<EngineWarning> EngineWarnings => new TelemetryValue<EngineWarning>(sdk, "EngineWarnings");

        public TelemetryValue<float> FuelLevel => new TelemetryValue<float>(sdk, "FuelLevel");

        public TelemetryValue<float> FuelLevelPct => new TelemetryValue<float>(sdk, "FuelLevelPct");

        public TelemetryValue<float> FuelUsePerHour =>
            new TelemetryValue<float>(sdk, "FuelUsePerHour");

        public TelemetryValue<int> ReplayPlaySpeed => new TelemetryValue<int>(sdk, "ReplayPlaySpeed");

        public TelemetryValue<bool> ReplayPlaySlowMotion => new TelemetryValue<bool>(sdk, "ReplayPlaySlowMotion");

        public TelemetryValue<double> ReplaySessionTime => new TelemetryValue<double>(sdk, "ReplaySessionTime");

        public TelemetryValue<int> ReplaySessionNum => new TelemetryValue<int>(sdk, "ReplaySessionNum");

        public TelemetryValue<float> WaterTemp => new TelemetryValue<float>(sdk, "WaterTemp");

        public TelemetryValue<float> WaterLevel => new TelemetryValue<float>(sdk, "WaterLevel");

        public TelemetryValue<float> FuelPress => new TelemetryValue<float>(sdk, "FuelPress");

        public TelemetryValue<float> OilTemp => new TelemetryValue<float>(sdk, "OilTemp");

        public TelemetryValue<float> OilPress => new TelemetryValue<float>(sdk, "OilPress");

        public TelemetryValue<float> OilLevel => new TelemetryValue<float>(sdk, "OilLevel");

        public TelemetryValue<float> Voltage => new TelemetryValue<float>(sdk, "Voltage");

        public TelemetryValue<double> SessionTimeRemain => new TelemetryValue<double>(sdk, "SessionTimeRemain");

        public TelemetryValue<int> ReplayFrameNumEnd => new TelemetryValue<int>(sdk, "ReplayFrameNumEnd");

        public TelemetryValue<float> AirDensity => new TelemetryValue<float>(sdk, "AirDensity");

        public TelemetryValue<float> AirPressure => new TelemetryValue<float>(sdk, "AirPressure");

        public TelemetryValue<float> AirTemp => new TelemetryValue<float>(sdk, "AirTemp");

        public TelemetryValue<float> FogLevel => new TelemetryValue<float>(sdk, "FogLevel");

        public TelemetryValue<int> Skies => new TelemetryValue<int>(sdk, "Skies");

        public TelemetryValue<float> TrackTemp => new TelemetryValue<float>(sdk, "TrackTemp");

        public TelemetryValue<float> TrackTempCrew => new TelemetryValue<float>(sdk, "TrackTempCrew");

        public TelemetryValue<float> RelativeHumidity => new TelemetryValue<float>(sdk, "RelativeHumidity");

        public TelemetryValue<int> WeatherType => new TelemetryValue<int>(sdk, "WeatherType");

        public TelemetryValue<float> WindDir => new TelemetryValue<float>(sdk, "WindDir");

        public TelemetryValue<float> WindVel => new TelemetryValue<float>(sdk, "WindVel");

        public TelemetryValue<int> PlayerCarTeamIncidentCount => new TelemetryValue<int>(sdk, "PlayerCarTeamIncidentCount");

        public TelemetryValue<int> PlayerCarMyIncidentCount => new TelemetryValue<int>(sdk, "PlayerCarMyIncidentCount");

        public TelemetryValue<int> PlayerCarDriverIncidentCount => new TelemetryValue<int>(sdk, "PlayerCarDriverIncidentCount");

        public TelemetryValue<TrackSurfaces> PlayerTrackSurface => new TelemetryValue<TrackSurfaces>(sdk, "PlayerTrackSurface");

        public TelemetryValue<int> PlayerCarIdx => new TelemetryValue<int>(sdk, "PlayerCarIdx");

        public TelemetryValue<bool> WeatherDeclaredWet
            => new TelemetryValue<bool>(sdk, "WeatherDeclaredWet");

        public TelemetryValue<int> TrackWetness
            => new TelemetryValue<int>(sdk, "TrackWetness");

        public TelemetryOutput(iRacingSDK sdk)
        {
            this.sdk = sdk;
        }

        public TelemetryOutput()
        {
        }

        public IEnumerable<TelemetryValue> GetValues()
        {
            List<TelemetryValue> list = new List<TelemetryValue>();

            list.AddRange(new TelemetryValue[101]
            {
            SessionTime, SessionNum, SessionState, SessionUniqueID, SessionFlags, SessionLapsRemaining, DriverMarker, IsReplayPlaying, ReplayFrameNum, CarIdxLap, CarIdxLapCompleted,
            CarIdxLapDistPct, CarIdxTrackSurface, CarIdxSteer, CarIdxRPM, CarIdxGear, CarIdxF2Time, CarIdxEstTime, CarIdxOnPitRoad, CarIdxPosition, CarIdxClassPosition,
            SteeringWheelAngle, Throttle, Brake, Clutch, Gear, RPM, Lap, LapDist, LapDistPct, RaceLaps, LapLastLapTime,
            LongAccel, LatAccel, VertAccel, RollRate, PitchRate, YawRate, Speed, VelocityX, VelocityY, VelocityZ,
            Yaw, Pitch, Roll, CamCarIdx, CamCameraNumber, CamCameraState, CamGroupNumber, IsOnTrack, IsInGarage, SteeringWheelTorque,
            SteeringWheelPctTorque, ShiftIndicatorPct, EngineWarnings, FuelLevel, FuelLevelPct, FuelUsePerHour, IsOnPitRoad, IsPitstopActive, PitOptRepairLeft, PitRepairLeft, ReplayPlaySpeed, ReplaySessionTime, ReplaySessionNum, WaterTemp, WaterLevel,
            FuelPress, OilTemp, OilPress, OilLevel, Voltage, SessionTimeRemain, ReplayFrameNumEnd, AirDensity, AirPressure, AirTemp,
            FogLevel, Skies, TrackTemp, TrackTempCrew, RelativeHumidity, WeatherType, WindDir, WindVel, MGUKDeployAdapt, MGUKDeployFixed,
            MGUKRegenGain, EnergyBatteryToMGU, EnergyBudgetBattToMGU, EnergyERSBattery, PowerMGUH, PowerMGUK, TorqueMGUK, DrsStatus, LapCompleted, PlayerCarDriverIncidentCount,
            PlayerCarTeamIncidentCount, PlayerCarMyIncidentCount, PlayerTrackSurface, PlayerCarIdx
            });

            return list;
        }
    }
}
