namespace BioTrack.Server.DTOs
{
    public class CreateProtocolDto
    {

        public string Title { get; set; } = string.Empty;
        public int Phase { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal EnrollmentRate { get; set; }
        public decimal CompletionRate { get; set; }
        public string Status { get; set; } = "ACTIVE";
        public string Objectives { get; set; } = string.Empty;

        // Option A: assign existing investigator
        public int? LeadResearcherId { get; set; }

        // Option B: create new investigator
        public CreateResearcherDto? NewInvestigator { get; set; }

        // Optionally create/assign study sites immediately
        public List<StudySiteCreateDto>? StudySites
        {
            get; set;

        }
    }
    }
