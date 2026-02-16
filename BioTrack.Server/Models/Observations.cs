using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioTrack.Server.Models
{
    public class Observations
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ObservationID { get; set; }

        [Required]
        public int ParticipantID { get; set; }

        [ForeignKey(nameof(ParticipantID))]
        public Participants Participant { get; set; }

        public int? ProtocolID { get; set; }
        // nullable foreign key (safe)
        [ForeignKey(nameof(ProtocolID))]
        public TrialProtocols? Protocol { get; set; }


        [Required]
        public DateTime VisitDate { get; set; }

 

        // DataPoints — Vitals
        [Column(TypeName = "decimal(5,2)")]
        public decimal BodyTemperature { get; set; }

        [Required]
        public int HeartRate { get; set; }


        [MaxLength(20)]
        [Column(TypeName = "varchar(20)")] // or nvarchar if needed
        public string? BloodPressure { get; set; }


        [Column(TypeName = "decimal(5,2)")]
        public decimal OxygenSaturation { get; set; }

        // Lab results (free text for now)
        [Required]
        public int Hemoglobin { get; set; }

        [Required]
        public int Creatinine { get; set; }


        
    }
}
