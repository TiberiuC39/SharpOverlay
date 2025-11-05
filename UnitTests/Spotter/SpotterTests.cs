using Core.Events;
using Core.Models;
using Core.Services;
using Core.Services.Spotter;
using NSubstitute;

namespace Tests.Spotter
{
    [TestFixture]
    public class SpotterTests
    {
        private BarSpotterService _sut;
        private ISimReader _mockSimReader;
        private const double TrackLengthM = 5000.0;

        [SetUp]
        public void SetUp()
        {
            _mockSimReader = Substitute.For<ISimReader>();

            _sut = new BarSpotterService(_mockSimReader);
        }

        [TearDown]
        public void TearDown() {
            _sut.Dispose();
            _mockSimReader.Dispose();
        }

        [Test]
        public void OnTelemetry_ClosestCarAlongside_FiresEventWithOffsetZero()
        {
            // Arrange
            var argsFired = new List<BarSpotterEventArgs>();
            _sut.OnBarUpdated += (_, e) => argsFired.Add(e);

            _mockSimReader.OnSessionUpdated += Raise.EventWith(TestUtils.BarSpotter.CreateSessionEventArgs());

            var telemetryArgs = TestUtils.BarSpotter.CreateTelemetryEventArgs(
                playerDistPct: 0.50f,
                opponentDistPct: 0.50f,
                carLeftRight: Enums.Spotter.CarLeft
            );

            // Act
            _mockSimReader.OnTelemetryUpdated += Raise.EventWith(telemetryArgs);

            // Assert
            Assert.That(argsFired, Is.Not.Empty, "Event should have fired.");
            var eventArgs = argsFired[0];

            Assert.Multiple(() =>
            {
                Assert.That(eventArgs.OffsetPct, Is.EqualTo(0.00).Within(0.001), "Offset should be 0% (centered) when cars are at the same distance.");
                Assert.That(eventArgs.CarPos, Is.EqualTo(Enums.Spotter.CarLeft), "CarPos should match telemetry output.");
            });
        }

        [Test]
        public void OnTelemetry_ClosestCarAhead_FiresEventWithPositiveOffset()
        {
            // Arrange
            var argsFired = new List<BarSpotterEventArgs>();
            _sut.OnBarUpdated += (_, e) => argsFired.Add(e);

            _mockSimReader.OnSessionUpdated += Raise.EventWith(TestUtils.BarSpotter.CreateSessionEventArgs());

            var opponentAheadPct = 0.50f + (5.0 / TrackLengthM); // 0.501f
            var telemetryArgs = TestUtils.BarSpotter.CreateTelemetryEventArgs(
                playerDistPct: 0.50f,
                opponentDistPct: (float)opponentAheadPct
            );

            // Act
            _mockSimReader.OnTelemetryUpdated += Raise.EventWith(telemetryArgs);

            // Assert
            var eventArgs = argsFired[0];

            Assert.That(eventArgs.OffsetPct, Is.EqualTo(1.0).Within(0.01), "Offset should be 1.0 when exactly one car length ahead.");
        }

        [Test]
        public void OnTelemetry_ClosestCarBehind_FiresEventWithNegativeOffset()
        {
            // Arrange
            var argsFired = new List<BarSpotterEventArgs>();
            _sut.OnBarUpdated += (_, e) => argsFired.Add(e);

            _mockSimReader.OnSessionUpdated += Raise.EventWith(TestUtils.BarSpotter.CreateSessionEventArgs());

            var opponentBehindPct = 0.50f - (2.5 / TrackLengthM); // 0.4995f
            var telemetryArgs = TestUtils.BarSpotter.CreateTelemetryEventArgs(
                playerDistPct: 0.50f,
                opponentDistPct: (float)opponentBehindPct
            );

            // Act
            _mockSimReader.OnTelemetryUpdated += Raise.EventWith(telemetryArgs);

            // Assert
            var eventArgs = argsFired[0];
            Assert.That(eventArgs.OffsetPct, Is.EqualTo(-0.5).Within(0.01), "Offset should be -0.05 when half a car length behind.");

        }

        [Test]
        public void OnTelemetry_ClosestCarTooFar_FiresEventWithOutOfFrameOffset()
        {
            // Arrange
            var argsFired = new List<BarSpotterEventArgs>();
            _sut.OnBarUpdated += (_, e) => argsFired.Add(e);
            _mockSimReader.OnSessionUpdated += Raise.EventWith(TestUtils.BarSpotter.CreateSessionEventArgs());

            var opponentFarAheadPct = 0.50f + (6.0 / TrackLengthM); // 0.5012f
            var telemetryArgs = TestUtils.BarSpotter.CreateTelemetryEventArgs(
                playerDistPct: 0.50f,
                opponentDistPct: (float)opponentFarAheadPct
            );

            // Act
            _mockSimReader.OnTelemetryUpdated += Raise.EventWith(telemetryArgs);

            // Assert
            var eventArgs = argsFired[0];

            Assert.That(eventArgs.OffsetPct, Is.AtLeast(1.0).Within(0.01), "Offset should be 1.0 (out of frame) when distance is too large.");
        }

