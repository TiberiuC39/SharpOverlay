using iRacingSdkWrapper;

namespace Core.Models
{
    public class SessionOutputDTO
    {
        public WeekendInfoDTO WeekendInfo { get; set; }

        public List<SessionDTO> Sessions { get; set; }

        public List<Driver> Drivers { get; set; }

        public PlayerDTO Player { get; set; }

        public List<SectorDTO> Sectors { get; set; }

        public List<QualifyResultDTO> QualifyResults { get; set; }

        public SessionOutputDTO(SessionInfo sessionInfo)
        {
            WeekendInfo = sessionInfo.WeekendInfo is not null ? new WeekendInfoDTO(sessionInfo.WeekendInfo) : new WeekendInfoDTO();
            Sessions = sessionInfo.Sessions is not null ? sessionInfo.Sessions.Select(s => new SessionDTO(s)).ToList() : new List<SessionDTO>();
            Drivers = sessionInfo.Drivers is not null ? sessionInfo.Drivers.Select(d => new Driver(d)).ToList() : new List<Driver>();
            Player = sessionInfo.Player is not null ? new PlayerDTO(sessionInfo.Player) : new PlayerDTO();
            Sectors = sessionInfo.Sectors is not null ? sessionInfo.Sectors.Select(s => new SectorDTO(s)).ToList() : new List<SectorDTO>();
            QualifyResults = sessionInfo.QualifyResults is not null ? sessionInfo.QualifyResults.Select(r => new QualifyResultDTO(r)).ToList() : new List<QualifyResultDTO>();
        }
    }
}
