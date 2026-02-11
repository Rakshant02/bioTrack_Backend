using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static BioTrack.Server.Models.AdverseEvents;

namespace BioTrack.Server.Models
{
    [Table("Participants")]
    public class Participants
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // ensure identity in DB
        public int ParticipantID { get; set; }

        [Required]
        public int ProtocolID { get; set; }

        [ForeignKey(nameof(ProtocolID))]
        public TrialProtocols TrialProtocol { get; set; } = default!; // required nav

        public int? SiteID { get; set; }

        [ForeignKey(nameof(SiteID))]
        public StudySites? StudySite { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime DOB { get; set; }

        [Required, MaxLength(20)]
        public string Gender { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        public string Age { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(254)]
        public string ContactInfo { get; set; } = string.Empty; // <-- nullable

        [Required, MaxLength(300)]
        public string Address { get; set; } = string.Empty;

        [Required]
        public int BloodPressure { get; set; }

        [Required]
        public double Temperature { get; set; }

        [Required]
        public int HeartRate { get; set; }

        [Required, MaxLength(20)]
        public string EligibilityStatus { get; set; } = string.Empty; // PENDING/ELIGIBLE/INELIGIBLE

        [Required, MaxLength(20)]
        public string Status { get; set; } = string.Empty; // ENROLLED/WITHDRAWN/COMPLETED

        public ICollection<ConsentForm> Consents { get; set; } = new List<ConsentForm>();
        public ICollection<Observations> Observations { get; set; } = new List<Observations>();
        public ICollection<AdverseEvents> AdverseEvents { get; set; } = new List<AdverseEvents>();
    }
}
