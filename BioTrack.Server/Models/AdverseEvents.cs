using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioTrack.Server.Models
{
    public class AdverseEvents
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventID { get; set; }

        [Required]
        public int ParticipantID { get; set; }

        [ForeignKey(nameof(ParticipantID))]
        public Participants? Participant { get; set; } // optional nav

        [Required]
        public AdverseEventSeverity Severity { get; set; }

        [Required]
        public DateTime ReportedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// What action(s) were taken in response to the adverse event.
        /// </summary>
        [Required]
        [MaxLength(1000)]
        public string ActionTaken { get; set; } = string.Empty;

        /// <summary>
        /// Outcome of the adverse event (e.g., Recovered, Recovering, NotRecovered, Fatal, Unknown).
        /// You can switch this to an enum later if you prefer a constrained set.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Outcome { get; set; } = string.Empty;
    }

    public enum AdverseEventSeverity
    {
        Mild = 1,
        Moderate = 2,
        Severe = 3,
        LifeThreatening = 4
    }
}