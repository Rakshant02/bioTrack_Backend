using System.ComponentModel.DataAnnotations;

namespace BioTrack.Server.Models
{
    public class ComplianceReports
    {
        [Key]
        public int ReportID { get; set; }

        // Metrics: Count of times protocol rules were broken
        [Required]
        public int DeviationCount { get; set; }

        // Metrics: Percentage of protocol adherence (e.g., 98.5)
        [Required]
        [Range(0, 100)]
        public decimal AdherenceRate { get; set; }

        [Required]
        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;

        // Best practice: Link the report to a specific trial phase or site
        public int TrialProtocolID { get; set; }
    }
}

