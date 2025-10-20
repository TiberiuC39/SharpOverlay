namespace Core.Models
{
    public class Enums
    {
        public enum Spotter
        {
            Off,
            Clear, // no cars around us.
            CarLeft, // there is a car to our left.
            CarRight, // there is a car to our right.
            CarLeftRight, // there are cars on each side.
            TwoCarsLeft, // there are two cars to our left.
            TwoCarsRight // there are two cars to our right. 
        };
    }
}
