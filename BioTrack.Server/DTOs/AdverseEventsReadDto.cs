using BioTrack.Server.Models;

namespace BioTrack.Server.DTOs
{
    public class AdverseEventsReadDto
    {

        public int EventID { get; set; }
        public int ParticipantID { get; set; }
        public AdverseEventSeverity Severity { get; set; }
        public DateTime ReportedDate { get; set; }
        public string ActionTaken { get; set; } = string.Empty;
        public string Outcome { get; set; } = string.Empty;

    }
}
