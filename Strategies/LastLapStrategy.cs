namespace SharpOverlay.Strategies
{
    public class LastLapStrategy : CoreStrategy
    {
        private const string _name = "LAST";

        public LastLapStrategy(double fuelCutOff)
            :base(_name, fuelCutOff)
        {            
        }
    }
}
