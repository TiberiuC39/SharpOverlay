using System.ComponentModel;
using Presentation.Models;

namespace Presentation.Services
{
    public class TrackedWindowState
    {
        public TrackedWindowState(BaseSettings settings)
        {
            UpdateIsOpen(settings.IsOpen);
            UpdateIsEnabled(settings.IsEnabled);
            UpdateIsInTestMode(settings.IsInTestMode);
            UpdateIsInDebugMode(false);
        }

        public bool IsOpen { get; private set; }
        public bool IsEnabled { get; private set; }
        public bool IsInTestMode { get; private set; }
        public bool IsInDebugMode { get; private set; }

        public bool RequiresChange { get; private set; }

        public void Update(bool isCarOnTrack)
        {
            if (!IsInTestMode)
            {
                UpdateIsOpen(isCarOnTrack);
            }
        }

        public void Update(PropertyChangedEventArgs eventArgs)
        {
            string propertyName = eventArgs.PropertyName!;

            if (propertyName == nameof(IsEnabled))
            {
                UpdateIsEnabled(!IsEnabled);
            }
            else if (propertyName == nameof(IsOpen))
            {
                UpdateIsOpen(!IsOpen);
            }
            else if (propertyName == nameof(IsInTestMode))
            {
                UpdateIsInTestMode(!IsInTestMode);
            }
            else if (propertyName == nameof(IsInDebugMode))
            {
                UpdateIsInDebugMode(!IsInDebugMode);
            }
        }

        public void CompleteChange()
        {
            RequiresChange = false;
        }

        private void UpdateIsOpen(bool isOpen)
        {
            if (IsOpen == !isOpen)
            {
                IsOpen = isOpen;
                RaiseChange();
            }
        }

        private void UpdateIsEnabled(bool isEnabled)
        {
            if (IsEnabled == !isEnabled)
            {
                IsEnabled = isEnabled;
                RaiseChange();
            }
        }

        private void UpdateIsInTestMode(bool isInTestMode)
        {
            if (IsInTestMode == !isInTestMode)
            {
                IsInTestMode = isInTestMode;
                RaiseChange();
            }
        }

        private void UpdateIsInDebugMode(bool isInDebugMode)
        {
            if (IsInDebugMode == !isInDebugMode)
            {
                IsInDebugMode = isInDebugMode;
                RaiseChange();
            }
        }

        private void RaiseChange()
        {
            RequiresChange = true;
        }
    }
}
