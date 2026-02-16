namespace BioTrack.Server.DTOs
{
    public class CreateComplianceReport
    {

        public int ProtocolID { get; set; }
        // Optional: accept AdherenceRate; defaults to 0 if omitted
        public decimal? AdherenceRate
        {
            get; set;

        }
    }
    }
