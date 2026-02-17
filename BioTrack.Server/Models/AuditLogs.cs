using System;
using System.ComponentModel.DataAnnotations;

namespace BioTrack.Server.Models
{
    public class AuditLogs
    {
        [Key]
        public int LogId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Action { get; set; }

        [MaxLength(256)]
        public string User { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}