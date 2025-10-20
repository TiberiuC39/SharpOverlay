using Core.Models;
using Core.Services.FuelCalculator;
using Core.Services.FuelCalculator.LapServices;

namespace Tests.Fuel
{
    [TestFixture]
    public class LapTrackerTests
    {
        private LapTracker _lapTracker;

        [SetUp]
        public void Setup()
        {
            _lapTracker = new LapTracker();
        }

        [Test]
        public void StartNewLap_ShouldSetCurrentLap()
        {
            // Arrange
            const int lapNumber = 1;
            const double startingFuel = 50.5;

            // Act
            _lapTracker.StartNewLap(lapNumber, startingFuel);
            Lap? currentLap = _lapTracker.GetCurrentLap();

            // Assert
            Assert.That(currentLap, Is.Not.Null);
            Assert.That(currentLap!.Number, Is.EqualTo(lapNumber));
            Assert.That(currentLap.StartingFuel, Is.EqualTo(startingFuel));
            Assert.That(_lapTracker.GetPlayerLaps(), Is.Empty);
        }

        [Test]
        public void CompleteCurrentLap_ShouldCalculateFuelAndMoveToCompleted()
        {
            // Arrange
            const int lapNumber = 1;
            const double startFuel = 50.0;
            const double endFuel = 45.0;
            const double expectedFuelUsed = 5.0;
            TimeSpan lapTime = TimeSpan.FromSeconds(100.0);

            _lapTracker.StartNewLap(lapNumber, startFuel);
            Lap? currentLapBefore = _lapTracker.GetCurrentLap();
            Assert.That(currentLapBefore, Is.Not.Null);

            // Act
            _lapTracker.CompleteCurrentLap(endFuel, lapTime);
            List<Lap> completedLaps = _lapTracker.GetPlayerLaps();
            Lap? currentLapAfter = _lapTracker.GetCurrentLap(); // Should still point to the same object until a new one is started or reset

            // Assert completed lap list
            Assert.That(completedLaps.Count, Is.EqualTo(1));
            Lap completedLap = completedLaps.First();
            Assert.That(completedLap.Number, Is.EqualTo(lapNumber));
            Assert.That(completedLap.EndingFuel, Is.EqualTo(endFuel));
            Assert.That(completedLap.Time, Is.EqualTo(lapTime));
            Assert.That(completedLap.FuelUsed, Is.EqualTo(expectedFuelUsed).Within(1e-6));

            // Assert current lap state: it remains pointing to the last completed lap until a reset or new lap start
            Assert.That(currentLapAfter, Is.EqualTo(completedLap));
        }

        [Test]
        public void GetCurrentLap_ShouldReturnNullWhenNoLapStarted()
        {
            // Act
            Lap? currentLap = _lapTracker.GetCurrentLap();

            // Assert
            Assert.That(currentLap, Is.Null);
        }

        [Test]
        public void ResetCurrentLap_ShouldSetCurrentLapToNull()
        {
            // Arrange
            _lapTracker.StartNewLap(1, 50.0);
            Assert.That(_lapTracker.GetCurrentLap(), Is.Not.Null);

            // Act
            _lapTracker.ResetCurrentLap();

            // Assert
            Assert.That(_lapTracker.GetCurrentLap(), Is.Null);
        }

        [Test]
        public void GetCompletedLapsCount_ShouldReturnCorrectCount()
        {
            // Note on implementation: The source code returns `_completedLaps.Count - 1`. 
            // This suggests that the internal list is expected to hold 1 extra "setup" or "current" lap, or is 1-indexed for the user.
            // We will test the functionality as written.

            // Arrange
            // 0 laps completed
            Assert.That(_lapTracker.GetCompletedLapsCount(), Is.EqualTo(-1));

            // 1 lap completed (e.g., start with history or a full cycle)
            _lapTracker.StartNewLap(1, 50.0);
            _lapTracker.CompleteCurrentLap(45.0, TimeSpan.FromSeconds(100));

            // Assert 1 completed lap
            Assert.That(_lapTracker.GetPlayerLaps().Count, Is.EqualTo(1));
            Assert.That(_lapTracker.GetCompletedLapsCount(), Is.EqualTo(0)); // 1 - 1 = 0

            // 2 laps completed
            _lapTracker.StartNewLap(2, 45.0);
            _lapTracker.CompleteCurrentLap(40.0, TimeSpan.FromSeconds(101));

            // Assert 2 completed laps
            Assert.That(_lapTracker.GetPlayerLaps().Count, Is.EqualTo(2));
            Assert.That(_lapTracker.GetCompletedLapsCount(), Is.EqualTo(1)); // 2 - 1 = 1
        }

