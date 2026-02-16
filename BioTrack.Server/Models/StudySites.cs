using System.Collections.Generic;
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

        // You chose to keep this
        [Required]
        public string InvestigatorName { get; set; } // Consider deprecating later

        // Principal Investigator (required 1 : many to Researcher)
        [Required]
        public int PrincipalInvestigatorId { get; set; }

        [ForeignKey(nameof(PrincipalInvestigatorId))]
        public ResearcherCredentials PrincipalInvestigator { get; set; }

        // Many-to-many collaborators (skip navigation)
        public ICollection<ResearcherCredentials> StudySiteResearchers { get; set; } = new List<ResearcherCredentials>();

        // Existing relationship to participants
        public ICollection<Participants> Participants { get; set; } = new List<Participants>();
    }
}