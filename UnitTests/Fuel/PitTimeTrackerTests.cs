using Core.Services.FuelCalculator.PitServices;

namespace Tests.Fuel
{
    [TestFixture]
    public class PitTimeTrackerTests
    {
        private PitTimeTracker _tracker;

        [SetUp]
        public void Setup()
        {
            _tracker = new PitTimeTracker();
        }

        [Test]
        public void InitialState_ShouldBeCorrect()
        {
            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(_tracker.IsTrackingTime, Is.False);
                Assert.That(_tracker.GetPitDuration(), Is.EqualTo(TimeSpan.Zero));
                Assert.That(_tracker.GetAvgPitStopTime(), Is.EqualTo(TimeSpan.Zero));
            });
        }

        [Test]
        public void Start_ShouldSetTrackingFlagAndTime()
        {
            // Arrange
            TimeSpan startTime = TimeSpan.FromMinutes(10);

            // Act
            _tracker.Start(startTime);

            // Assert
            Assert.That(_tracker.IsTrackingTime, Is.True);
            // _timeAtPitStart is private, relying on Stop method to confirm it's set correctly
        }

        [Test]
        public void Stop_ShouldCalculateDurationAndResetStartTime()
        {
            // Arrange
            TimeSpan startTime = TimeSpan.FromSeconds(300);
            TimeSpan endTime = TimeSpan.FromSeconds(270);
            TimeSpan expectedDuration = TimeSpan.FromSeconds(30);

            _tracker.Start(startTime);
            Assert.That(_tracker.IsTrackingTime, Is.True);

            // Act
            _tracker.Stop(endTime);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(_tracker.IsTrackingTime, Is.False);
                Assert.That(_tracker.GetPitDuration(), Is.EqualTo(expectedDuration));
                Assert.That(_tracker.GetAvgPitStopTime(), Is.EqualTo(expectedDuration));
            });
        }

        [Test]
        public void Stop_ShouldNotCalculateDurationIfStartWasNotCalled()
        {
            // Arrange
            TimeSpan endTime = TimeSpan.FromSeconds(50);

            // Act
            _tracker.Stop(endTime);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(_tracker.IsTrackingTime, Is.False);
                Assert.That(_tracker.GetPitDuration(), Is.EqualTo(TimeSpan.Zero));
            });
        }

        [Test]
        public void Stop_ShouldNotCalculateDurationIfTimeAtPitStartIsZero()
        {
            // Arrange
            // Simulate a scenario where Start was called with TimeSpan.Zero, or it was reset
            _tracker.Start(TimeSpan.Zero);
            TimeSpan endTime = TimeSpan.FromSeconds(-10); // A non-sensical time, but the condition is based on _timeAtPitStart > TimeSpan.Zero

            // Act
            _tracker.Stop(endTime);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(_tracker.IsTrackingTime, Is.False);
                Assert.That(_tracker.GetPitDuration(), Is.EqualTo(TimeSpan.Zero));
                Assert.That(_tracker.GetAvgPitStopTime(), Is.EqualTo(TimeSpan.Zero));
            });
        }

        [Test]
        public void Stop_ShouldTrackMultiplePitStopDurations()
        {
            // Pit Stop 1: 50 seconds
            _tracker.Start(TimeSpan.FromSeconds(100));
            _tracker.Stop(TimeSpan.FromSeconds(50));
            Assert.That(_tracker.GetPitDuration(), Is.EqualTo(TimeSpan.FromSeconds(50)));

            // Pit Stop 2: 70 seconds
            _tracker.Start(TimeSpan.FromSeconds(200));
            _tracker.Stop(TimeSpan.FromSeconds(130));
            Assert.Multiple(() =>
            {
                Assert.That(_tracker.GetPitDuration(), Is.EqualTo(TimeSpan.FromSeconds(70)));

                // Assert average: (50 + 70) / 2 = 60 seconds
                Assert.That(_tracker.GetAvgPitStopTime(), Is.EqualTo(TimeSpan.FromSeconds(60)));
            });
        }

        [Test]
        public void GetAvgPitStopTime_ShouldReturnZeroIfNoStopsRecorded()
        {
            // Act
            TimeSpan result = _tracker.GetAvgPitStopTime();

            // Assert
            Assert.That(result, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void GetAvgPitStopTime_ShouldReturnCorrectAverage_OneStop()
        {
            // Arrange
            _tracker.Start(TimeSpan.FromSeconds(100));
            _tracker.Stop(TimeSpan.FromSeconds(70));
            TimeSpan expected = TimeSpan.FromSeconds(30);

            // Act
            TimeSpan result = _tracker.GetAvgPitStopTime();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetAvgPitStopTime_ShouldReturnCorrectAverage_MultipleStops()
        {
            // Arrange
            // Stop 1: 50s
            _tracker.Start(TimeSpan.FromSeconds(100));
            _tracker.Stop(TimeSpan.FromSeconds(50));

            // Stop 2: 70s
            _tracker.Start(TimeSpan.FromSeconds(200));
            _tracker.Stop(TimeSpan.FromSeconds(130));

            // Stop 3: 65s
            _tracker.Start(TimeSpan.FromSeconds(150));
            _tracker.Stop(TimeSpan.FromSeconds(85));

            // Total seconds: 50 + 70 + 65 = 185
            // Average seconds: 185 / 3 = 61.666...
            TimeSpan expected = TimeSpan.FromSeconds(185.0 / 3.0);

            // Act
            TimeSpan result = _tracker.GetAvgPitStopTime();

            // Assert
            Assert.That(result.TotalSeconds, Is.EqualTo(expected.TotalSeconds).Within(0.001));
        }

        [Test]
        public void Reset_ShouldClearTrackingTimeButKeepPitStopHistory()
        {
            // Arrange: Record one stop
            _tracker.Start(TimeSpan.FromSeconds(100));
            _tracker.Stop(TimeSpan.FromSeconds(50));

            // Start another one that's not completed
            _tracker.Start(TimeSpan.FromSeconds(200));

            Assert.Multiple(() =>
            {
                // Assert before reset
                Assert.That(_tracker.IsTrackingTime, Is.True);
                Assert.That(_tracker.GetAvgPitStopTime(), Is.Not.EqualTo(TimeSpan.Zero));
            });
            // _timeAtPitStart is non-zero (200s)

            // Act
            _tracker.Reset();

            // Assert after reset
            Assert.That(_tracker.IsTrackingTime, Is.False);
            // _timeAtPitStart should be zero, which we infer by checking a subsequent Stop call
            _tracker.Stop(TimeSpan.FromSeconds(150)); // This Stop should do nothing to _pitDuration

            Assert.Multiple(() =>
            {
                Assert.That(_tracker.GetPitDuration(), Is.EqualTo(TimeSpan.FromSeconds(50))); // Should hold the last completed duration
                                                                                              // Crucially, the history is NOT cleared by Reset()
                Assert.That(_tracker.GetAvgPitStopTime(), Is.EqualTo(TimeSpan.FromSeconds(50)));
            });
        }
    }
}
