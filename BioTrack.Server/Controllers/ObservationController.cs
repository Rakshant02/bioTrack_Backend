// File: Controllers/ObservationsController.cs
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
    public class ObservationsController : ControllerBase
    {
        private readonly BioDataContext _db;
        private readonly ILogger<ObservationsController> _logger;
        private readonly IMapper _mapper;

        public ObservationsController(BioDataContext db, ILogger<ObservationsController> logger, IMapper mapper)
        {
            _db = db;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<ObservationsReadDto>>> GetAllAsync(
     [FromQuery] int page = 1,
     [FromQuery] int pageSize = 100,
     CancellationToken ct = default)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest(new { message = "page and pageSize must be positive." });

            try
            {
                var query = _db.Observations.AsNoTracking();

                var results = await query
                    .OrderByDescending(o => o.VisitDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ProjectTo<ObservationsReadDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(ct);

                return Ok(results);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetAllAsync(Observations) canceled by client.");
                return StatusCode(499);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllAsync(Observations).");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred while fetching observations." });
            }
        }

        [HttpPost("CreateObservation")]
        public async Task<ActionResult<ObservationsReadDto>> CreateAsync(
     [FromBody] ObservationsCreateDto dto,
     CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                // Check if participant exists
                bool participantExists = await _db.Participants
                    .AsNoTracking()
                    .AnyAsync(p => p.ParticipantID == dto.ParticipantID, ct);

                if (!participantExists)
                    return NotFound(new { message = $"Participant {dto.ParticipantID} not found." });

                if (dto.ProtocolID.HasValue)
                {
                    bool protocolExists = await _db.TrialsProtocols
                        .AsNoTracking()
                        .AnyAsync(p => p.ProtocolID == dto.ProtocolID.Value, ct);

                    if (!protocolExists)
                        return NotFound(new { message = $"Protocol {dto.ProtocolID.Value} not found." });
                }


                // Map DTO -> Entity
                var entity = _mapper.Map<Observations>(dto);

                // Save to DB
                _db.Observations.Add(entity);
                await _db.SaveChangesAsync(ct);

                // Map saved entity -> DTO
                var readDto = _mapper.Map<ObservationsReadDto>(entity);

                // Return 201 Created
                return CreatedAtAction(nameof(GetByIdAsync),
                    new { observationId = readDto.ObservationID },
                    readDto);
            }
            catch (DbUpdateException dbEx)
            {
                // For learning-level projects, simple logging is enough
                _logger.LogError(dbEx, "Database error while creating observation.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Database error occurred." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating observation.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Unexpected server error occurred." });
            }
        }


        [HttpGet("{observationId:int}", Name = nameof(GetByIdAsync))]
        public async Task<ActionResult<ObservationsReadDto>> GetByIdAsync(int observationId, CancellationToken ct)
        {
            if (observationId <= 0)
                return BadRequest(new { message = "Invalid observationId." });

            try
            {
                var obs = await _db.Observations
                    .AsNoTracking()
                    .Where(o => o.ObservationID == observationId)
                    .ProjectTo<ObservationsReadDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(ct);

                if (obs is null)
                    return NotFound(new { message = $"Observation {observationId} not found." });

                return Ok(obs);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetByIdAsync({observationId}) canceled by client.", observationId);
                return StatusCode(499);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetByIdAsync({observationId}).", observationId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred while fetching the observation." });
            }
        }
    }
}