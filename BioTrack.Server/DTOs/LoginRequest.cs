namespace BioTrack.Server.DTOs
{
    public class LoginRequest
    {
        public string Role { get; set; } = default!;           // "Admin" | "Researcher"
        public string UserNameOrEmail { get; set; } = default!; // Admin uses username/email; Researcher uses Email
        public string? Password { get; set; }
    }
}