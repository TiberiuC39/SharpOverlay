using iRacingSdkWrapper;

namespace SharpOverlay.Services.FuelServices.PitServices
{
    public class PitManager : IClear
    {
        private bool _isInService;
        private bool _hasBegunService;
        private bool _hasCompletedService;
        private bool _hasEnteredPits;
        private bool _isOnPitRoad;
        private bool _isComingOutOfPits;

        public void Clear()
        {
            _isInService = false;
            _hasBegunService = false;
            _hasCompletedService = false;
            _hasEnteredPits = false;
            _isOnPitRoad = false;
            _isComingOutOfPits = false;
        }

        public bool HasBegunService()
            => _hasBegunService;

        public bool HasFinishedService()
            => _hasCompletedService;
        public bool IsOnPitRoad()
            => _isOnPitRoad;

        public bool HasEnteredPits()
            => _hasEnteredPits;

        public bool HasResetToPits(int enterExitResetButton)
            => enterExitResetButton == 1 && !_hasEnteredPits;

        public void SetPitRoadStatus(bool isOnPitRoad, TrackSurfaces trackSurface)
        {
            if (!_hasEnteredPits && trackSurface == TrackSurfaces.AproachingPits)
            {
                _hasEnteredPits = true;
            }
            else if (!_isOnPitRoad && isOnPitRoad)
            {
                _isOnPitRoad = true;
            }
            else if (_isOnPitRoad && !isOnPitRoad && trackSurface != TrackSurfaces.InPitStall)
            {
                _isOnPitRoad = false;
                _hasEnteredPits = false;
                _isComingOutOfPits = true;
            }
        }

        public void SetPitServiceStatus(bool isReceivingPitService)
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

        public bool IsComingOutOfPits()
            => _isComingOutOfPits;

        public void ResetIsComingOutOfPits()
        {
            _isComingOutOfPits = false;
        }
    }
}
