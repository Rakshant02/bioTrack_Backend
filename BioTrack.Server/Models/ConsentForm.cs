using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioTrack.Server.Models
{
    public class ConsentForm
    {
        [Key]
        public int ConsentID { get; set; }

        [Required]
        public int ParticipantID { get; set; }

        [ForeignKey(nameof(ParticipantID))]
        public Participants Participant { get; set; }

        public DateTime SignedDate { get; set; }

        [Required]
        public ConsentStatus Status { get; set; } // SIGNED/WITHDRAWN

        // NEW: Support re‑consent (PDF mentions consent mgmt; amendments imply versions)
        public int Version { get; set; } = 1;

        // NEW: Optional pointer to stored consent document
        public string? FileUri { get; set; }

        // Optional: app-level rule – only one active SIGNED at a time
        public bool IsActive { get; set; } = true;
    }

    public enum ConsentStatus
    {
        SIGNED = 1,
        WITHDRAWN = 2
    }
}