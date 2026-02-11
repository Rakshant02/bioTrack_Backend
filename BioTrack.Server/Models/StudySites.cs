using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioTrack.Server.Models
{
    [Table("StudySites")] 
    public class StudySites
    {
        [Key]
        public int SiteID { get; set; }

        [Required]
        public int ProtocolID { get; set; }

        [ForeignKey(nameof(ProtocolID))]
        public TrialProtocols TrialProtocol { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string InvestigatorName { get; set; }

        
        public ICollection<Participants> Participants { get; set; } = new List<Participants>();
        
    }
}