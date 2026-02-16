using System.ComponentModel.DataAnnotations;

namespace BioTrack.Server.DTOs
{
    public class UpdateProtcolDto
    {

        [Required]
        public int ProtocolID { get; set; }

        [Required(ErrorMessage = "Protocol Title is mandatory")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Range(1, 3, ErrorMessage = "Phase must be 1, 2, or 3")]
        public int Phase { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Range(0, 100)]
        public decimal EnrollmentRate { get; set; }

        [Range(0, 100)]
        public decimal CompletionRate { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string Objectives { get; set; } = string.Empty;

        public int? LeadResearcherId { get; set; }

        /// <summary>
        /// Replace current relationships with these IDs (idempotent).
        /// Send the full set you want persisted.
        /// </summary>
        public List<int> StudySiteIds { get; set; } = new();
        public List<int> ParticipantIds { get; set; } = new();
        public List<int> ComplianceReportIds { get; set; } = new();
        public List<int> TrialReportIds { get; set; } = new();
        public List<int> ProtocolDeviationIds { get; set; } = new();

    }
}
