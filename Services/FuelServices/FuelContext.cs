using System.Collections.Generic;

namespace SharpOverlay.Services.FuelServices
{
    public class FuelContext
    {
        public Dictionary<int, RaceHistory> ByTrack { get; set; } = [];
    }
}