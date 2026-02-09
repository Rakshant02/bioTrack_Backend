using System.ComponentModel.DataAnnotations;

namespace BioTrack.Server.Models
{
    public class StudySites
    {
        [Key]
        public int SiteID { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string InvestigatorName { get; set; }
    }
}
