using iRacingSdkWrapper;
using SharpOverlay.Models;
using System.ComponentModel;

namespace SharpOverlay.Services.Base
{
    public class WindowState
    {
        public WindowState(BaseSettings settings)
        {
            UpdateIsOpen(settings.IsOpen);
            UpdateIsEnabled(settings.IsEnabled);
            UpdateIsInTestMode(settings.IsInTestMode);
        }

        public bool IsOpen { get; private set; }
        public bool IsEnabled { get; private set; }
        public bool IsInTestMode { get; private set; }

        public bool RequiresChange { get; private set; }

        public void Update(SdkWrapper.TelemetryUpdatedEventArgs eventArgs)
        {
            bool isCarOnTrack = eventArgs.TelemetryInfo.IsOnTrack.Value;

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

        private void RaiseChange()
        {
            RequiresChange = true;
        }
    }
}