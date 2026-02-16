using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioTrack.Server.Models
{
    // You can rename the table if you prefer a different name
    //[Table("ProtocolDeviations")]
    public class ProtocolDeviation
    {
        [Key]
        public int DeviationId { get; set; }

        // --- Foreign Keys ---
        [Required]
        public int ProtocolID { get; set; }

        [ForeignKey(nameof(ProtocolID))]
        public TrialProtocols TrialProtocol { get; set; }

        [Required]
        public int ParticipantID { get; set; }

        [ForeignKey(nameof(ParticipantID))]
        public Participants Participant { get; set; }

        // Observation link is often optional (not all deviations arise from a recorded observation)
        public int? ObservationID { get; set; }

        [ForeignKey(nameof(ObservationID))]
        public Observations Observation { get; set; }

        // --- Deviation Details ---
        [Required, MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public SeverityLevel Severity { get; set; } // Minor, Major, Critical

        //[Required, MaxLength(150)]
        //public string DetectedBy { get; set; } // free-text (could be a researcher name or staff id)

        [Required]
        public DateTime ReportedDate { get; set; } // store in UTC
    }

    public enum SeverityLevel
    {
        Minor = 0,
        Major = 1,
        Critical = 2
    }
}