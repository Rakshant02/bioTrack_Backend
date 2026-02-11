using System.ComponentModel.DataAnnotations;

namespace BioTrack.Server.DTOs
{
    public class UpdateParticipantsDto
    {
        [Required(ErrorMessage = "Participant ID is required")]
        public int ParticipantID { get; set; }

        [Required]
        public int ProtocolID { get; set; }   // <-- include so Protocol can be updated if needed

        public int? SiteID { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime DOB { get; set; }

        [Required, MaxLength(20)]
        public string Gender { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        public string Age { get; set; } = string.Empty;

        // Optional email variant (align with entity choice)
        [EmailAddress, MaxLength(254)]
        public string? ContactInfo { get; set; }

        [Required, MaxLength(300)]
        public string Address { get; set; } = string.Empty;

        [Required, Range(40, 300)]
        public int BloodPressure { get; set; }

        [Required, Range(30, 45)]
        public double Temperature { get; set; }

        [Required, Range(20, 250)]
        public int HeartRate { get; set; }

        [Required, RegularExpression("PENDING|ELIGIBLE|INELIGIBLE")]
        public string EligibilityStatus { get; set; } = string.Empty;

        [Required, RegularExpression("ENROLLED|WITHDRAWN|COMPLETED")]
        public string Status { get; set; } = string.Empty;
    }
}