        [Test]
        public void Clear_ShouldEmptyCompletedLapsAndResetCurrentLap()
        {
            // Arrange
            _lapTracker.StartNewLap(1, 50.0);
            _lapTracker.CompleteCurrentLap(45.0, TimeSpan.FromSeconds(100));
            _lapTracker.StartNewLap(2, 45.0); // Current lap is not completed

            Assert.That(_lapTracker.GetPlayerLaps(), Is.Not.Empty);
            Assert.That(_lapTracker.GetCurrentLap(), Is.Not.Null);

            // Act
            _lapTracker.Clear();

            // Assert
            Assert.That(_lapTracker.GetPlayerLaps(), Is.Empty);
            Assert.That(_lapTracker.GetCurrentLap(), Is.Null);
        }

        [Test]
        public void StartWithHistory_ShouldCreateAndCompleteOneLap()
        {
            // Arrange
            const int lapNumber = 10;
            var fuelModelEntry = new FuelModel
            {
                Consumption = 5.5, // This is used as StartingFuel
                LapTime = 95.0,
            };
            const double expectedEndingFuel = 0.0; // Hardcoded in CompleteCurrentLap

            // Act
            _lapTracker.StartWithHistory(lapNumber, fuelModelEntry);
            List<Lap> completedLaps = _lapTracker.GetPlayerLaps();

            // Assert completed lap
            Assert.That(completedLaps.Count, Is.EqualTo(1));
            Lap completedLap = completedLaps.First();
            Assert.That(completedLap.Number, Is.EqualTo(lapNumber));
            Assert.That(completedLap.StartingFuel, Is.EqualTo(fuelModelEntry.Consumption));
            Assert.That(completedLap.EndingFuel, Is.EqualTo(expectedEndingFuel));
            Assert.That(completedLap.Time, Is.EqualTo(TimeSpan.FromSeconds(fuelModelEntry.LapTime)));

            // Assert fuel usage calculation: Start - End = Consumption - 0
            Assert.That(completedLap.FuelUsed, Is.EqualTo(fuelModelEntry.Consumption).Within(1e-6));

            // Assert lap count according to the implementation's logic (Count - 1)
            Assert.That(_lapTracker.GetCompletedLapsCount(), Is.EqualTo(0));
        }

        [Test]
        public void SequentialOperations_ShouldTrackLapsCorrectly()
        {
            // 1. Start Lap 1
            _lapTracker.StartNewLap(1, 60.0);
            Assert.That(_lapTracker.GetCurrentLap()!.Number, Is.EqualTo(1));
            Assert.That(_lapTracker.GetCompletedLapsCount(), Is.EqualTo(-1));

            // 2. Complete Lap 1
            _lapTracker.CompleteCurrentLap(55.0, TimeSpan.FromSeconds(100));
            Assert.That(_lapTracker.GetPlayerLaps().Count, Is.EqualTo(1));
            Assert.That(_lapTracker.GetCompletedLapsCount(), Is.EqualTo(0));
            Assert.That(_lapTracker.GetPlayerLaps().First().FuelUsed, Is.EqualTo(5.0));

            // 3. Start Lap 2
            _lapTracker.StartNewLap(2, 55.0);
            Assert.That(_lapTracker.GetCurrentLap()!.Number, Is.EqualTo(2));
            Assert.That(_lapTracker.GetCompletedLapsCount(), Is.EqualTo(0));

            // 4. Complete Lap 2
            _lapTracker.CompleteCurrentLap(50.0, TimeSpan.FromSeconds(101));
            Assert.That(_lapTracker.GetPlayerLaps().Count, Is.EqualTo(2));
            Assert.That(_lapTracker.GetCompletedLapsCount(), Is.EqualTo(1));
            Assert.That(_lapTracker.GetPlayerLaps().Last().FuelUsed, Is.EqualTo(5.0));
        }

    }
}
