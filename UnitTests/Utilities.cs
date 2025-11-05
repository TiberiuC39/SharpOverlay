using Core.Events;
using Core.Models;
using iRacingSdkWrapper;
using iRacingSdkWrapper.JsonModels;

namespace Tests
{
    public static class TestUtils
    {
        public static class Fuel
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

        public static class BarSpotter
        {
            public static SessionEventArgs CreateSessionEventArgs()
            {
                const int trackLength = 5;
                const int paceCarIdx = 0;
                const int playerIdx = 1;
                var sessionInfo = new SessionInfo
                {
                    WeekendInfo = new WeekendInfo { TrackLength = trackLength },
                    Player = new PlayerRacer { DriverCarIdx = playerIdx },
                    Drivers = new List<Racer>
                    {
                        new Racer { CarIdx = paceCarIdx},
                        new Racer { CarIdx = playerIdx},
                        new Racer { CarIdx = playerIdx + 1},
                    }
                };

                return new SessionEventArgs(sessionInfo);
            }

            internal static TelemetryEventArgs CreateTelemetryEventArgs(float playerDistPct, float opponentDistPct, Enums.Spotter carLeftRight)
            {
                const float paceCarPct = -2f;
                return new TelemetryEventArgs()
                {
                    TelemetryOutput = new TelemetryOutputDTO {
                        CarIdxTrackDistPct = [paceCarPct, playerDistPct, opponentDistPct],
                        CarLeftRight = carLeftRight,
                    }
                };
            }

            internal static TelemetryEventArgs CreateTelemetryEventArgs(float playerDistPct, float opponentDistPct)
            {
                const float paceCarPct = -2f;
                return new TelemetryEventArgs()
                {
                    TelemetryOutput = new TelemetryOutputDTO {
                        CarIdxTrackDistPct = [paceCarPct, playerDistPct, opponentDistPct],
                    }
                };
            }
        }
    }
}
