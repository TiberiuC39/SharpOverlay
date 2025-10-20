using Core.Models;
using Core.Services.FuelCalculator.Strategies;
using SharpOverlay.Strategies;

namespace Tests.Fuel
{
    [TestFixture]
    public class FiveLapStrategyTests
    {
        private FiveLapStrategy _strategy;
        private const double FuelCutOff = 1.0;
        private const string StrategyName = "5L";

        [SetUp]
        public void Setup()
        {
            _strategy = new FiveLapStrategy(FuelCutOff);
        }

        [Test]
        public void Constructor_ShouldSetCorrectName()
        {
            // Act
            StrategyViewModel view = _strategy.GetView();

            // Assert
            Assert.That(view.Name, Is.EqualTo(StrategyName));
        }

        [Test]
        public void GetAverageFuelConsumption_ShouldUseBaseImplementation_WhenFiveLapsOrFewer()
        {
            // Arrange
            var laps3 = new List<Lap>
            {
                new Lap { Number = 1, FuelUsed = 10.0, StartingFuel = 50.0, EndingFuel = 40.0 },
                new Lap { Number = 2, FuelUsed = 5.0, StartingFuel = 40.0, EndingFuel = 35.0 },
                new Lap { Number = 3, FuelUsed = 4.0, StartingFuel = 35.0, EndingFuel = 31.0 },
            };

            var laps5 = new List<Lap>
            {
                new Lap { Number = 1, FuelUsed = 10.0, StartingFuel = 50.0, EndingFuel = 40.0 },
                new Lap { Number = 2, FuelUsed = 5.0, StartingFuel = 40.0, EndingFuel = 35.0 },
                new Lap { Number = 3, FuelUsed = 4.0, StartingFuel = 35.0, EndingFuel = 31.0 },
                new Lap { Number = 4, FuelUsed = 6.0, StartingFuel = 31.0, EndingFuel = 25.0 },
                new Lap { Number = 5, FuelUsed = 7.0, StartingFuel = 25.0, EndingFuel = 18.0 }
            };

            // Case 1: Less than 5 laps (3 laps)
            _strategy.Calculate(laps3, 0);
            double consumption3Laps = _strategy.GetView().FuelConsumption;
            Assert.That(consumption3Laps, Is.EqualTo(laps3.Last().FuelUsed), "Should use base (last lap) for < 5 laps.");

            // Case 2: Exactly 5 laps
            _strategy.Calculate(laps5, 0);
            double consumption5Laps = _strategy.GetView().FuelConsumption;
            Assert.That(consumption5Laps, Is.EqualTo(laps5.Last().FuelUsed), "Should use base (last lap) for 5 laps.");
        }

        [Test]
        public void GetAverageFuelConsumption_ShouldCalculateAverageOfLastFiveLaps_WhenMoreThanFiveLaps()
        {
            // Arrange
            // Consumption: L1: 10, L2: 5, L3: 4, L4: 6, L5: 7, L6: 5, L7: 8
            var laps = new List<Lap>
            {
                // L1: 10.0 (Ignored in 5-lap average)
                new Lap { Number = 1, FuelUsed = 10.0, EndingFuel = 90.0 }, 
                // L2: 5.0 (Included in 5-lap average)
                new Lap { Number = 2, FuelUsed = 5.0, EndingFuel = 85.0 },
                // L3: 4.0 (Included in 5-lap average)
                new Lap { Number = 3, FuelUsed = 4.0, EndingFuel = 81.0 },
                // L4: 6.0 (Included in 5-lap average)
                new Lap { Number = 4, FuelUsed = 6.0, EndingFuel = 75.0 },
                // L5: 7.0 (Included in 5-lap average)
                new Lap { Number = 5, FuelUsed = 7.0, EndingFuel = 68.0 },
                // L6: 5.0 (Included in 5-lap average)
                new Lap { Number = 6, FuelUsed = 5.0, EndingFuel = 63.0 },
                // L7: 8.0 (Included in 5-lap average, Last Lap with current fuel 50.0)
                new Lap { Number = 7, FuelUsed = 8.0, EndingFuel = 50.0 },
            };
            // Expected average of last 5: (4.0 + 6.0 + 7.0 + 5.0 + 8.0) / 5 = 30.0 / 5 = 6.0

            // Act
            _strategy.Calculate(laps, 0);
            double consumption7Laps = _strategy.GetView().FuelConsumption;

            // Assert
            Assert.That(consumption7Laps, Is.EqualTo(6.0));
        }

        [Test]
        public void Calculate_ShouldUseFiveLapAverage_AndUpdateRefuel()
        {
            // Generate 7 laps where the last 5 average 6.0
            var laps = new List<Lap>
            {
                new Lap { Number = 1, FuelUsed = 10.0, EndingFuel = 90 }, // L1 (Ignored) - 90
                new Lap { Number = 2, FuelUsed = 5.0, EndingFuel = 85 }, // L2 (Last 5 start here) - 85
                new Lap { Number = 3, FuelUsed = 4.0 }, // L3 - 81
                new Lap { Number = 4, FuelUsed = 6.0 }, // L4 - 75
                new Lap { Number = 5, FuelUsed = 7.0 }, // L5 - 68
                new Lap { Number = 6, FuelUsed = 5.0 }, // L6 - 63
                new Lap { Number = 7, FuelUsed = 8.0, EndingFuel = 55 } // L7 - 55
            };

            // Arrange
            const int lapsRemaining = 10;
            const double avgConsumption = 6.0;
            const double startingFuel = 100.0;
            double fuelUsedBeforeCalc = laps.Count * laps.Average(l => l.FuelUsed); // 45.0
            double fuelRequiredTotal = avgConsumption * lapsRemaining + fuelUsedBeforeCalc; // 105.0
            double refuelRequired = fuelRequiredTotal - startingFuel + FuelCutOff;

            // Act
            _strategy.Calculate(laps, lapsRemaining);
            StrategyViewModel view = _strategy.GetView();

            // Assert
            Assert.That(view.FuelConsumption, Is.EqualTo(avgConsumption));
            Assert.That(view.RefuelAmount, Is.EqualTo(refuelRequired));
            // Laps remaining: 55 / 6.0 = 11.666...
            Assert.That(view.LapsOfFuelRemaining, Is.LessThan(9.166666666666667).Within(1e-9));
            Assert.That(_strategy.RequiresRefueling(), Is.True);
        }
    }
}
