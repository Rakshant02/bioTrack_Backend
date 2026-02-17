using System.ComponentModel.DataAnnotations;

namespace BioTrack.Server.DTOs
{
    public class StudySiteCreateDto
    {
        // No ProtocolID here anymore
        [Required]
        public string Location { get; set; } = default!;

        //[Required]
        //public string InvestigatorName { get; set; } = default!;
    }
}