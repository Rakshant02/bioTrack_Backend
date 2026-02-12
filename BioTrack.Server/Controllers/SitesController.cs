using BioTrack.Server.Data;
using BioTrack.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioTrack.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SitesController : ControllerBase
    {
        private readonly BioDataContext context;

        public SitesController(BioDataContext context)
        {
            this.context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllSites()
        {
            var list = await context.StudySites.ToListAsync();
            return Ok(list); 
        }

        [HttpPost]
        public async Task<IActionResult> AddSite([FromBody] StudySites site)
        {
            if(site == null)
            {
                return BadRequest("Invalid data");
            }

            await context.StudySites.AddAsync(site);

            await context.SaveChangesAsync();

            return Ok(site);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSiteById(int id)
        {
            var site = await context.StudySites.FindAsync(id);

            if(site == null)
            {
                return NotFound("Site not found");
            }

            return Ok(site);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetSiteCount()
        {
            var count = await context.StudySites.CountAsync();

            return Ok(new
            {
                totalSites = count
            });
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSite(int id)
        {
            var site = await context.StudySites.FindAsync(id);

            if (site == null)
                return NotFound("Site not found");

            context.StudySites.Remove(site);
            await context.SaveChangesAsync();

            return Ok("Deleted successfully");
        }
    }
}
