using BioTrack.Server.Models;
using System.ComponentModel.DataAnnotations;

namespace BioTrack.Server.DTOs
{
    public class CreateProtocolDeviationDto
    {

        [Required]
        public int ProtocolID { get; set; }

        [Required]
        public int ParticipantID { get; set; }

        /// <summary>
        /// Optional: not all deviations arise from a recorded observation.
        /// </summary>
        public int? ObservationID { get; set; }

        // --- Details ---
        [Required, MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [EnumDataType(typeof(SeverityLevel))]
        public SeverityLevel Severity { get; set; }

        //[Required, MaxLength(150)]
        //public string DetectedBy { get; set; } = string.Empty;

        /// <summary>
        /// If omitted by the client, your service can default to DateTime.UtcNow.
        /// Must be in UTC if provided.
        /// </summary>
        /// 
        [Required]
        public DateTime? ReportedDate { get; set; }
    

}
}
