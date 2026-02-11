using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioTrack.Server.Models
{
    public class AdverseEvents
    {
        [Key]
        public int EventID { get; set; }

        [Required]
        public int ParticipantID { get; set; }

        [ForeignKey(nameof(ParticipantID))]
        public Participants Participant { get; set; }

        [Required, MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public AdverseEventSeverity Severity { get; set; }

        [Required]
        public DateTime ReportedDate { get; set; } = DateTime.UtcNow;

        public bool IsRegulatoryReported { get; set; } = false;
    }

    public enum AdverseEventSeverity
    {
        Mild = 1,
        Moderate = 2,
        Severe = 3,
        LifeThreatening = 4
    }
}