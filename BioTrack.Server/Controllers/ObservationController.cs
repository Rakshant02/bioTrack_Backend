using BioTrack.Server.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioTrack.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObservationController(BioDataContext context ) : ControllerBase
    {
        private readonly BioDataContext _context;

        // The constructor handles the Dependency Injection
        

        [HttpGet("getAllObservations")]
        public async Task<IActionResult> GetAllObservations()
        {
            var list = await context.Observations.ToListAsync();
            return Ok(list);
        }

    }
}
