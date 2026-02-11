using System.ComponentModel.DataAnnotations;

namespace BioTrack.Server.DTOs
{
    public class CreateParticipantDto
    {
        [Required]
        public int ProtocolID { get; set; }      // <-- added, required

        public int? SiteID { get; set; }         // <-- optional site

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of Birth is required")]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [MaxLength(20)]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age is required")]
        [MaxLength(10)]
        public string Age { get; set; } = string.Empty;

        // Option A: Optional email (matches nullable entity)
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(254)]
        public string? ContactInfo { get; set; }

        // Option B (if required): uncomment next two lines and remove the above:
        // [Required, EmailAddress, MaxLength(254)]
        // public string ContactInfo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [MaxLength(300)]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Blood Pressure is required")]
        [Range(40, 300)]
        public int BloodPressure { get; set; }

        [Required(ErrorMessage = "Temperature is required")]
        [Range(30, 45)]
        public double Temperature { get; set; }

        [Required(ErrorMessage = "Heart Rate is required")]
        [Range(20, 250)]
        public int HeartRate { get; set; }

        [Required(ErrorMessage = "Eligibility Status is required")]
        [RegularExpression("PENDING|ELIGIBLE|INELIGIBLE", ErrorMessage = "EligibilityStatus must be PENDING, ELIGIBLE, or INELIGIBLE")]
        public string EligibilityStatus { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("ENROLLED|WITHDRAWN|COMPLETED", ErrorMessage = "Status must be ENROLLED, WITHDRAWN, or COMPLETED")]
        public string Status { get; set; } = string.Empty;
    }
}
