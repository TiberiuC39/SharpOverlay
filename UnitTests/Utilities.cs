using Core.Models;

namespace Tests
{
    public static class TestUtils
    {
        public static class Laps
        {
            public static List<Lap> GenerateSeed(int count = 0, double targetConsumption = 0.0, double startingFuel = 100)
            {
                var laps = new List<Lap>();
                double currentFuel = startingFuel;

                for (int i = 0; i < count; i++)
                {
                    var lap = new Lap
                    {
                        Number = i + 1,
                        StartingFuel = currentFuel,
                        EndingFuel = currentFuel - targetConsumption,
                        FuelUsed = targetConsumption,
                    };

                    currentFuel -= targetConsumption;

                    laps.Add(lap);
                }

                return laps;
            }

            public static void SetLapTime(List<Lap> laps, TimeSpan avgLapTime)
            {
                foreach (var lap in laps)
                {
                    lap.Time = avgLapTime;
                }
            }
        }

    }
}
