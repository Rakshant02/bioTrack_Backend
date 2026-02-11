using BioTrack.Server.Data;
using BioTrack.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BioTrack.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProtocolsController : ControllerBase

    {
        private readonly BioDataContext _context;

        // The constructor handles the Dependency Injection
        public ProtocolsController(BioDataContext context)
        {
            _context = context;
        }


        [HttpGet("getAllProtocols")]
        public async Task<IActionResult> GetAllProtocols()
        {
            var list = await _context.TrialsProtocols.ToListAsync();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProtocol([FromBody] TrialProtocols protocol)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.TrialsProtocols.AddAsync(protocol);

            await _context.SaveChangesAsync();

            return Ok(protocol);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProtocol(int id)
        {
            var protocol = await _context.TrialsProtocols.FindAsync(id);

            if(protocol == null)
            {
                return NotFound($"Protocol with ID {id} not Found");
            }

            _context.TrialsProtocols.Remove(protocol);

            await _context.SaveChangesAsync();

            return Ok($"Protocol with ID {id} deleted successfully");
        }
    }
}
