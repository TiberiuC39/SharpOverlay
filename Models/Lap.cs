using System;

namespace SharpOverlay.Models
{
    public class Lap
    {
        public Lap()
        {
            
        }
        private Lap(int lapNumber) 
        {
            Number = lapNumber;
        }

        public Lap(int lapNumber, double startingFuel)
            :this(lapNumber)
        {
            StartingFuel = startingFuel;
        }

        public Lap(int lapNumber, TimeSpan lapTime)
            :this(lapNumber)
        {
            Time = lapTime; 
        }

        public int Number { get; set; }
        public TimeSpan Time { get; set; }
        public double StartingFuel { get; set; }
        public double EndingFuel { get; set; }
        public double FuelUsed { get; set; }
    }
}
