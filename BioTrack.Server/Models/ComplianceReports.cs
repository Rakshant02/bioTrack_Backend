using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioTrack.Server.Models
{
    public class ComplianceReports
    {
        [Key]
        public int ReportID { get; set; }

        [Required]
        public int ProtocolID { get; set; }  // renamed for consistency
        [ForeignKey(nameof(ProtocolID))]
        public TrialProtocols TrialProtocol { get; set; }

        [Required]
        public int DeviationCount { get; set; }     // PDF metric

        [Required, Range(0, 100)]
        public decimal AdherenceRate { get; set; }  // PDF metric (%)

        [Required]
        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;
    }
}