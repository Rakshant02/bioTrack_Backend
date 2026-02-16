using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BioTrack.Server.Models
{
    [Table("TrialsProtocols")]
    public class TrialProtocols
    {
        [Key]
        public int ProtocolID { get; set; }

        [Required(ErrorMessage = "Protocol Title is mandatory")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Range(1, 3, ErrorMessage = "Phase must be 1, 2, or 3")]
        public int Phase { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Precision(18, 2)]
        [Display(Name = "Enrollment Rate (%)")]
        public decimal EnrollmentRate { get; set; }

        [Precision(18, 2)]
        [Display(Name = "Completion Rate (%)")]
        public decimal CompletionRate { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        // NEW: Trial objectives/aims (free text)
        [MaxLength(2000)]
        public string Objectives { get; set; } = string.Empty;

        // NEW: Lead investigator (FK -> ResearcherCredentials)
        public int? LeadResearcherId { get; set; }

        [ForeignKey(nameof(LeadResearcherId))]
        public ResearcherCredentials? LeadResearcher { get; set; }

        // Existing navigations
        public ICollection<StudySites> StudySites { get; set; } = new List<StudySites>();
        public ICollection<Participants> Participants { get; set; } = new List<Participants>();
        public ICollection<ComplianceReports> ComplianceReports { get; set; } = new List<ComplianceReports>();
        public ICollection<TrialReports> TrialReports { get; set; } = new List<TrialReports>();
        public ICollection<ProtocolDeviation> ProtocolDeviations { get; set; } = new List<ProtocolDeviation>();
    }
}