using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioTrack.Server.Models
{
    public class AdverseEvents
    {
        public class AdverseEvent
        {
            [Key]
            public int EventID { get; set; }

            [Required]
            public int ParticipantID { get; set; }

            //// Linking the event back to the specific participant
            //[ForeignKey("ParticipantID")]
            //public virtual Participant? Participant { get; set; }

            [Required]
            [MaxLength(2000)] // Standard clinical notes length limit
            public string Description { get; set; } = string.Empty;

            [Required]
            public AdverseEventSeverity Severity { get; set; }

            [Required]
            public DateTime ReportedDate { get; set; } = DateTime.UtcNow;

            // Common addition for BioTrack compliance: 
            // Whether this was reported to the regulatory body
            public bool IsRegulatoryReported { get; set; } = false;
        }

        public enum AdverseEventSeverity
        {
            Mild = 1,      // Awareness of signs/symptoms, but easily tolerated
            Moderate = 2,  // Enough discomfort to interfere with usual activity
            Severe = 3,    // Incapacitating; unable to work or perform usual activity
            LifeThreatening = 4
        }
    }
}
