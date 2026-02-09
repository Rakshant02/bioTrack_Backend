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

        // Navigation property to the Participant
        //[ForeignKey("ParticipantID")]
        //public virtual Participant Participant { get; set; }

        public DateTime SignedDate { get; set; }

        [Required]
        public ConsentStatus Status { get; set; }
    }

    public enum ConsentStatus
    {
        SIGNED = 1,
        WITHDRAWN = 2
    }
}

