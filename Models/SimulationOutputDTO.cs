﻿using iRacingSdkWrapper;
using System;

namespace SharpOverlay.Models
{
    public class SimulationOutputDTO
    {
        public SimulationOutputDTO()
        {            
        }
        public SimulationOutputDTO(TelemetryInfo telemetry)
        {
            FuelLevel = telemetry.FuelLevel.Value;
            CurrentLapNumber = telemetry.Lap.Value;
            IsOnPitRoad = telemetry.IsOnPitRoad.Value;
            IsReceivingService = telemetry.IsPitstopActive.Value;
            EnterExitResetButton = telemetry.EnterExitReset.Value;
            SessionState = telemetry.SessionState.Value;
            LastLapTime = TimeSpan.FromSeconds(telemetry.LapLastLapTime.Value);
            SessionTimeRemaining = TimeSpan.FromSeconds(telemetry.SessionTimeRemain.Value);
            CarIdxLastLapTime = telemetry.CarIdxLastLapTime.Value;
            CarIdxLapCompleted = telemetry.CarIdxLapCompleted.Value;
        }

        public double FuelLevel { get; }
        public int CurrentLapNumber { get; }
        public bool IsOnPitRoad { get; }
        public bool IsReceivingService { get; }
        public int EnterExitResetButton { get; }
        public SessionStates SessionState { get; }
        public TimeSpan LastLapTime { get; }
        public TimeSpan SessionTimeRemaining { get; }
        public float[] CarIdxLastLapTime { get; }
        public int[] CarIdxLapCompleted { get; }
    }
}