namespace BioTrack.Server.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; } = default!;
        public string Role { get; set; } = default!;
        public DateTime ExpiresAtUtc { get; set; }
    }
}