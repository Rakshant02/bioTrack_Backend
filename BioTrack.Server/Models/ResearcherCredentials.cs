using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioTrack.Server.Models
{
    [Table("ResearcherCredentials")]
    public class ResearcherCredentials
    {
        [Key]
        public int ResearcherId { get; set; }

        [Required, MaxLength(150)]
        public string FullName { get; set; }

        [Required, MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; }

        // Store only a secure hash, never plaintext passwords
        [Required]
        public string PasswordHash { get; set; }

        // Reverse navigation for sites where this researcher is the PI (1 : many)
        public ICollection<StudySites> PrincipalInvestigatorSites { get; set; } = new List<StudySites>();

        // Many-to-many: sites where this researcher collaborates
        public ICollection<StudySites> CollaboratingSites { get; set; } = new List<StudySites>();
        public ICollection<TrialProtocols> LeadProtocols { get; set; } = new List<TrialProtocols>();    
    }
}