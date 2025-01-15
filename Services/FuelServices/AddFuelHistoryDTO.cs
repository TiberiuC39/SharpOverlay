using System;

namespace SharpOverlay.Services.FuelServices
{
    public class AddFuelHistoryDTO
    {
        public int TrackId { get; set; }
        public double Consumption { get; set; }
        public int LapCount { get; set; }
        public TimeSpan LapTime { get; set; }
        public int CarId { get; set; }
    }
}