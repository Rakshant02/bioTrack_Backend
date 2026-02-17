using BioTrack.Server.Data;
using BioTrack.Server.DTOs;
using BioTrack.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioTrack.Server.service;
namespace BioTrack.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly BioDataContext _db;
        private readonly IConfiguration _config;
        private readonly TokenService _tokenService;
        private readonly PasswordHasher<ResearcherCredentials> _hasher = new();

        public AuthController(BioDataContext db, IConfiguration config, TokenService tokenService)
        {
            _db = db;
            _config = config;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.Role))
                return BadRequest("Role is required.");

            if (request.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                var adminUser = _config["Admin:Username"];
                var adminPass = _config["Admin:Password"];

                if (!string.Equals(request.UserNameOrEmail, adminUser, StringComparison.OrdinalIgnoreCase) ||
                    request.Password != adminPass)
                {
                    return Unauthorized("Invalid admin credentials.");
                }

                var token = _tokenService.CreateToken(
                    userName: adminUser!,
                    role: "Admin",
                    extraClaims: new Dictionary<string, string> { { "email", adminUser! } }
                );

                return Ok(new LoginResponse
                {
                    Token = token,
                    Role = "Admin",
                    ExpiresAtUtc = ReadExpiry(token)
                });
            }

            if (request.Role.Equals("Researcher", StringComparison.OrdinalIgnoreCase))
            {
                var email = request.UserNameOrEmail?.Trim();
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
                    return BadRequest("Email and password are required for researcher login.");

                var researcher = await _db.ResearcherCredentials
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Email == email, ct);

                if (researcher is null)
                    return Unauthorized("Researcher not found.");

                // Verify password against stored hash
                var result = _hasher.VerifyHashedPassword(researcher, researcher.PasswordHash, request.Password);
                if (result == PasswordVerificationResult.Failed)
                    return Unauthorized("Invalid researcher credentials.");

                var token = _tokenService.CreateToken(
                    userName: researcher.FullName,
                    role: "Researcher",
                    extraClaims: new Dictionary<string, string>
                    {
                        { "email", researcher.Email },
                        { "researcherId", researcher.ResearcherId.ToString() }
                    }
                );

                return Ok(new LoginResponse
                {
                    Token = token,
                    Role = "Researcher",
                    ExpiresAtUtc = ReadExpiry(token)
                });
            }

            return BadRequest("Unknown role. Use 'Admin' or 'Researcher'.");
        }

        private static DateTime ReadExpiry(string jwt)
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            var expUnix = long.Parse(token.Claims.First(c => c.Type == "exp").Value);
            return DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
        }
    }
}