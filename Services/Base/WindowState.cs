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

            UpdateIsOpen(isCarOnTrack);
        }

        public void Update(PropertyChangedEventArgs eventArgs)
        {
            string propertyName = eventArgs.PropertyName!;

            if (propertyName == nameof(IsEnabled))
            {
                IsEnabled = !IsEnabled;
                RaiseChange();
            }
            else if (propertyName == nameof(IsOpen))
            {
                IsOpen = !IsOpen;
                RaiseChange();
            }
            else if (propertyName == nameof(IsInTestMode))
            {
                IsInTestMode = !IsInTestMode;
                RaiseChange();
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
                IsEnabled = isInTestMode;
                RaiseChange();
            }
        }

        private void RaiseChange()
        {
            RequiresChange = true;
        }
    }
}