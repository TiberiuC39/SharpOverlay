using Core.Models;
using Core.Services.FuelCalculator.Strategies;

namespace Tests.Fuel
{
    // Concrete implementation to test the abstract class
    public class TestCoreStrategy : CoreStrategy
    {
        public TestCoreStrategy(string name, double fuelCutOff) : base(name, fuelCutOff) { }
    }

    [TestFixture]
    public class CoreStrategyTests
    {
        private TestCoreStrategy _strategy;
        private const double FuelCutOff = 1.0;
        private const string StrategyName = "Test Strategy";

        [SetUp]
        public void Setup()
        {
            _strategy = new TestCoreStrategy(StrategyName, FuelCutOff);
        }

        [TearDown]
        public void TearDown()
        {
            _strategy.Clear(); 
        }

        [Test]
        public void Constructor_ShouldSetNameAndFuelCutOff()
        {
            // Act
            StrategyViewModel view = _strategy.GetView();

            // Assert
            Assert.That(view.Name, Is.EqualTo(StrategyName));
        }

        [Test]
        public void Clear_ShouldResetAllValuesToZero()
        {
            // Arrange: Perform a calculation to set internal state
            var laps = TestUtils.Fuel.GenerateSeed(5, 50.0);
            _strategy.Calculate(laps, 5);

            // Act
            _strategy.Clear();
            StrategyViewModel view = _strategy.GetView();

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(view.FuelConsumption, Is.EqualTo(0.0));
                Assert.That(view.RefuelAmount, Is.EqualTo(0.0));
                Assert.That(view.LapsOfFuelRemaining, Is.EqualTo(0.0));
                Assert.That(_strategy.RequiresRefueling(), Is.False);
            });
        }

        // --- GetAverageFuelConsumption Tests ---

        [Test]
        public void GetAverageFuelConsumption_ShouldReturnLastLapFuelUsed()
        {
            // Arrange
            var laps = TestUtils.Fuel.GenerateSeed(3);
            laps[0].FuelUsed = 10.0;
            laps[1].FuelUsed = 5.0;
            laps[2].FuelUsed = 4.5; // Only the last lap's consumption matters

            // Act
            _strategy.Calculate(laps, 1);
            double result = _strategy.GetView().FuelConsumption;

            // Assert
            double target = 4.5;
            Assert.That(result, Is.EqualTo(target));
        }

        [Test]
        public void GetAverageFuelConsumption_ShouldReturnZeroForNoLaps()
        {
            var laps = TestUtils.Fuel.GenerateSeed(0);

            // Act
            _strategy.Calculate(laps, 1);
            double result = _strategy.GetView().FuelConsumption;

            // Assert
            double target = 0;
            Assert.That(result, Is.EqualTo(target));
        }

        // --- Calculate Tests ---
        [Test]
        public void Calculate_ShouldDetermineFuelConsumptionAndCallUpdateRefuel()
        {
            // Arrange
            const int lapCount = 5;
            const double consumption = 5.0;
            const double startFuel = 50.0;
            const int lapsRemaining = 5; // Fuel required: 5 * 5.0 = 25.0
            var laps = TestUtils.Fuel.GenerateSeed(lapCount, consumption, startFuel);

            // Act
            _strategy.Calculate(laps, lapsRemaining);
            StrategyViewModel view = _strategy.GetView();

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(view.FuelConsumption, Is.EqualTo(consumption));
                // Check Refuel amount to account for engine shut off
                Assert.That(view.RefuelAmount, Is.EqualTo(FuelCutOff));
                // Check LapsOfFuelRemaining (50.0 current / 5.0 consumption = 10.0 laps)
                Assert.That(view.LapsOfFuelRemaining, Is.LessThan(lapsRemaining));
            });
        }

        [Test]
        public void Calculate_ShouldSkipUpdateRefuelIfNoLapsCompleted()
        {
            // Arrange: Initial state should remain 0
            const int lapsRemaining = 5;

            // Act
            _strategy.Calculate(new List<Lap>(), lapsRemaining);
            StrategyViewModel view = _strategy.GetView();

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(view.FuelConsumption, Is.EqualTo(0.0));
                Assert.That(view.RefuelAmount, Is.EqualTo(0.0));
                Assert.That(view.LapsOfFuelRemaining, Is.EqualTo(0.0));
            });
        }

        // --- UpdateRefuel Tests ---

        [Test]
        public void UpdateRefuel_ShouldRequireRefuel_WhenFuelLevelIsInsufficient()
        {
            // Arrange
            const double startingFuel = 10.0;
            const int lapsRemaining = 5;
            const double consumption = 5.0; // Fuel required: 25.0
            const double expectedRefuel = (startingFuel - consumption - (lapsRemaining * consumption)) * -1 + FuelCutOff; // 15.0 required

            // Set consumption
            var laps = TestUtils.Fuel.GenerateSeed(1, consumption, startingFuel);
            _strategy.Calculate(laps, lapsRemaining);

            // Act
            StrategyViewModel view = _strategy.GetView();

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(view.RefuelAmount, Is.EqualTo(expectedRefuel));
                Assert.That(_strategy.RequiresRefueling(), Is.True);
            });
        }

        [Test]
        public void UpdateRefuel_ShouldNotRequireRefuel_WhenFuelLevelIsSufficient()
        {
            // Arrange
            const double startingFuel = 35;
            const double currentFuel = 30.0;
            const int lapsRemaining = 5;
            const double consumption = 5.0; // Fuel required: 25.0

            // Set consumption
            _strategy.Calculate(TestUtils.Fuel.GenerateSeed(1, consumption, startingFuel), 0);

            // Act
            _strategy.UpdateRefuel(currentFuel, lapsRemaining);
            StrategyViewModel view = _strategy.GetView();

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(view.RefuelAmount, Is.LessThan(0));
                Assert.That(_strategy.RequiresRefueling(), Is.False);
            });
        }

        [Test]
        public void UpdateRefuel_ShouldApplyFuelCutOff_WhenFuelAtEndIsPositiveButBelowCutOff()
        {
            // Arrange
            const double startingFuel = 25.5; // Enough fuel (25.0) + 0.5 left
            const int lapsRemaining = 4;
            const double consumption = 5.0; // Fuel required: 25.0
                                            // FuelAtEnd = 20.5 - 20.0 = 0.5. This is > 0 but < FuelCutOff (1.0).
                                            // Difference = 1.0 - 0.5 = 0.5.
            const double expectedRefuel = 0.5;

            // Set consumption
            var laps = TestUtils.Fuel.GenerateSeed(1, consumption, startingFuel);
            _strategy.Calculate(laps, lapsRemaining);

            // Act
            // _strategy.UpdateRefuel(currentFuel, lapsRemaining);
            StrategyViewModel view = _strategy.GetView();

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(view.RefuelAmount, Is.EqualTo(expectedRefuel));
                Assert.That(_strategy.RequiresRefueling(), Is.True);
            });
        }

        [Test]
        public void UpdateRefuel_ShouldNotApplyFuelCutOff_WhenFuelAtEndIsAboveCutOff()
        {
            // Arrange
            const double currentFuel = 26.1;
            const int lapsRemaining = 5;
            const double consumption = 5.0;
            // FuelAtEnd = 26.1 - 25.0 = 1.1. This is > FuelCutOff (1.0).
            // RefuelRequired should be 0.

            // Set consumption
            _strategy.Calculate(TestUtils.Fuel.GenerateSeed(1, consumption, currentFuel), 0);

            // Act
            _strategy.UpdateRefuel(currentFuel, lapsRemaining);
            StrategyViewModel view = _strategy.GetView();

            // Assert
            Assert.That(_strategy.RequiresRefueling(), Is.False);
        }

        [Test]
        public void UpdateRefuel_ShouldReturnZeroRefuel_WhenZeroLapsRemaining()
        {
            // Arrange
            const double currentFuel = 50.0;
            const double consumption = 5.0;

            // Set consumption
            _strategy.Calculate(TestUtils.Fuel.GenerateSeed(1, consumption, currentFuel), 0);

            // Act
            _strategy.UpdateRefuel(currentFuel, 0);
            StrategyViewModel view = _strategy.GetView();

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(view.RefuelAmount, Is.EqualTo(0.0));
                // FuelAtEnd and LapsOfFuelRemaining should still be calculated in this case
                Assert.That(view.LapsOfFuelRemaining, Is.LessThan(10.0));
            });
        }

        [Test]
        public void UpdateRefuel_ShouldHandleZeroConsumption()
        {
            // Arrange
            const double currentFuel = 50.0;
            const int lapsRemaining = 5;
            const double consumption = 0.0;

            // Set consumption
            _strategy.Calculate(TestUtils.Fuel.GenerateSeed(1, consumption, currentFuel), 0);

            // Act
            _strategy.UpdateRefuel(currentFuel, lapsRemaining);
            StrategyViewModel view = _strategy.GetView();

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(view.RefuelAmount, Is.EqualTo(0.0));
                Assert.That(view.LapsOfFuelRemaining, Is.EqualTo(0.0));
            });
        }

        // --- UpdateLapsOfFuelRemaining Tests ---

        [Test]
        public void UpdateLapsOfFuelRemaining_ShouldCalculateCorrectLaps()
        {
            // Arrange
            const double currentFuel = 50.0;
            const double consumption = 5.0; // 50 / 5 = 10 less than laps due to fuel cut off

            // Set consumption
            _strategy.Calculate(TestUtils.Fuel.GenerateSeed(1, consumption, currentFuel), 0);

            // Act
            _strategy.UpdateLapsOfFuelRemaining(currentFuel);
            StrategyViewModel view = _strategy.GetView();

            // Assert
            Assert.That(view.LapsOfFuelRemaining, Is.LessThan(10.0));
        }

        [Test]
        public void UpdateLapsOfFuelRemaining_ShouldReturnZeroForZeroConsumption()
        {
            // Arrange
            const double currentFuel = 45.0;
            const double consumption = 0.0;

            // Set consumption
            _strategy.Calculate(TestUtils.Fuel.GenerateSeed(1, consumption, currentFuel), 0);

            // Act
            _strategy.UpdateLapsOfFuelRemaining(currentFuel);
            StrategyViewModel view = _strategy.GetView();

            // Assert
            Assert.That(view.LapsOfFuelRemaining, Is.EqualTo(0.0));
        }

        [Test]
        public void RequiresRefueling_ShouldReflectRefuelRequiredValue()
        {
            // Arrange: Requires refuel
            const double currentFuel = 10.0;
            const int lapsRemaining = 5;
            const double consumption = 5.0;
            _strategy.Calculate(TestUtils.Fuel.GenerateSeed(1, consumption, currentFuel), 0);
            _strategy.UpdateRefuel(currentFuel, lapsRemaining);

            // Assert 1
            Assert.That(_strategy.RequiresRefueling(), Is.True);

            // Arrange: Does not require refuel
            _strategy.UpdateRefuel(30.0, lapsRemaining);

            // Assert 2
            Assert.That(_strategy.RequiresRefueling(), Is.False);
        }
    }
}

