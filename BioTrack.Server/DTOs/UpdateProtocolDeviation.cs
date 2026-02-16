using BioTrack.Server.Models;
using System.ComponentModel.DataAnnotations;

namespace BioTrack.Server.DTOs
{
    public class UpdateProtocolDeviation
    {

        [Required]
        public int ProtocolID { get; set; }

        [Required]
        public int ParticipantID { get; set; }

        /// <summary>
        /// Optional. Use null to clear Observation linkage if your service supports that.
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
        /// Must be UTC.
        /// </summary>
        [Required]
        public DateTime ReportedDate { get; set; }

        }
    }
