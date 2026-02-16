using BioTrack.Server.Models;
using System.ComponentModel.DataAnnotations;

namespace BioTrack.Server.DTOs
{
    public class AdverseEventsCreateDto
    {

        [Required] public int ParticipantID { get; set; }
        [Required] public AdverseEventSeverity Severity { get; set; }
        public DateTime? ReportedDate { get; set; } // optional; defaulted if null
        [Required, MaxLength(1000)] public string ActionTaken { get; set; } = string.Empty;
        [Required, MaxLength(200)]
        public string Outcome { get; set; } = string.Empty;

    }
}
