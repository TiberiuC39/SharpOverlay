using iRacingSdkWrapper;

namespace SharpOverlay.Models
{
    public class Racer
    {
        public Racer(YamlQuery yaml)
        {
            UserID = int.Parse(yaml["UserID"].Value);
            UserName = yaml["UserName"].Value;
            AbbrevName = yaml["AbbrevName"].Value;
            Initials = yaml["Initials"].Value;
            TeamName = yaml["TeamName"].Value;
            TeamID = int.Parse(yaml["TeamID"].Value);
            CarIdx = int.Parse(yaml["CarIdx"].Value);
            IRating = int.Parse(yaml["IRating"].Value);
            LicLevel = int.Parse(yaml["LicLevel"].Value);
            LicSubLevel = int.Parse(yaml["LicSubLevel"].Value);
            LicString = yaml["LicString"].Value;
            LicColor = yaml["LicColor"].Value;
            IsSpectator = int.Parse(yaml["IsSpectator"].Value);
            CarDesignStr = yaml["CarDesignStr"].Value;
            HelmetDesignStr = yaml["HelmetDesignStr"].Value;
            SuitDesignStr = yaml["SuitDesignStr"].Value;
            BodyType = int.Parse(yaml["BodyType"].Value);
            FaceType = int.Parse(yaml["FaceType"].Value);
            HelmetType = int.Parse(yaml["HelmetType"].Value);
            CarNumberDesignStr = yaml["CarNumberDesignStr"].Value;
            CarSponsor_1 = int.Parse(yaml["CarSponsor_1"].Value);
            CarSponsor_2 = int.Parse(yaml["CarSponsor_2"].Value);
            yaml["ClubName"].TryGetValue(out string clubName);
            ClubName = clubName ?? string.Empty;

            //ClubID = int.Parse(yaml["ClubID"].Value);
            //DivisionName = yaml["DivisionName"].Value;                <=== These dont show in Test Mode add a check down the line?
            //DivisionID = int.Parse(yaml["DivisionID"].Value);
            CurDriverIncidentCount = int.Parse(yaml["CurDriverIncidentCount"].Value);
            TeamIncidentCount = int.Parse(yaml["TeamIncidentCount"].Value);
        }

        public int CarIdx { get; set; }
        public string UserName { get; set; }
        public string AbbrevName { get; set; }
        public string Initials { get; set; }
        public int UserID { get; set; }
        public int TeamID { get; set; }
        public string TeamName { get; set; }
        public int IRating { get; set; }
        public int LicLevel { get; set; }
        public int LicSubLevel { get; set; }
        public string LicString { get; set; }
        public string LicColor { get; set; }
        public int IsSpectator { get; set; }
        public string CarDesignStr { get; set; }
        public string HelmetDesignStr { get; set; }
        public string SuitDesignStr { get; set; }
        public int BodyType { get; set; }
        public int FaceType { get; set; }
        public int HelmetType { get; set; }
        public string CarNumberDesignStr { get; set; }
        public int CarSponsor_1 { get; set; }
        public int CarSponsor_2 { get; set; }
        public string ClubName { get; set; }
        public int ClubID { get; set; }
        public string DivisionName { get; set; }
        public int DivisionID { get; set; }
        public int CurDriverIncidentCount { get; set; }
        public int TeamIncidentCount { get; set; }
    }
}
