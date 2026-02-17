namespace BioTrack.Server.DTOs
{
    public class StudySiteReadDto
    {

        public int SiteID { get; set; }
        public int? ProtocolID { get; set; }
        public string Location { get; set; } = string.Empty;
        //public string InvestigatorName { get; set; }
        //public ResearcherMiniDto PrincipalInvestigator { get; set; }

        // Optional: Include protocol summary
        //public string ProtocolName { get; set; }

        // Optional: Minimal participant info
        //public List<ParticipantDto> Participants { get; set; } = new();

    }
}
