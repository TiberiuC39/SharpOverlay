using iRacingSdkWrapper.JsonModels;

namespace Core.Models
{
    public class SectorDTO
    {
        public int Num { get; set; }

        public double StartPct { get; set; }

        public SectorDTO(Sector s)
        {
            Num = s.Num;
            StartPct = s.StartPct;
        }
    }
}


