using System.Collections.Generic;

namespace SharpOverlay.Services.FuelServices
{
    public class RaceHistory
    {
        public Dictionary<int, FuelModel> ByCarId { get; set; } = [];
    }
}
