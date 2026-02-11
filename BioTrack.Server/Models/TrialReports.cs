using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioTrack.Server.Models
{
    public class TrialReports
    {
        [Key]
        public int ReportID { get; set; }

        // NEW: tie each report to a protocol (PDF)
        [Required]
        public int ProtocolID { get; set; }
        [ForeignKey(nameof(ProtocolID))]
        public TrialProtocols TrialProtocol { get; set; }

        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;
    }
}