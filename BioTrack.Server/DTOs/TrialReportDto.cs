

namespace BioTrack.Server.Dtos
{
    public sealed class TrialReportReadDto
    {
        public int ReportID { get; set; }
        public int ProtocolID { get; set; }
        public string? ProtocolTitle { get; set; }
        public DateTime GeneratedDate { get; set; }
        // If later you add metrics (EnrollmentRate, CompletionRate), add them here.
    }
}