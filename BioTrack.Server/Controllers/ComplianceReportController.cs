using BioTrack.Server.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioTrack.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplianceReportController(BioDataContext context) : ControllerBase
    {
        private readonly BioDataContext context;

        

        [HttpGet("getAllComplianceReports")]
        public async Task<IActionResult> GetAllComplianceReports()
        {
            var list = await context.ComplianceReports.ToListAsync();
            return Ok(list);
        }
    }
}
