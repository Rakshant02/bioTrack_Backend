using BioTrack.Server.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ConsentForm
{
    [Key]
    public int ConsentID { get; set; }

    [Required]
    public int ParticipantID { get; set; }

    [ForeignKey(nameof(ParticipantID))]
    public Participants Participant { get; set; } = default!;

    [Required]
    public ConsentStatus Status { get; set; } // SIGNED/WITHDRAWN
}

public enum ConsentStatus
{
    SIGNED = 1,
    WITHDRAWN = 2
}
