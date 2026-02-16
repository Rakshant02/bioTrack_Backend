namespace BioTrack.Server.DTOs
{
    public class ReadComplianceReportDto
    {

        public int ReportID { get; set; }
        public int ProtocolID { get; set; }
        public int DeviationCount { get; set; }
        public decimal AdherenceRate { get; set; }
        public DateTime GeneratedDate { get; set; }

    }
}
