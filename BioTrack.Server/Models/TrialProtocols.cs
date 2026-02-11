using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioTrack.Server.Models
{
    [Table("TrialsProtocols")] // pin to existing DB table name
    public class TrialProtocols
    {
        [Key]
        public int ProtocolID { get; set; }

        [Required(ErrorMessage = "Protocol Title is mandatory")]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [Range(1, 3, ErrorMessage = "Phase must be 1, 2, or 3")]
        public int Phase { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Display(Name = "Enrollment Rate (%)")]
        public decimal EnrollmentRate { get; set; }

        [Display(Name = "Completion Rate (%)")]
        public decimal CompletionRate { get; set; }

        [Required]
        public string Status { get; set; } // "ACTIVE", "COMPLETED"

        // Navigations
        // ...
        public ICollection<StudySites> StudySites { get; set; } = new List<StudySites>();
        public ICollection<Participants> Participants { get; set; } = new List<Participants>();
        public ICollection<ComplianceReports> ComplianceReports { get; set; } = new List<ComplianceReports>();
        public ICollection<TrialReports> TrialReports { get; set; } = new List<TrialReports>();
        // ...
    }
}