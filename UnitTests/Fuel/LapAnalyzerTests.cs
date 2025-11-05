using Core.Models;
using Core.Services.FuelCalculator.LapServices;

namespace Tests.Fuel
{
    [TestFixture]
    public class LapAnalyzerTests
    {
        private LapAnalyzer _lapAnalyzer;

        [SetUp]
        public void Setup()
        {
            _lapAnalyzer = new LapAnalyzer();
        }

        [Test]
        public void Clear_ShouldClearDriversLaps()
        {
            // Arrange
            var drivers = new Dictionary<int, Driver> { { 1, new Driver() } };
            var lastLapTimes = new Dictionary<int, TimeSpan> { { 1, TimeSpan.FromSeconds(60) } };
            var carIdxLapsCompleted = new int[] { 0, 1 }; // Index 0 is unused, index 1 has 1 lap

            _lapAnalyzer.CollectAllDriversLaps(drivers, lastLapTimes, carIdxLapsCompleted);

            var laps = _lapAnalyzer.GetDriversLaps();
            // Assert before clear
            Assert.That(laps, Has.Count.EqualTo(1));

            // Act
            _lapAnalyzer.Clear();

            // Assert after clear
            Assert.That(laps, Is.Empty);
        }

        [Test]
        public void CollectAllDriversLaps_ShouldInitializeDriverIfMissing()
        {
            // Arrange
            var drivers = new Dictionary<int, Driver> { { 10, new Driver() } };
            var lastLapTimes = new Dictionary<int, TimeSpan> { { 10, TimeSpan.FromSeconds(70) } };
            var carIdxLapsCompleted = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // Size 11 to cover index 10

            // Act
            _lapAnalyzer.CollectAllDriversLaps(drivers, lastLapTimes, carIdxLapsCompleted);

            // Assert
            var driversLaps = _lapAnalyzer.GetDriversLaps();
            Assert.Multiple(() =>
            {
                Assert.That(driversLaps.ContainsKey(10), Is.True);
                Assert.That(driversLaps[10], Is.Empty);
            });
        }

        [Test]
        public void CollectAllDriversLaps_ShouldRecordNewLap()
        {
            // Arrange
            int carIdx = 5;
            var drivers = new Dictionary<int, Driver> { { carIdx, new Driver() } };
            var lastLapTime = TimeSpan.FromSeconds(65.5);
            var lastLapTimes = new Dictionary<int, TimeSpan> { { carIdx, lastLapTime } };
            var carIdxLapsCompleted = new int[carIdx + 1];
            carIdxLapsCompleted[carIdx] = 1; // 1 lap completed

            // Act
            _lapAnalyzer.CollectAllDriversLaps(drivers, lastLapTimes, carIdxLapsCompleted);

            // Assert
            var driversLaps = _lapAnalyzer.GetDriversLaps();
            Assert.That(driversLaps[carIdx], Has.Count.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(driversLaps[carIdx][0].Number, Is.EqualTo(1));
                Assert.That(driversLaps[carIdx][0].Time, Is.EqualTo(lastLapTime));
            });

            // Arrange for second lap
            lastLapTime = TimeSpan.FromSeconds(66.0);
            lastLapTimes[carIdx] = lastLapTime;
            carIdxLapsCompleted[carIdx] = 2; // 2 laps completed

            // Act again
            _lapAnalyzer.CollectAllDriversLaps(drivers, lastLapTimes, carIdxLapsCompleted);

