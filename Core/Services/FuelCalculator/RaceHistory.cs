namespace Core.Services.FuelCalculator
{
    public class RaceHistory
    {
        public Dictionary<int, FuelModel> ByCarId { get; set; } = [];
    }
}
