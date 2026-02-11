using BioTrack.Server.Data;
using BioTrack.Server.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioTrack.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrialReportsController(BioDataContext _context , ILogger _logger) : ControllerBase
    {
        private readonly BioDataContext context;
        private readonly ILogger<TrialReportsController> _logger;



        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrialReportReadDto>>> GetAll(CancellationToken ct)
        {
            try
            {
                // Eager load the protocol so DTO can include the title.
                var reports = await context.TrialsReports
                    .AsNoTracking()
                    .Include(r => r.TrialProtocol)
                    .OrderByDescending(r => r.GeneratedDate)
                    .Select(r => new TrialReportReadDto
                    {
                        ReportID = r.ReportID,
                        ProtocolID = r.ProtocolID,
                        ProtocolTitle = r.TrialProtocol.Title,
                        GeneratedDate = r.GeneratedDate
                    })
                    .ToListAsync(ct);

                return Ok(reports);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetAll TrialReports was canceled by the client.");
                return new StatusCodeResult(StatusCodes.Status499ClientClosedRequest); // Kestrel recognizes 499 pattern
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all TrialReports.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred while fetching trial reports." });
            }
        }

        /// <summary>
        /// Get a single trial report by ID (includes basic protocol info).
        /// </summary>
        [HttpGet("{trialReportsId:int}")]
        public async Task<ActionResult<TrialReportReadDto>> GetBy(int trialReportsId, CancellationToken ct)
        {
            if (trialReportsId <= 0)
            {
                return BadRequest(new { message = "Invalid trialReportsId." });
            }

            try
            {
                var report = await context.TrialsReports
                    .AsNoTracking()
                    .Include(r => r.TrialProtocol)
                    .Where(r => r.ReportID == trialReportsId)
                    .Select(r => new TrialReportReadDto
                    {
                        ReportID = r.ReportID,
                        ProtocolID = r.ProtocolID,
                        ProtocolTitle = r.TrialProtocol.Title,
                        GeneratedDate = r.GeneratedDate
                    })
                    .FirstOrDefaultAsync(ct);

                if (report is null)
                {
                    return NotFound(new { message = $"TrialReport with id {trialReportsId} was not found." });
                }

                return Ok(report);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetBy({trialReportsId}) was canceled by the client.", trialReportsId);
                return new StatusCodeResult(StatusCodes.Status499ClientClosedRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching TrialReport {trialReportsId}.", trialReportsId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred while fetching the trial report." });
            }
        }



    }
}
