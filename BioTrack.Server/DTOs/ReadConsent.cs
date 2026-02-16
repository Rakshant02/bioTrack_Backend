namespace BioTrack.Server.DTOs
{
    public class ReadConsent
    {


        public int ConsentID { get; set; }
        public int ParticipantID { get; set; }
        public ConsentStatus Status
        {
            get; set;
        }

        }
    }
