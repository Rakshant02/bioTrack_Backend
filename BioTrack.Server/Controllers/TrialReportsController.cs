using BioTrack.Server.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioTrack.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrialReportsController(BioDataContext context) : ControllerBase
    {
        private readonly BioDataContext context;

        

        [HttpGet]
        public async Task<IActionResult> GetAllReports()
        {
            var list = await context.TrialsReports.ToListAsync();
            return Ok(list);     
        }
    }
}
