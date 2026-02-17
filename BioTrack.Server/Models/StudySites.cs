// Models/StudySites.cs
using BioTrack.Server.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("StudySites")]
public class StudySites
{
    [Key] public int SiteID { get; set; }

    // ✅ MUST be nullable for SET NULL to work
    public int? ProtocolID { get; set; }

    [ForeignKey(nameof(ProtocolID))]
    public TrialProtocols? TrialProtocol { get; set; }  // nullable nav

    [Required] public string Location { get; set; } = default!;

    public ICollection<Participants> Participants { get; set; } = new List<Participants>();
}