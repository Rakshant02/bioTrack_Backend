using BioTrack.Server.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BioTrack.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProtocolsController(BioDataContext context) : ControllerBase

    {
        private readonly BioDataContext _context;

        // The constructor handles the Dependency Injection
       

        [HttpGet("getAllProtocols")]
        public async Task<IActionResult> GetAllProtocols()
        {
            var list = await _context.TrialsProtocols.ToListAsync();
            return Ok(list);
        }
    }
}
