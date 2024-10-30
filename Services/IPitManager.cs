namespace SharpOverlay.Services
{
    public interface IPitManager : IClear
    {
        void UpdatePitServiceStatus(bool isReceivingPitService);
        void ResetBegunServiceStatus();
        void ResetFinishedServiceStatus();
        void UpdatePitRoadStatus(bool isOnPitRoad);
        bool HasBegunService();
        bool HasFinishedService();
        bool HasResetToPits(int enterExitResetButton);
    }
}
