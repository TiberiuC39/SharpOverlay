namespace SharpOverlay.Models
{
    public class DriverPosition
    {
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