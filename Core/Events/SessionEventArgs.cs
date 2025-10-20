using Core.Models;
using iRacingSdkWrapper;

namespace Core.Events
{
    public class SessionEventArgs : EventArgs
    {
        public SessionEventArgs(SessionInfo sessionInfo)
        {
            SessionOutput = new SessionOutputDTO(sessionInfo);
        }

        public SessionOutputDTO SessionOutput { get; }
    }
}
