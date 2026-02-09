using BioTrack.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BioTrack.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace BioTrack.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantsController(BioDataContext context) : ControllerBase

    {
        private readonly BioDataContext _context;

        // The constructor handles the Dependency Injection
       


        [HttpGet("getAllPartcipants")]
        public async Task<IActionResult> GetAllParticipants()
        {
            var list = await _context.Participants.ToListAsync();
            return Ok(list);
        }


    }
}
