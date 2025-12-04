using Core.Services.FuelCalculator.PitServices;

namespace Tests.Pit
{
    [TestFixture]
    public class PitTimeTests
    {
        private PitTimeTracker _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new PitTimeTracker();
        }

        // --- Start/Stop Pit Tracking Tests ---

        [Test]
        public void Start_SetsTimeAtPitStartAndIsTrackingTimeTrue()
        {
            var startTime = TimeSpan.FromMinutes(10);

            _sut.Start(startTime);

            Assert.That(_sut.IsTrackingTime, Is.True);
            // Cannot directly assert _timeAtPitStart as it's private, but it's used in Stop.
        }

        [Test]
        public void Stop_CalculatesPitDurationCorrectly_WithoutService()
        {
            var startTime = TimeSpan.FromMinutes(10);
            var stopTime = TimeSpan.FromMinutes(8);
            var expectedDuration = TimeSpan.FromMinutes(2); // 10 min - 8 min

            _sut.Start(startTime);
            _sut.Stop(stopTime);

            Assert.That(_sut.GetPitDuration(), Is.EqualTo(expectedDuration));
            Assert.That(_sut.IsTrackingTime, Is.False);
        }

        [Test]
        public void Stop_CalculatesPitDurationCorrectly_WithService()
        {
            var pitStartTime = TimeSpan.FromMinutes(10);
            var serviceStartTime = TimeSpan.FromMinutes(9.5);
            var serviceStopTime = TimeSpan.FromMinutes(9);
            var pitStopTime = TimeSpan.FromMinutes(8);

            var serviceDuration = TimeSpan.FromMinutes(0.5); // 9.5 min - 9 min
            var expectedPitDuration = TimeSpan.FromMinutes(1.5); // (10 min - 8 min) - 0.5 min

            _sut.Start(pitStartTime);

            _sut.StartService(serviceStartTime);
            _sut.StopService(serviceStopTime);

            _sut.Stop(pitStopTime);

            Assert.That(_sut.GetPitDuration(), Is.EqualTo(expectedPitDuration));
            Assert.That(_sut.IsTrackingTime, Is.False);
        }

        [Test]
        public void Stop_DoesNothing_IfPitWasNotStarted()
        {
            var stopTime = TimeSpan.FromMinutes(8);

            // State before calling Stop
            var initialDuration = _sut.GetPitDuration();
            var initialAvg = _sut.GetAvgPitStopTime();

            _sut.Stop(stopTime);

            Assert.That(_sut.GetPitDuration(), Is.EqualTo(initialDuration), "Pit duration should not change.");
            Assert.That(_sut.GetAvgPitStopTime(), Is.EqualTo(initialAvg), "Average pit time should not change.");
            Assert.That(_sut.IsTrackingTime, Is.False, "Tracking should remain false.");
        }

        // --- Pit Stop Average Tests ---

