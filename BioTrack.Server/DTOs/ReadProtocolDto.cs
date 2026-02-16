namespace BioTrack.Server.DTOs
{
    public class ReadProtocolDto
    {

        public int ProtocolID { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Phase { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal EnrollmentRate { get; set; }
        public decimal CompletionRate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Objectives { get; set; } = string.Empty;

        public ResearcherMiniDto? LeadResearcher { get; set; }

        // Related counts & IDs for client-side navigation
        public int StudySiteCount { get; set; }
        public int ParticipantCount { get; set; }
        public int ComplianceReportCount { get; set; }
        public int TrialReportCount { get; set; }
        public int ProtocolDeviationCount { get; set; }

        public List<int> StudySiteIds { get; set; } = new();
        public List<int> ParticipantIds { get; set; } = new();
        public List<int> ComplianceReportIds { get; set; } = new();
        public List<int> TrialReportIds { get; set; } = new();
        public List<int> ProtocolDeviationIds { get; set; } = new();

    }
}
