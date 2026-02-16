using BioTrack.Server.Models;

namespace BioTrack.Server.DTOs
{
    public class ReadProtocolDeviation
    {

        public int ProtocolID { get; set; }
        public int ParticipantID { get; set; }
        public int? ObservationID { get; set; }

        // --- Details ---
        public string Description { get; set; } = string.Empty;
        public SeverityLevel Severity { get; set; }
        //public string DetectedBy { get; set; } = string.Empty;
        public DateTime ReportedDate { get; set; } // UTC

    }
}