        [Test]
        public void GetAvgPitStopTime_ReturnsZero_WhenNoPitsRecorded()
        {
            Assert.That(_sut.GetAvgPitStopTime(), Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void GetAvgPitStopTime_ReturnsCorrectAverage_ForSinglePit()
        {
            var startTime = TimeSpan.FromSeconds(100);
            var stopTime = TimeSpan.FromSeconds(90);
            var expectedDuration = TimeSpan.FromSeconds(10);

            _sut.Start(startTime);
            _sut.Stop(stopTime);

            Assert.That(_sut.GetAvgPitStopTime(), Is.EqualTo(expectedDuration));
        }

        [Test]
        public void GetAvgPitStopTime_ReturnsCorrectAverage_ForMultiplePits()
        {
            // Pit 1: 10 seconds
            _sut.Start(TimeSpan.FromSeconds(100));
            _sut.Stop(TimeSpan.FromSeconds(90));

            // Pit 2: 20 seconds
            _sut.Start(TimeSpan.FromSeconds(50));
            _sut.Stop(TimeSpan.FromSeconds(30));

            // Average: (10 + 20) / 2 = 15 seconds
            var expectedAverage = TimeSpan.FromSeconds(15);

            Assert.That(_sut.GetAvgPitStopTime(), Is.EqualTo(expectedAverage));
        }

        [Test]
        public void GetAvgPitStopTime_HandlesServiceDurationInMultiplePits()
        {
            // Pit 1: Total time 10s. Service time 2s. Pit Duration 8s.
            _sut.Start(TimeSpan.FromSeconds(100));
            _sut.StartService(TimeSpan.FromSeconds(95));
            _sut.StopService(TimeSpan.FromSeconds(93)); // Service: 2s
            _sut.Stop(TimeSpan.FromSeconds(90));
            // Pit 1: (100-90) - 2 = 8s

            // Pit 2: Total time 20s. Service time 5s. Pit Duration 15s.
            _sut.Start(TimeSpan.FromSeconds(50));
            _sut.StartService(TimeSpan.FromSeconds(40));
            _sut.StopService(TimeSpan.FromSeconds(35)); // Service: 5s
            _sut.Stop(TimeSpan.FromSeconds(30));
            // Pit 2: (50-30) - 5 = 15s

            // Average: (8 + 15) / 2 = 11.5 seconds
            var expectedAverage = TimeSpan.FromSeconds(11.5);

            Assert.That(_sut.GetAvgPitStopTime().TotalSeconds, Is.EqualTo(expectedAverage.TotalSeconds));
        }

        // --- Service Tracking Tests ---

        [Test]
        public void StopService_CalculatesServiceDurationCorrectly()
        {
            var startTime = TimeSpan.FromMinutes(5);
            var stopTime = TimeSpan.FromMinutes(4.5);
            var expectedDuration = TimeSpan.FromMinutes(0.5);

            _sut.StartService(startTime);
            _sut.StopService(stopTime);

            Assert.That(_sut.GetServiceDuration(), Is.EqualTo(expectedDuration));
        }

        [Test]
        public void StopService_DoesNothing_IfServiceWasNotStarted()
        {
            // GetServiceDuration() will throw InvalidOperationException if _serviceDurations is empty.
            // We need to ensure StopService doesn't try to add a zero duration if StartService wasn't called.
            // The implementation's guard clause `if (_serviceStart > TimeSpan.Zero)` prevents this.
            var stopTime = TimeSpan.FromMinutes(4.5);
            _sut.StopService(stopTime);

            // To properly test this, we would need to know the state of _serviceDurations before StopService.
            // Since the class starts with no service durations, a successful test is one where
            // GetServiceDuration still throws (or returns Zero if we had recorded pits).
            // Since there is no pit recorded yet, GetServiceDuration will throw an exception.
            // Let's first record a service duration so we can reliably check the last one.
            _sut.StartService(TimeSpan.FromSeconds(100));
            _sut.StopService(TimeSpan.FromSeconds(90));
            var initialLastDuration = _sut.GetServiceDuration();

            // Now call StopService without StartService
            _sut.StopService(stopTime);

            // The last duration should still be the initial one (10 seconds)
            Assert.That(_sut.GetServiceDuration(), Is.EqualTo(initialLastDuration));
        }

        [Test]
        public void GetServiceDuration_NoException_IfNoServiceRecorded()
        {
            // The implementation uses .Last() on an internal list, which throws InvalidOperationException if the list is empty.
            Assert.That(() => _sut.GetServiceDuration(), Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void GetServiceDuration_ReturnsLastRecordedDuration()
        {
            // Service 1: 5s
            _sut.StartService(TimeSpan.FromSeconds(10));
            _sut.StopService(TimeSpan.FromSeconds(5));

            // Service 2: 1s
            _sut.StartService(TimeSpan.FromSeconds(3));
            _sut.StopService(TimeSpan.FromSeconds(2));

            var expectedLastDuration = TimeSpan.FromSeconds(1);

            Assert.That(_sut.GetServiceDuration(), Is.EqualTo(expectedLastDuration));
        }

        // --- Reset Tests ---

        [Test]
        public void Reset_SetsTimeAtPitStartAndServiceStartToZeroAndTrackingToFalse()
        {
            _sut.Start(TimeSpan.FromMinutes(10));
            _sut.StartService(TimeSpan.FromMinutes(9));

            _sut.Reset();

            Assert.That(_sut.IsTrackingTime, Is.False);
            // Cannot directly assert _timeAtPitStart and _serviceStart, but we can verify
            // that a subsequent Stop/StopService call does nothing (which implies they are Zero).

            // A subsequent Stop should not record a pit
            _sut.Stop(TimeSpan.FromMinutes(8));
            Assert.That(_sut.GetAvgPitStopTime(), Is.EqualTo(TimeSpan.Zero), "Pit Stop duration list should be empty or not added to.");

            // A subsequent StopService should not record a service (assuming no service was recorded before Reset)
            _sut.StopService(TimeSpan.FromMinutes(7));
            Assert.That(() => _sut.GetServiceDuration(), Is.EqualTo(TimeSpan.Zero), "Service duration should be zero.");
        }
    }
}
