using iRacingSdkWrapper.JsonModels;

namespace Core.Models
{
    public class FastestLapDTO
    {
        public FastestLapDTO(FastestLap fl)
        {
            CarIdx = fl.CarIdx;
            FastestLapNumber = fl.FastestLapNumber;
            FastestTime = fl.FastestTime;
        }

        public int CarIdx { get; set; }

        public int FastestLapNumber { get; set; }

        public float FastestTime { get; set; }
    }
}

