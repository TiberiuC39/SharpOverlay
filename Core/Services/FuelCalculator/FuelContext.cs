namespace Core.Services.FuelCalculator
{
    public class FuelContext
    {
        public Dictionary<int, RaceHistory> ByTrack { get; set; } = [];
    }
}
