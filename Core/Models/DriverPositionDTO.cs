using iRacingSdkWrapper.JsonModels;

namespace Core.Models
{
    public class DriverPositionDTO
    {
        public DriverPositionDTO(DriverPosition dp)
        {
            Position = dp.Position;
            ClassPosition = dp.ClassPosition;
            CarIdx = dp.CarIdx;
            Lap = dp.Lap;
            Time = dp.Time;
            FastestLap = dp.FastestLap;
            FastestTime = dp.FastestTime;
            LastTime = dp.LastTime;
            LapsLed = dp.LapsLed;
            LapsComplete = dp.LapsComplete;
            JokerLapsComplete = dp.JokerLapsComplete;
            LapsDriven = dp.LapsDriven;
            Incidents = dp.Incidents;
            ReasonOutId = dp.ReasonOutId;
            ReasonOutStr = dp.ReasonOutStr;
        }

        public int Position { get; set; }

        public int ClassPosition { get; set; }

        public int CarIdx { get; set; }

        public int Lap { get; set; }

        public float Time { get; set; }

        public int FastestLap { get; set; }

        public float FastestTime { get; set; }

        public float LastTime { get; set; }

        public int LapsLed { get; set; }

        public int LapsComplete { get; set; }

        public int JokerLapsComplete { get; set; }

        public float LapsDriven { get; set; }

        public int Incidents { get; set; }

        public int ReasonOutId { get; set; }

        public string ReasonOutStr { get; set; }
    }
}

