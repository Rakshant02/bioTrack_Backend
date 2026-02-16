using System.ComponentModel.DataAnnotations;

namespace BioTrack.Server.DTOs
{
    /// <summary>
    /// DTO used to create a new ResearcherCredentials record.
    /// Accepts a plaintext Password which will be hashed server-side.
    /// </summary>
    public class CreateResearcherDto
    {
        [Required, MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(150), EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Plaintext password from the user. Do NOT store this directly.
        /// Hash it into ResearcherCredentials.PasswordHash on the server.
        /// </summary>
        //[Required]
        //[MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        // You can add stronger rules if needed:
        // [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
        //     ErrorMessage = "Password must contain upper, lower, and a digit.")]
        //public string Password { get; set; } = string.Empty;

        // Optional extras (uncomment if you plan to capture them)
        // [Phone] public string? Phone { get; set; }
        // [MaxLength(150)] public string? Department { get; set; }
    }
}
