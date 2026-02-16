using AutoMapper;
using AutoMapper.QueryableExtensions;
using BioTrack.Server.Data;
using BioTrack.Server.DTOs;
using BioTrack.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioTrack.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdverseEventsController : ControllerBase
    {
        private readonly BioDataContext _db;
        private readonly ILogger<AdverseEventsController> _logger;
        private readonly IMapper _mapper;

        public AdverseEventsController(BioDataContext db, ILogger<AdverseEventsController> logger, IMapper mapper)
        {
            _db = db;
            _logger = logger;
            _mapper = mapper;
        }

        // POST: /api/adverseevents
        [HttpPost]
        public async Task<ActionResult<AdverseEventsReadDto>> CreateAsync(
           [FromBody]  AdverseEventsCreateDto dto,
            CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                // FK safety: ensure participant exists
                bool participantExists = await _db.Participants
                    .AsNoTracking()
                    .AnyAsync(p => p.ParticipantID == dto.ParticipantID, ct);

                if (!participantExists)
                    return NotFound(new { message = $"Participant {dto.ParticipantID} not found." });

                // Optional enum safety
                if (!Enum.IsDefined(typeof(AdverseEventSeverity), dto.Severity))
                    return BadRequest(new { message = $"Invalid severity value: {dto.Severity}." });

                var entity = _mapper.Map<AdverseEvents>(dto);

                _db.AdverseEvents.Add(entity);
                await _db.SaveChangesAsync(ct);

                var readDto = _mapper.Map<AdverseEventsReadDto>(entity);

                return CreatedAtAction(nameof(GetByIdAsync), new { eventId = readDto.EventID }, readDto);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("CreateAsync(AdverseEvents) canceled by client.");
                return StatusCode(499);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while creating AdverseEvent.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Database error occurred while creating the adverse event." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating AdverseEvent.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Unexpected server error occurred." });
            }
        }

        // GET: /api/adverseevents/{eventId}
        [HttpGet("{eventId:int}")]
        public async Task<ActionResult<AdverseEventsReadDto>> GetByIdAsync(int eventId, CancellationToken ct)
        {
            if (eventId <= 0)
                return BadRequest(new { message = "Invalid eventId." });

            try
            {
                var item = await _db.AdverseEvents
                    .AsNoTracking()
                    .Where(ae => ae.EventID == eventId)
                    .ProjectTo<AdverseEventsReadDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(ct);

                if (item is null)
                    return NotFound(new { message = $"AdverseEvent {eventId} not found." });

                return Ok(item);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetByIdAsync({eventId}) canceled by client.", eventId);
                return StatusCode(499);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetByIdAsync({eventId}).", eventId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred while fetching the adverse event." });
            }
        }

        // GET: /api/adverseevents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdverseEventsReadDto>>> GetAllAsync(CancellationToken ct)
        {
            try
            {
                var items = await _db.AdverseEvents
                    .AsNoTracking()
                    .OrderByDescending(ae => ae.ReportedDate)
                    .ProjectTo<AdverseEventsReadDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(ct);

                return Ok(items);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetAllAsync(AdverseEvents) canceled by client.");
                return StatusCode(499);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllAsync(AdverseEvents).");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred while fetching adverse events." });
            }
        }

        // GET: /api/adverseevents/mild
        [HttpGet("mild")]
        public Task<ActionResult<IEnumerable<AdverseEventsReadDto>>> GetMildAsync(CancellationToken ct)
            => GetBySeverityInternalAsync(AdverseEventSeverity.Mild, ct);

        // GET: /api/adverseevents/moderate
        [HttpGet("moderate")]
        public Task<ActionResult<IEnumerable<AdverseEventsReadDto>>> GetModerateAsync(CancellationToken ct)
            => GetBySeverityInternalAsync(AdverseEventSeverity.Moderate, ct);

        // GET: /api/adverseevents/severe
        [HttpGet("severe")]
        public Task<ActionResult<IEnumerable<AdverseEventsReadDto>>> GetSevereAsync(CancellationToken ct)
            => GetBySeverityInternalAsync(AdverseEventSeverity.Severe, ct);

        // GET: /api/adverseevents/life-threatening  (treat as "critical")
        [HttpGet("life-threatening")]
        public Task<ActionResult<IEnumerable<AdverseEventsReadDto>>> GetLifeThreateningAsync(CancellationToken ct)
            => GetBySeverityInternalAsync(AdverseEventSeverity.LifeThreatening, ct);

        private async Task<ActionResult<IEnumerable<AdverseEventsReadDto>>> GetBySeverityInternalAsync(
            AdverseEventSeverity severity, CancellationToken ct)
        {
            try
            {
                var items = await _db.AdverseEvents
                    .AsNoTracking()
                    .Where(ae => ae.Severity == severity)
                    .OrderByDescending(ae => ae.ReportedDate)
                    .ProjectTo<AdverseEventsReadDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(ct);

                return Ok(items);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetBySeverityInternalAsync({severity}) canceled by client.", severity);
                return StatusCode(499);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetBySeverityInternalAsync({severity}).", severity);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred while fetching adverse events by severity." });
            }
        }

        // GET: /api/adverseevents/count
        [HttpGet("count")]
        public async Task<ActionResult<int>> CountOfAdverseEventsAsync(CancellationToken ct)
        {
            try
            {
                var count = await _db.AdverseEvents.CountAsync(ct);
                return Ok(count);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("CountOfAdverseEventsAsync canceled by client.");
                return StatusCode(499);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CountOfAdverseEventsAsync.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred while counting adverse events." });
            }
        }

        // GET: /api/adverseevents/count/severe-and-critical
        [HttpGet("count/severe-and-critical")]
        public async Task<ActionResult<int>> CountOfSevereAndCriticalAsync(CancellationToken ct)
        {
            try
            {
                var count = await _db.AdverseEvents
                    .Where(ae => ae.Severity == AdverseEventSeverity.Severe
                              || ae.Severity == AdverseEventSeverity.LifeThreatening)
                    .CountAsync(ct);

                return Ok(count);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("CountOfSevereAndCriticalAsync canceled by client.");
                return StatusCode(499);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CountOfSevereAndCriticalAsync.");
                return StatusCodes.Status500InternalServerError is int code
                    ? StatusCode(code, new { message = "An unexpected error occurred while counting severe/critical adverse events." })
                    : StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred while counting severe/critical adverse events." });
            }
        }
    }
}