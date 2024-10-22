namespace SharpOverlay.Strategies
{
    public class FullRaceStrategy : CoreStrategy
    {
        private const string _name = "FULL";

        public FullRaceStrategy(double fuelCutOff)
            : base(_name, fuelCutOff)
        {            
        }
    }
}
