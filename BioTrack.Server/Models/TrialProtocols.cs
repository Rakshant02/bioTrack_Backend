using System.ComponentModel.DataAnnotations;

namespace BioTrack.Server.Models
{
    public class TrialProtocols
    {
        [Key]
        public int ProtocolID { get; set; }

        [Required(ErrorMessage = "Protocol Title is mandatory")]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [RegularExpression("^(I|II|III)$", ErrorMessage = "Phase must be I, II, or III")]
        public string Phase { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; } // Nullable if trial is ongoing

        [Required]
        public string Status { get; set; } // e.g., "ACTIVE", "COMPLETED"
    }
}
