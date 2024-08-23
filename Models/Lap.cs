using System;

namespace SharpOverlay.Models
{
    public class Lap
    {
        public int Number { get; set; }
        public TimeSpan Time { get; set; }
        public float StartingFuel { get; set; }
        public float EndingFuel { get; set; }
        public float FuelUsed { get; set; }
    }
}
