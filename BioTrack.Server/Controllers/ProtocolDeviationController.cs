    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using BioTrack.Server.Data;
    using BioTrack.Server.DTOs;
    using BioTrack.Server.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    namespace BioTrack.Server.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        [Produces(MediaTypeNames.Application.Json)]
        public class ProtocolDeviationController : ControllerBase
        {
            private readonly BioDataContext _db;
            private readonly IMapper _mapper;
            private readonly ILogger<ProtocolDeviationController> _logger;

            public ProtocolDeviationController(
                BioDataContext db,
                IMapper mapper,
                ILogger<ProtocolDeviationController> logger)
            {
                _db = db;
                _mapper = mapper;
                _logger = logger;
            }

            // --------------------------------------------------------
            // GET: api/protocoldeviations/getAll?page=1&pageSize=100
            // Pagination LIKE your Observations method (list-only body)
            // --------------------------------------------------------
            [HttpGet("getAll")]
            [ProducesResponseType(typeof(IEnumerable<ReadProtocolDeviation>), StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            public async Task<ActionResult<IEnumerable<ReadProtocolDeviation>>> GetAllAsync(
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 100,
                CancellationToken ct = default)
            {
                if (page <= 0 || pageSize <= 0)
                    return BadRequest(new { message = "page and pageSize must be positive." });

                try
                {
                    var query = _db.ProtocolDeviations.AsNoTracking();

                    var results = await query
                        .OrderByDescending(d => d.ReportedDate)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ProjectTo<ReadProtocolDeviation>(_mapper.ConfigurationProvider)
                        .ToListAsync(ct);

                    return Ok(results);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("GetAllAsync(ProtocolDeviations) canceled by client.");
                    // 499: Client Closed Request (commonly used by gateways like Nginx)
                    return StatusCode(499);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in GetAllAsync(ProtocolDeviations).");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { message = "An unexpected error occurred while fetching protocol deviations." });
                }
            }

            // --------------------------------------------------------
            // GET: api/protocoldeviations/getById/123
            // --------------------------------------------------------
            [HttpGet("getById/{id:int}")]
            [ProducesResponseType(typeof(ReadProtocolDeviation), StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<ActionResult<ReadProtocolDeviation>> GetByIdAsync(
                [FromRoute] int id,
                CancellationToken ct = default)
            {
                try
                {
                    var dto = await _db.ProtocolDeviations
                        .AsNoTracking()
                        .Where(d => d.DeviationId == id)
                        .ProjectTo<ReadProtocolDeviation>(_mapper.ConfigurationProvider)
                        .FirstOrDefaultAsync(ct);

                    if (dto == null) return NotFound();
                    return Ok(dto);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("GetByIdAsync(ProtocolDeviations) canceled by client. Id={Id}", id);
                    return StatusCode(499);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in GetByIdAsync(ProtocolDeviations). Id={Id}", id);
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { message = "An unexpected error occurred while fetching the protocol deviation." });
                }
            }

            // --------------------------------------------------------
            // POST: api/protocoldeviations/create
            // Body: ProtocolDeviationCreateDto
            // --------------------------------------------------------
            [HttpPost("create")]
            [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            public async Task<IActionResult> CreateAsync(
                [FromBody] CreateProtocolDeviationDto dto,
                CancellationToken ct = default)
            {
                if (!ModelState.IsValid)
                    return ValidationProblem(ModelState);

                try
                {
                    // Map DTO -> Entity (ReportedDate defaulted in MappingProfile if null)
                    var entity = _mapper.Map<ProtocolDeviation>(dto);

                    await _db.ProtocolDeviations.AddAsync(entity, ct);
                    await _db.SaveChangesAsync(ct);

                    _logger.LogInformation("Created ProtocolDeviation {DeviationId}", entity.DeviationId);

                    return CreatedAtAction(nameof(GetByIdAsync),
                        new { id = entity.DeviationId },
                        new { id = entity.DeviationId });
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("CreateAsync(ProtocolDeviations) canceled by client.");
                    return StatusCode(499);
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "DbUpdateException in CreateAsync(ProtocolDeviations). Payload={@Payload}", dto);
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { message = "Failed to save the protocol deviation to the database." });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in CreateAsync(ProtocolDeviations). Payload={@Payload}", dto);
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { message = "An unexpected error occurred while creating the protocol deviation." });
                }
            }

            // --------------------------------------------------------
            // PUT: api/protocoldeviations/update/123
            // Body: ProtocolDeviationUpdateDto (full update of all properties)
            // --------------------------------------------------------
            [HttpPut("update/{id:int}")]
            [ProducesResponseType(StatusCodes.Status204NoContent)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            public async Task<IActionResult> UpdateAsync(
                [FromRoute] int id,
                [FromBody] UpdateProtocolDeviation dto,
                CancellationToken ct = default)
            {
                if (!ModelState.IsValid)
                    return ValidationProblem(ModelState);

                try
                {
                    var entity = await _db.ProtocolDeviations
                        .FirstOrDefaultAsync(d => d.DeviationId == id, ct);

                    if (entity == null)
                        return NotFound(new { message = $"ProtocolDeviation {id} not found." });

                    // Apply full replacement via AutoMapper (keeps identity and navs safe per mapping config)
                    _mapper.Map(dto, entity);

                    await _db.SaveChangesAsync(ct);
                    _logger.LogInformation("Updated ProtocolDeviation {DeviationId}", id);

                    return NoContent();
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("UpdateAsync(ProtocolDeviations) canceled by client. Id={Id}", id);
                    return StatusCode(499);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency error in UpdateAsync(ProtocolDeviations). Id={Id}", id);
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { message = "Concurrency error while updating the protocol deviation." });
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "DbUpdateException in UpdateAsync(ProtocolDeviations). Id={Id}", id);
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { message = "Failed to update the protocol deviation in the database." });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in UpdateAsync(ProtocolDeviations). Id={Id}", id);
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { message = "An unexpected error occurred while updating the protocol deviation." });
                }
            }
        }
    }
