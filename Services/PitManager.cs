namespace SharpOverlay.Services
{
    public class PitManager : IPitManager
    {
        private bool _isInService;
        private bool _hasBegunService;
        private bool _hasCompletedService;
        private bool _isOnPitRoad;

        public void Clear()
        {
            _isInService = false;
            _hasBegunService = false;
            _hasCompletedService = false;
            _isOnPitRoad = false;
        }

        public bool HasBegunService()
            => _hasBegunService;

        public bool HasFinishedService()
            => _hasCompletedService;
        public bool IsOnPitRoad()
            => _isOnPitRoad;

        public bool HasResetToPits(int enterExitResetButton)
            => enterExitResetButton == 0;

        public void UpdatePitRoadStatus(bool isOnPitRoad)
        {
            if (!_isOnPitRoad && isOnPitRoad)
            {
                _isOnPitRoad = true;
            }
            else if (_isOnPitRoad && !isOnPitRoad)
            {
                _isOnPitRoad = false;
            }
        }

        public void UpdatePitServiceStatus(bool isReceivingPitService)
        {
            if (isReceivingPitService && !_isInService)
            {
                _hasBegunService = true;
                _isInService = true;
            }
            else if (!isReceivingPitService && _isInService)
            {
                _isInService = false;
                _hasCompletedService = true;
            }
        }

        public void ResetBegunServiceStatus()
        {
            _hasBegunService = false;
        }

        public void ResetFinishedServiceStatus()
        {
            _hasCompletedService = false;
        }

        
    }
}
