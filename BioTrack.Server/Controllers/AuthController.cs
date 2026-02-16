using BioTrack.Server.DTOs;
using BioTrack.Server.service;
using Microsoft.AspNetCore.Mvc;

namespace BioTrack.Server.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            // TEMP login (replace with DB later)
            if (dto.Username != "admin" || dto.Password != "123")
                return Unauthorized("Invalid credentials");

            var token = _tokenService.CreateToken(dto.Username);

            return Ok(new AuthResponseDto
            {
                Token = token
            });
        }
    }
}
