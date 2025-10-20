using iRacingSdkWrapper;

namespace Core.Services.FuelCalculator.PitServices
{
    public class PitManager : IClear
    {
        private bool _hasBegunService;
        private bool _hasCompletedService;
        private bool _hasEnteredPits;
        private bool _isOnPitRoad;
        private bool _isComingOutOfPits;

        public void Clear()
        {
            _hasBegunService = false;
            _hasCompletedService = false;
            _hasEnteredPits = false;
            _isOnPitRoad = false;
            _isComingOutOfPits = false;
            HasResetToPits = false;
        }

        public bool HasBegunService()
            => _hasBegunService;

        public bool HasFinishedService()
            => _hasCompletedService;
        public bool IsOnPitRoad()
            => _isOnPitRoad;

        public bool HasEnteredPits()
            => _hasEnteredPits;

        public bool IsResettingToPits(int enterExitResetButton)
            => enterExitResetButton == 1 && !_hasEnteredPits && !_hasCompletedService;

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
            else if (_isOnPitRoad && !isOnPitRoad && (trackSurface != TrackSurfaces.InPitStall || trackSurface != TrackSurfaces.AproachingPits))
            {
                _isOnPitRoad = false;
                _hasEnteredPits = false;
                _isComingOutOfPits = true;
                HasResetToPits = false;
            }
        }

        public void SetPitServiceStatus(bool isReceivingPitService)
        {
            if (isReceivingPitService && !_hasBegunService)
            {
                _hasBegunService = true;
                _hasCompletedService = false;
            }
            else if (!isReceivingPitService && _hasBegunService)
            {
                _hasBegunService = false;
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

        public bool HasResetToPits { get; set; }
    }
}
