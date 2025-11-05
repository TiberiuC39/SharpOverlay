using iRacingSdkWrapper;
using iRacingSdkWrapper.Bitfields;
using static Core.Models.Enums;

namespace Core.Models
{
    public class TelemetryOutputDTO
    {
        public TelemetryOutputDTO()
        {
        }
        public TelemetryOutputDTO(TelemetryInfo telemetry)
        {
            FuelLevel = telemetry.FuelLevel.Value;
            CurrentLapNumber = telemetry.Lap.Value;
            IsOnPitRoad = telemetry.IsOnPitRoad.Value;
            TrackSurface = telemetry.PlayerTrackSurface.Value;
            IsReceivingService = telemetry.IsPitstopActive.Value;
            PlayerTrackDistPct = telemetry.LapDistPct.Value;
            CarIdxTrackDistPct = telemetry.CarIdxLapDistPct.Value;
            EnterExitResetButton = telemetry.EnterExitReset.Value;
            SessionState = telemetry.SessionState.Value;
            LastLapTime = TimeSpan.FromSeconds(telemetry.LapLastLapTime.Value);
            SessionTimeRemaining = TimeSpan.FromSeconds(telemetry.SessionTimeRemain.Value);
            CarIdxLastLapTime = telemetry.CarIdxLastLapTime.Value;
            CarIdxLapCompleted = telemetry.CarIdxLapCompleted.Value;
            IsOnTrack = telemetry.IsOnTrack.Value;
            SessionFlag = (SessionFlags)telemetry.SessionFlags.Value.Value;
            CarLeftRight = (Spotter)(int)telemetry.CarLeftRight.Value;
            Brake = telemetry.Brake.Value;
            BrakeRaw = telemetry.BrakeRaw.Value;
            Throttle = telemetry.Throttle.Value;
            ThrottleRaw = telemetry.ThrottleRaw.Value;
            Clutch = telemetry.Clutch.Value;
            ClutchRaw = telemetry.ClutchRaw.Value;
            BrakeABSactive = telemetry.BrakeABSactive.Value;
            WindVel = telemetry.WindVel.Value;
            YawNorth = telemetry.YawNorth.Value;
            WindDir = telemetry.WindDir.Value;
            SteeringWheelAngle = telemetry.SteeringWheelAngle.Value;
            CarIdxPosition = telemetry.CarIdxPosition.Value;
            CarIdxClass = telemetry.CarIdxClass.Value;
            PlayerCarIdx = telemetry.PlayerCarIdx.Value;
            SessionNum = telemetry.SessionNum.Value;
            IsReplayPlaying = telemetry.IsReplayPlaying.Value;
        }

        public double FuelLevel { get; }
        public int CurrentLapNumber { get; }
        public bool IsOnPitRoad { get; }
        public bool IsReceivingService { get; }
        public TrackSurfaces TrackSurface { get; }
        public float PlayerTrackDistPct { get; set; }
        public float[] CarIdxTrackDistPct { get; set;}
        public int EnterExitResetButton { get; }
        public SessionStates SessionState { get; }
        public TimeSpan LastLapTime { get; }
        public TimeSpan SessionTimeRemaining { get; }
        public float[] CarIdxLastLapTime { get; }
        public int[] CarIdxLapCompleted { get; }
        public bool IsOnTrack { get; }
        public SessionFlags SessionFlag { get; }
        public Enums.Spotter CarLeftRight { get; set;}
        public float BrakeRaw { get; set; }
        public float ThrottleRaw { get; set; }
        public float ClutchRaw { get; set; }
        public float Brake { get; set; }
        public float Throttle { get; set; }
        public float Clutch { get; set; }
        public float SteeringWheelAngle { get; set; }
        public bool BrakeABSactive { get; set; }
        public float WindVel { get; set; }
        public float WindDir { get; set; }
        public float YawNorth { get; set; }
        public int[] CarIdxPosition { get; }
        public int[] CarIdxClass { get; }
        public int PlayerCarIdx { get; }
        public int SessionNum { get; }
        public bool IsReplayPlaying { get; internal set; }
    }
}
