
using System.Globalization;

namespace BioTrack.Server.DTOs
    {
        public class ParticipantsDto
        {
            public int ParticipantID { get; set; }

            public string Name { get; set; }

            public DateTime DOB { get; set; }

            public string Gender { get; set; }
        public string Age { get; set; }

            public string ContactInfo { get; set; }

            public string Address { get; set; }

            public int BloodPressure { get; set; }

            public double Temperature { get; set; }

            public int HeartRate { get; set; }

            public string EligibilityStatus { get; set; }
       public string Status { get; set; }
        }
    } 



