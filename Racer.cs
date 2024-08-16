namespace SharpOverlay.Models
{
    public class Racer
    {
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
