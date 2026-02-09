using System.ComponentModel.DataAnnotations;

namespace BioTrack.Server.Models
{
    public class Participants
    {
        [Key]
        public int ParticipantID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public DateTime DOB { get; set; }
        [Required]
        public string Gender { get; set; }

        [EmailAddress] // Ensures valid email format
        public string ContactInfo { get; set; }

        
        [Required]
        public string Address {  get; set; }

        [Required]
        public int BloodPressure { get; set; }

        [Required]
        public double Temperature { get; set; }

        [Required]
        public int HeartRate { get; set; }

        [Required]
        public string EligibilityStatus
        {
            get; set;
        }


    }
}
