using System.Collections.Generic;

namespace SharpOverlay.Models
{
    public class RaceDataOutput
    {
        public WeekendData WeekendData { get; set; }
        public List<Session> Sessions { get; set; }
        public PlayerRacer Driver { get; set; }
        public Dictionary<int, Racer> Racers { get; set; }
    }
}