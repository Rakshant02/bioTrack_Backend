using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BioTrack.Server.Models
{
    public class ComplianceReports
    {
        [Key]
        public int ReportID { get; set; }

        [Required]
        public int ProtocolID { get; set; }

        [ForeignKey(nameof(ProtocolID))]
        public TrialProtocols TrialProtocol { get; set; } = default!;

        [Required]
        public int DeviationCount { get; set; }

        [Precision(18, 2)]
        [Range(0, 100)]
        public decimal AdherenceRate { get; set; }  // keep non-null; defaults to 0 if not provided

        [Required]
        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;
    }
}