// File: Dtos/Observations/ObservationCreateDto.cs
using System.ComponentModel.DataAnnotations;

namespace BioTrack.Server.DTOs
{
    public sealed class ObservationsCreateDto
    {
        [Required]
        public int ParticipantID { get; set; }
        public int? ProtocolID { get; set; }
        [Required]
        public DateTime VisitDate { get; set; }

       

        [Range(30, 45, ErrorMessage = "BodyTemperature must be between 30.00 and 45.00 °C")]
        public decimal BodyTemperature { get; set; }

        [Range(20, 250)]
        public int HeartRate { get; set; }
        [Required]
        [RegularExpression(@"^\d{2,3}/\d{2,3}\s?mmHg$", ErrorMessage = "BloodPressure should look like '120/80 mmHg'")]
        public string? BloodPressure { get; set; }

        [Range(0, 100)]
        public decimal OxygenSaturation { get; set; }

        // Basic sanity bounds (tune as per lab normal ranges)
        [Range(3, 20)]
        public int Hemoglobin { get; set; }

        [Range(0, 20)]
        public int Creatinine { get; set; }
    }
}
