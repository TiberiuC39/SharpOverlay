using iRacingSdkWrapper.JsonModels;

namespace Core.Models
{
    public class QualifyResultDTO
    {
        public int CarIdx { get; set; }

        public int Position { get; set; }

        public int ClassPosition { get; set; }

        public int FastestLap { get; set; }

        public double FastestTime { get; set; }

        public QualifyResultDTO(QualifyResults r)
        {
            CarIdx = r.CarIdx;
            Position = r.Position;
            ClassPosition = r.ClassPosition;
            FastestLap = r.FastestLap;
            FastestTime = r.FastestTime;
        }
    }
}


