using System;
using Windows.ApplicationModel.VoiceCommands;

namespace SharpOverlay.Services.FuelServices
{
    public class SetupChangeTracker
    {
        private string _setupName = string.Empty;

        public bool IsSetupChanged { get; private set; }

        public void UpdateSetupName(string newSetupName)
        {
            if (CheckIfDifferent(newSetupName))
            {
                _setupName = newSetupName;
            }            
        }

        private bool CheckIfDifferent(string newSetupName)
        {
            if (_setupName != newSetupName)
            {
                IsSetupChanged = true;

                return true;
            }

            return false;
        }

        public void Reset()
        {
            IsSetupChanged = false;
        }
    }
}
