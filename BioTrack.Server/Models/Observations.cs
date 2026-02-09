using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioTrack.Server.Models
{
    public class Observations
    {
        [Key]
        public int ObservationID { get; set; }

        [Required]
        public int ParticipantID { get; set; }

        // Navigation property to link back to the Participant
        //[ForeignKey("ParticipantID")]
        //public virtual Participant? Participant { get; set; }

        [Required]
        public DateTime VisitDate { get; set; }

        // --- DataPoints: Vitals ---
        public decimal BodyTemperature { get; set; }
        public int HeartRate { get; set; }
        public string? BloodPressure { get; set; } // e.g., "120/80 mmHg"
        public decimal OxygenSaturation { get; set; }

        // --- DataPoints: LabResults ---
        // Stored as a string/JSON for flexibility or a separate notes field
        public string? LabResults { get; set; }

        // Metadata for auditing
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