        [Test]
        public void OnTelemetry_AroundFinishLine_CorrectlyHandlesWrapAroundDistance()
        {
            // Arrange
            var argsFired = new List<BarSpotterEventArgs>();
            _sut.OnBarUpdated += (_, e) => argsFired.Add(e);

            _mockSimReader.OnSessionUpdated += Raise.EventWith(TestUtils.BarSpotter.CreateSessionEventArgs());

            var telemetryArgs = TestUtils.BarSpotter.CreateTelemetryEventArgs(
                playerDistPct: 0.998f, // 10m before line
                opponentDistPct: 0.001f // 5m after line
            );

            // Act
            _mockSimReader.OnTelemetryUpdated += Raise.EventWith(telemetryArgs);

            // Assert
            var eventArgs = argsFired[0];
            Assert.That(eventArgs.OffsetPct, Is.AtLeast(1.0).Within(0.01), "Offset should be > 1.0 (out of frame) when car is far ahead over the line.");
        }

        [Test]
        public void OnTelemetry_AroundFinishLine_CorrectlyHandlesWrapAroundDistance_HalfOverlap()
        {
            // Arrange
            var argsFired = new List<BarSpotterEventArgs>();
            _sut.OnBarUpdated += (_, e) => argsFired.Add(e);

            // 1. Setup Session Info (Track Length = 5000m)
            _mockSimReader.OnSessionUpdated += Raise.EventWith(TestUtils.BarSpotter.CreateSessionEventArgs());

            var telemetryArgs = TestUtils.BarSpotter.CreateTelemetryEventArgs(
                playerDistPct: 0.9995f, // 2.5m before line
                opponentDistPct: 0.000f // 0m after line
            );

            // Act
            _mockSimReader.OnTelemetryUpdated += Raise.EventWith(telemetryArgs);

            // Assert
            var eventArgs = argsFired[0];
            Assert.That(eventArgs.OffsetPct, Is.EqualTo(0.50).Within(0.01), "Offset should be 50.0 when player car is behind over the line.");
        }

        [Test]
        public void OnTelemetry_AroundFinishLine_CorrectlyHandlesWrapAroundDistance_HalfOverlap_PlayerAhead()
        {
            // Arrange
            var argsFired = new List<BarSpotterEventArgs>();
            _sut.OnBarUpdated += (_, e) => argsFired.Add(e);

            // 1. Setup Session Info (Track Length = 5000m)
            _mockSimReader.OnSessionUpdated += Raise.EventWith(TestUtils.BarSpotter.CreateSessionEventArgs());

            var telemetryArgs = TestUtils.BarSpotter.CreateTelemetryEventArgs(
                playerDistPct: 0.0000f, // 0m after line
                opponentDistPct: 0.9995f // 2.5m before line
            );

            // Act
            _mockSimReader.OnTelemetryUpdated += Raise.EventWith(telemetryArgs);

            // Assert
            var eventArgs = argsFired[0];
            Assert.That(eventArgs.OffsetPct, Is.EqualTo(-0.50).Within(0.01), "Offset should be -50.0 when player car is ahead over the line.");
        }

        [Test]
        public void OnTelemetry_AroundFinishLine_CorrectlyHandlesWrapAroundDistance_HalfOverlap_PlayerAhead_FurtherAwayFromLine()
        {
            // Arrange
            var argsFired = new List<BarSpotterEventArgs>();
            _sut.OnBarUpdated += (_, e) => argsFired.Add(e);

            // 1. Setup Session Info (Track Length = 5000m)
            _mockSimReader.OnSessionUpdated += Raise.EventWith(TestUtils.BarSpotter.CreateSessionEventArgs());

            var telemetryArgs = TestUtils.BarSpotter.CreateTelemetryEventArgs(
                playerDistPct: 0.0002f, // 1m after line
                opponentDistPct: 0.9997f // 1.5m before line
            );

            // Act
            _mockSimReader.OnTelemetryUpdated += Raise.EventWith(telemetryArgs);

            // Assert
            var eventArgs = argsFired[0];
            Assert.That(eventArgs.OffsetPct, Is.EqualTo(-0.50).Within(0.01), "Offset should be -0.50 when player car is ahead over the line.");
        }

        [Test]
        public void OnTelemetry_AroundFinishLine_CorrectlyHandlesWrapAroundDistance_NotCrossedLine()
        {
            // Arrange
            var argsFired = new List<BarSpotterEventArgs>();
            _sut.OnBarUpdated += (_, e) => argsFired.Add(e);

            // 1. Setup Session Info (Track Length = 5000m)
            _mockSimReader.OnSessionUpdated += Raise.EventWith(TestUtils.BarSpotter.CreateSessionEventArgs());

            var telemetryArgs = TestUtils.BarSpotter.CreateTelemetryEventArgs(
                playerDistPct: 0.9998f, // 1m before line
                opponentDistPct: 0.9997f // 1.5m before line
            );

            // Act
            _mockSimReader.OnTelemetryUpdated += Raise.EventWith(telemetryArgs);

            // Assert
            var eventArgs = argsFired[0];
            Assert.That(eventArgs.OffsetPct, Is.EqualTo(-0.10).Within(0.01), "Offset should be -0.50 when player car is ahead over the line.");
        }
    }
}
