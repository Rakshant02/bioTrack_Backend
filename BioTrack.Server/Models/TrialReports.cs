using System.ComponentModel.DataAnnotations;

namespace BioTrack.Server.Models
{
    public class TrialReports
    {
        [Key]
        public int ReportID { get; set; }

        // Flattening the Metrics into individual properties
        [Display(Name = "Enrollment Rate (%)")]
        public decimal EnrollmentRate { get; set; }

        [Display(Name = "Completion Rate (%)")]
        public decimal CompletionRate { get; set; }

        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;
    }
}
