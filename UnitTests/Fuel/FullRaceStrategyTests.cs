using Core.Models;
using Core.Services.FuelCalculator.Strategies;
using SharpOverlay.Strategies;

namespace Tests.Fuel
{
    [TestFixture]
    public class FullRaceStrategyTests
    {
        private FullRaceStrategy _strategy;
        private const double FuelCutOff = 1.0;
        private const string StrategyName = "FULL";

        [SetUp]
        public void Setup()
        {
            _strategy = new FullRaceStrategy(FuelCutOff);
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
        public void GetAverageFuelConsumption_ShouldUseBaseImplementation_WhenOneLapOrFewer()
        {
            // Arrange
            var zeroLaps = TestUtils.Fuel.GenerateSeed();
            var oneLap = TestUtils.Fuel.GenerateSeed(1, 10, 50);

            // Case 1: Zero laps
            _strategy.Calculate(zeroLaps, 1);
            StrategyViewModel view = _strategy.GetView();
            double consumption0Laps = view.FuelConsumption;
            Assert.That(consumption0Laps, Is.EqualTo(0.0), "Should return default (0) for 0 laps.");

            // Case 2: Exactly 1 lap
            _strategy.Calculate(oneLap, 1);
            view = _strategy.GetView();
            double consumption1Lap = view.FuelConsumption;
            Assert.That(consumption1Lap, Is.EqualTo(10.0), "Should use base (last lap) for 1 lap.");
        }

        [Test]
        public void GetAverageFuelConsumption_ShouldCalculateAverageOfAllButFirstLap_WhenMoreThanOneLap()
        {
            // Arrange
            // L1 (Skipped): 10.0
            // L2 (Included): 5.0
            // L3 (Included): 4.0
            // L4 (Included): 6.0
            var laps = new List<Lap>
            {
                new Lap { Number = 1, FuelUsed = 10.0 },
                new Lap { Number = 2, FuelUsed = 5.0 },
                new Lap { Number = 3, FuelUsed = 4.0 },
                new Lap { Number = 4, FuelUsed = 6.0, EndingFuel = 50.0 }
            };
            // Expected average of laps 2, 3, and 4: (5.0 + 4.0 + 6.0) / 3 = 15.0 / 3 = 5.0

            // Act
            _strategy.Calculate(laps, 1);
            StrategyViewModel view = _strategy.GetView();
            double consumption4Laps = view.FuelConsumption;

            // Assert
            Assert.That(consumption4Laps, Is.EqualTo(5.0));
        }

        [Test]
        public void Calculate_ShouldUseFullRaceAverage_AndUpdateAllValues()
        {
            // Arrange
            const int lapsRemaining = 10;
            const double avgConsumption = 5.0;

            // Generate 4 laps where L2, L3, L4 average 5.0
            var laps = new List<Lap>
            {
                new Lap { Number = 1, FuelUsed = 10.0 }, // L1 (Ignored for FullRaceStrategy)
                new Lap { Number = 2, FuelUsed = 5.0 }, // L2
                new Lap { Number = 3, FuelUsed = 4.0 }, // L3
                new Lap { Number = 4, FuelUsed = 6.0, EndingFuel = 50.0 } // L4 (End fuel 50.0)
            };

            // Act
            _strategy.Calculate(laps, lapsRemaining);
            StrategyViewModel view = _strategy.GetView();

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(view.FuelConsumption, Is.EqualTo(avgConsumption));
                Assert.That(view.RefuelAmount, Is.EqualTo(FuelCutOff));
                // Laps remaining: 70.0 / 5.0 = 14.0
                Assert.That(view.LapsOfFuelRemaining, Is.LessThan(10.0).Within(1e-9));
                Assert.That(_strategy.RequiresRefueling(), Is.True); // Fuel Cut Off
            });
        }
    }
}
