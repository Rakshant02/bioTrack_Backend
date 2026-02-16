namespace BioTrack.Server.DTOs
{
    public class CreateConsent
    {

        public int ParticipantID { get; set; }           // can be populated from route to avoid client tampering
        public ConsentStatus Status { get; set; }

    }
}