            // Assert second lap
            Assert.That(driversLaps[carIdx], Has.Count.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(driversLaps[carIdx][1].Number, Is.EqualTo(2));
                Assert.That(driversLaps[carIdx][1].Time, Is.EqualTo(lastLapTime));
            });
        }

        [Test]
        public void CollectAllDriversLaps_ShouldNotRecordSameLapTwice()
        {
            // Arrange
            int carIdx = 2;
            var drivers = new Dictionary<int, Driver> { { carIdx, new Driver() } };
            var lastLapTimes = new Dictionary<int, TimeSpan> { { carIdx, TimeSpan.FromSeconds(60) } };
            var carIdxLapsCompleted = new int[carIdx + 1];
            carIdxLapsCompleted[carIdx] = 1; // 1 lap completed

            _lapAnalyzer.CollectAllDriversLaps(drivers, lastLapTimes, carIdxLapsCompleted);

            // Act - Call again with same completed lap count
            _lapAnalyzer.CollectAllDriversLaps(drivers, lastLapTimes, carIdxLapsCompleted);

            var driversLaps = _lapAnalyzer.GetDriversLaps();
            // Assert
            Assert.That(driversLaps[carIdx], Has.Count.EqualTo(1));
        }

        [Test]
        public void CollectAllDriversLaps_ShouldHandleMultipleDrivers()
        {
            // Arrange
            var drivers = new Dictionary<int, Driver> { { 1, new Driver() }, { 3, new Driver() } };
            var lastLapTimes = new Dictionary<int, TimeSpan> {
                { 1, TimeSpan.FromSeconds(58) },
                { 3, TimeSpan.FromSeconds(62) }
            };
            var carIdxLapsCompleted = new int[] { 0, 1, 0, 1 }; // Index 1: 1 lap, Index 3: 1 lap

            // Act
            _lapAnalyzer.CollectAllDriversLaps(drivers, lastLapTimes, carIdxLapsCompleted);

            // Assert
            var driversLaps = _lapAnalyzer.GetDriversLaps();
            Assert.That(driversLaps, Has.Count.EqualTo(2));
            Assert.That(driversLaps[1], Has.Count.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(driversLaps[1][0].Time, Is.EqualTo(TimeSpan.FromSeconds(58)));
                Assert.That(driversLaps[3], Has.Count.EqualTo(1));
            });
            Assert.That(driversLaps[3][0].Time, Is.EqualTo(TimeSpan.FromSeconds(62)));
        }

        [Test]
        public void GetLeaderIdx_ShouldReturnLeaderIndex_WhenPositionsExist()
        {
            // Arrange
            // Position -> CarIdx
            var positionIdx = new Dictionary<int, int>
            {
                { 1, 10 }, // Leader
                { 2, 20 },
                { 5, 30 }
            };

            // Act
            int leaderIdx = _lapAnalyzer.GetLeaderIdx(positionIdx);

            // Assert
            Assert.That(leaderIdx, Is.EqualTo(10));
        }

        [Test]
        public void GetLeaderIdx_ShouldReturnInvalidLeaderPosition_WhenPositionsAreEmpty()
        {
            // Arrange
            var positionIdx = new Dictionary<int, int>();
            const int invalidLeaderPosition = -1; // Based on the source code

            // Act
            int leaderIdx = _lapAnalyzer.GetLeaderIdx(positionIdx);

            // Assert
            Assert.That(leaderIdx, Is.EqualTo(invalidLeaderPosition));
        }

        [Test]
        public void GetDriversLaps_ShouldReturnInternalState()
        {
            // The test for CollectAllDriversLaps already verifies the state is being populated.
            // This test ensures the getter returns the actual dictionary.

            // Arrange
            int carIdx = 7;
            var drivers = new Dictionary<int, Driver> { { carIdx, new Driver() } };
            var lastLapTimes = new Dictionary<int, TimeSpan> { { carIdx, TimeSpan.FromSeconds(60) } };
            var carIdxLapsCompleted = new int[carIdx + 1];
            carIdxLapsCompleted[carIdx] = 1;

            _lapAnalyzer.CollectAllDriversLaps(drivers, lastLapTimes, carIdxLapsCompleted);

            // Act
            var result = _lapAnalyzer.GetDriversLaps();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(result.ContainsKey(carIdx), Is.True);
                Assert.That(result[carIdx], Has.Count.EqualTo(1));
            });
        }

        [Test]
        public void GetLapTime_ShouldReturnAverageLapTime_WhenLapsExist()
        {
            // Arrange
            int carIdx = 1;
            TimeSpan lap1 = TimeSpan.FromSeconds(60.0);
            TimeSpan lap2 = TimeSpan.FromSeconds(62.0);
            TimeSpan expectedAverage = TimeSpan.FromSeconds(61.0);

            var drivers = new Dictionary<int, Driver> { { carIdx, new Driver() } };
            var lastLapTimes = new Dictionary<int, TimeSpan> { { carIdx, lap1 } };
            var carIdxLapsCompleted = new int[] { 0, 1 };
            _lapAnalyzer.CollectAllDriversLaps(drivers, lastLapTimes, carIdxLapsCompleted);

            lastLapTimes[carIdx] = lap2;
            carIdxLapsCompleted[carIdx] = 2;
            _lapAnalyzer.CollectAllDriversLaps(drivers, lastLapTimes, carIdxLapsCompleted);

            // Act
            TimeSpan actualAverage = _lapAnalyzer.GetLapTime(carIdx);

            // Assert
            Assert.That(actualAverage.TotalSeconds, Is.EqualTo(expectedAverage.TotalSeconds).Within(0.001));
        }

        [Test]
        public void GetLapTime_ShouldIgnoreZeroTimeLapsInAverage()
        {
            // Arrange: Manually add laps, including one with zero time
            int carIdx = 1;
            TimeSpan validLap = TimeSpan.FromSeconds(60.0);
            TimeSpan zeroLap = TimeSpan.Zero;

            // Use a temporary LapAnalyzer to manually populate state (normally discouraged, but for specific state testing of a pure method)
            // Since GetDriversLaps exposes the internal dictionary, we can modify it after initialization
            var drivers = new Dictionary<int, Driver> { { carIdx, new Driver() } };
            var lastLapTimes = new Dictionary<int, TimeSpan> { { carIdx, validLap } };
            var carIdxLapsCompleted = new int[] { 0, 1 };
            _lapAnalyzer.CollectAllDriversLaps(drivers, lastLapTimes, carIdxLapsCompleted); // Adds the valid lap

            // Add a zero-time lap manually to simulate one not yet timed or invalid (the Lap class allows this)
            var driversLaps = _lapAnalyzer.GetDriversLaps();
            driversLaps[carIdx].Add(new Lap(2, zeroLap));

            // Expected average should only use the validLap: 60.0 / 1 = 60.0
            TimeSpan expectedAverage = validLap;

            // Act
            TimeSpan actualAverage = _lapAnalyzer.GetLapTime(carIdx);

            // Assert
            Assert.That(actualAverage.TotalSeconds, Is.EqualTo(expectedAverage.TotalSeconds).Within(0.001));
        }

        [Test]
        public void GetLapTime_ShouldReturnTimeSpanZero_WhenNoValidLapsExist()
        {
            // Arrange
            int carIdx = 1;
            // The driver is initialized by CollectAllDriversLaps, but has no laps recorded
            var drivers = new Dictionary<int, Driver> { { carIdx, new Driver() } };
            var lastLapTimes = new Dictionary<int, TimeSpan> { { carIdx, TimeSpan.FromSeconds(60) } };
            var carIdxLapsCompleted = new int[] { 0, 0 }; // 0 laps completed

            _lapAnalyzer.CollectAllDriversLaps(drivers, lastLapTimes, carIdxLapsCompleted);

            // Act
            TimeSpan actualAverage = _lapAnalyzer.GetLapTime(carIdx);

            // Assert
            Assert.That(actualAverage, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void GetLapTime_ShouldReturnTimeSpanZero_WhenCarIdxDoesNotExist()
        {
            // Act
            TimeSpan result = _lapAnalyzer.GetLapTime(999);

            // Assert
            Assert.That(result, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void GetLapTime_ShouldReturnTimeSpanZero_WhenCarIdxIsNegative()
        {
            // Act
            TimeSpan result = _lapAnalyzer.GetLapTime(-1);

            // Assert
            Assert.That(result, Is.EqualTo(TimeSpan.Zero));
        }
    }
}
