using BioTrack.Server.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioTrack.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SitesController(BioDataContext context) : ControllerBase
    {
        private readonly BioDataContext context;
       

        [HttpGet]
        public async Task<IActionResult> GetAllSites()
        {
            var list = await context.StudySites.ToListAsync();
            return Ok(list); 
        }
    }
}
