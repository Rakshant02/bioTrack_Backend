namespace BioTrack.Server.DTOs
{
    public class ObservationsReadDto
    {

        public int ObservationID { get; set; }
        public int ParticipantID { get; set; }
        public DateTime VisitDate { get; set; }
        
        public decimal BodyTemperature { get; set; }
        public int HeartRate { get; set; }
        public string? BloodPressure { get; set; }
        public decimal OxygenSaturation { get; set; }
        public int Hemoglobin { get; set; }
        public int Creatinine { get; set; }

    }
}
