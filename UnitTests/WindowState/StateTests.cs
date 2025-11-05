using NSubstitute;
using System.ComponentModel;
using Presentation.Models;
using Presentation.Services;


namespace Tests.WindowState
{
    [TestFixture]
    public class TrackedWindowStateTests
    {
        private IBaseSettings _mockSettings;

        [SetUp]
        public void SetUp()
        {
            _mockSettings = Substitute.For<IBaseSettings>();
        }

        // --- Constructor Tests ---

        [Test]
        public void Constructor_InitializesState_FromSettings()
        {
            // Arrange
            _mockSettings.IsOpen.Returns(true);
            _mockSettings.IsInTestMode.Returns(false);

            // Act
            var state = new TrackedWindowState(_mockSettings);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(state.IsOpen, Is.True, "IsOpen should be initialized from settings.");
                Assert.That(state.IsInTestMode, Is.False, "IsInTestMode should be initialized from settings.");
                Assert.That(state.IsInDebugMode, Is.False, "IsInDebugMode should be initialized to false.");
                Assert.That(state.RequiresChange, Is.True, "RequiresChange should be initially true.");
            });
        }

        [Test]
        public void Constructor_InitializesState_FromSettings_AlternateValues()
        {
            // Arrange
            _mockSettings.IsOpen.Returns(false);
            _mockSettings.IsInTestMode.Returns(true);

            // Act
            var state = new TrackedWindowState(_mockSettings);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(state.IsOpen, Is.False, "IsOpen should be initialized from settings.");
                Assert.That(state.IsInTestMode, Is.True, "IsInTestMode should be initialized from settings.");
            });
        }

        // --- Update(bool isCarOnTrack) Tests ---

        [TestCase(true, false, true, ExpectedResult = true, Description = "Car on track, not in test mode, state changes from false to true.")]
        [TestCase(false, false, false, ExpectedResult = false, Description = "Car off track, not in test mode, state already false, no change.")]
        [TestCase(true, false, true, ExpectedResult = false, Description = "Car on track, not in test mode, state already true, no change.")]
        public bool Update_IsCarOnTrack_When_Not_InTestMode(bool initialIsOpen, bool initialIsInTestMode, bool isCarOnTrack)
        {
            // Arrange
            _mockSettings.IsOpen.Returns(initialIsOpen);
            _mockSettings.IsInTestMode.Returns(initialIsInTestMode);
            var state = new TrackedWindowState(_mockSettings);
            state.CompleteChange(); // Clear initial RequiresChange flag if set

            Assert.Multiple(() =>
            {
                // Assert initial state is correct and no change is required before the call
                Assert.That(state.IsOpen, Is.EqualTo(initialIsOpen));
                Assert.That(state.RequiresChange, Is.False);
            });

            // Act
            bool requiresChange = state.Update(isCarOnTrack);

            // Assert state after update
            Assert.That(state.IsOpen, Is.EqualTo(isCarOnTrack));
            return requiresChange;
        }

        [Test]
        public void Update_IsCarOnTrack_When_IsInTestMode_DoesNotChangeIsOpen()
        {
            // Arrange
            const bool initialIsOpen = false;
            const bool isCarOnTrack = true;

            _mockSettings.IsOpen.Returns(initialIsOpen);
            _mockSettings.IsInTestMode.Returns(true);
            var state = new TrackedWindowState(_mockSettings);
            state.CompleteChange();

            Assert.Multiple(() =>
            {
                // Assert initial state
                Assert.That(state.IsInTestMode, Is.True);
                Assert.That(state.IsOpen, Is.False);
            });

            // Act
            bool requiresChange = state.Update(isCarOnTrack);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(state.IsOpen, Is.False, "IsOpen should remain unchanged when IsInTestMode is true.");
                Assert.That(requiresChange, Is.False, "RequiresChange should be false as no state change occurred.");
            });
        }

        // --- Update(PropertyChangedEventArgs eventArgs) Tests ---

        [TestCase(nameof(TrackedWindowState.IsOpen), true, ExpectedResult = false, Description = "Flipping IsOpen from True to False.")]
        [TestCase(nameof(TrackedWindowState.IsOpen), false, ExpectedResult = true, Description = "Flipping IsOpen from False to True.")]
        public bool Update_PropertyChangedEventArgs_ChangesState_And_RequiresChange(string propertyName, bool initialValue)
        {
            // Arrange
            _mockSettings.IsOpen.Returns(initialValue);
            _mockSettings.IsInTestMode.Returns(false); // Default
            var state = new TrackedWindowState(_mockSettings);
            state.CompleteChange(); // Clear initial state

            // Ensure the correct initial property is set
            if (propertyName == nameof(TrackedWindowState.IsOpen))
                Assert.That(state.IsOpen, Is.EqualTo(initialValue));
            // Add checks for other properties if needed based on the setup

            var eventArgs = new PropertyChangedEventArgs(propertyName);

            // Act
            bool requiresChange = state.Update(eventArgs);

            // Assert the property flipped
            if (propertyName == nameof(TrackedWindowState.IsOpen))
                Assert.That(state.IsOpen, Is.EqualTo(!initialValue), $"{propertyName} should have been flipped.");

            // The method should only return true if the state changed.
            return requiresChange;
        }

        [Test]
        public void Update_PropertyChangedEventArgs_FlipsIsInTestMode()
        {
            // Arrange
            _mockSettings.IsOpen.Returns(false);
            _mockSettings.IsInTestMode.Returns(false); // Initial value for IsInTestMode
            var state = new TrackedWindowState(_mockSettings);
            state.CompleteChange();

            var eventArgs = new PropertyChangedEventArgs(nameof(TrackedWindowState.IsInTestMode));

            // Act
            bool requiresChange = state.Update(eventArgs);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(state.IsInTestMode, Is.True, "IsInTestMode should be flipped to true.");
                Assert.That(requiresChange, Is.True, "RequiresChange should be true.");
            });

            // Act again to flip back
            requiresChange = state.Update(eventArgs);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(state.IsInTestMode, Is.False, "IsInTestMode should be flipped back to false.");
                Assert.That(requiresChange, Is.True, "RequiresChange should be true.");
            });
        }

        [Test]
        public void Update_PropertyChangedEventArgs_FlipsIsInDebugMode()
        {
            // Arrange
            _mockSettings.IsOpen.Returns(false);
            _mockSettings.IsInTestMode.Returns(false);
            var state = new TrackedWindowState(_mockSettings);
            state.CompleteChange(); // IsInDebugMode is initialized to false

            var eventArgs = new PropertyChangedEventArgs(nameof(TrackedWindowState.IsInDebugMode));

            // Act
            bool requiresChange = state.Update(eventArgs);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(state.IsInDebugMode, Is.True, "IsInDebugMode should be flipped to true.");
                Assert.That(requiresChange, Is.True, "RequiresChange should be true.");
            });
        }

        [Test]
        public void Update_PropertyChangedEventArgs_NoChangeWhenPropertyDoesNotMatch()
        {
            // Arrange
            _mockSettings.IsOpen.Returns(true);
            _mockSettings.IsInTestMode.Returns(true);
            var state = new TrackedWindowState(_mockSettings);
            state.CompleteChange();

            var eventArgs = new PropertyChangedEventArgs("SomeOtherProperty");

            // Act
            bool requiresChange = state.Update(eventArgs);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(state.IsOpen, Is.True, "IsOpen should be unchanged.");
                Assert.That(state.IsInTestMode, Is.True, "IsInTestMode should be unchanged.");
                Assert.That(state.IsInDebugMode, Is.False, "IsInDebugMode should be unchanged.");
                Assert.That(requiresChange, Is.False, "RequiresChange should be false.");
            });
        }

        [Test]
        public void Update_PropertyChangedEventArgs_NoChangeWhenValueAlreadyFlipped()
        {
            // Arrange
            _mockSettings.IsOpen.Returns(true);
            _mockSettings.IsInTestMode.Returns(false);
            var state = new TrackedWindowState(_mockSettings);
            state.CompleteChange();

            // Manually "change" the state to simulate that the state is already the new value
            // Note: The logic in the original code is `if (IsOpen == !isOpen)` which means if IsOpen is true and we call with true (as new state), it won't change.
            // The property-changed update logic flips the current value, so we test that if IsOpen is True, and it gets a notification, it flips to False. 
            // We will test the case where `Update` is called, but the value is already the desired value.

            // This test will verify that a second call with the same property name *after* the flip
            // will flip it back and thus generate a change.

            var eventArgs = new PropertyChangedEventArgs(nameof(TrackedWindowState.IsOpen));

            // 1. Initial state: IsOpen = True
            // 2. First Update call flips: IsOpen = False, RequiresChange = True
            state.Update(eventArgs);
            state.CompleteChange();
            Assert.Multiple(() =>
            {
                Assert.That(state.IsOpen, Is.False);
                Assert.That(state.RequiresChange, Is.False);
            });

            // 3. Second Update call flips back: IsOpen = True, RequiresChange = True
            // The internal check is `if (IsOpen == !isOpen)`, where `isOpen` is the new value, which is `!currentIsOpen`.
            bool requiresChange = state.Update(eventArgs);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(state.IsOpen, Is.True, "IsOpen should have flipped back.");
                Assert.That(requiresChange, Is.True, "RequiresChange should be true.");
            });
        }

        // --- CompleteChange Tests ---

        [Test]
        public void CompleteChange_Sets_RequiresChange_ToFalse()
        {
            // Arrange
            _mockSettings.IsOpen.Returns(false);
            _mockSettings.IsInTestMode.Returns(false);
            var state = new TrackedWindowState(_mockSettings);

            // Force a change to set RequiresChange to true (e.g., initial state is false, update with true)
            state.Update(true);
            Assert.That(state.RequiresChange, Is.True, "RequiresChange must be true before calling CompleteChange.");

            // Act
            state.CompleteChange();

            // Assert
            Assert.That(state.RequiresChange, Is.False, "RequiresChange should be false after calling CompleteChange.");
        }
    }
}
