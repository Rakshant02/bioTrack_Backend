using AutoMapper;
using AutoMapper.QueryableExtensions;
using BioTrack.Server.Data;
using BioTrack.Server.DTOs;
using BioTrack.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BioTrack.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProtocolsController : ControllerBase
    {
        private readonly BioDataContext _db;
        private readonly IMapper _mapper;

        public ProtocolsController(BioDataContext context, IMapper mapper)
        {
            _db = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all protocols (using ReadProtocolDto with counts and related IDs).
        /// GET: /api/protocols
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ReadProtocolDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ReadProtocolDto>>> GetAll(CancellationToken ct)
        {
            try
            {
                var list = await _db.TrialsProtocols
                    .AsNoTracking()
                    .OrderByDescending(p => p.ProtocolID)
                    .ProjectTo<ReadProtocolDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(ct);

                return Ok(list);
            }
            catch
            {
                return StatusCode(500, new { message = "Failed to fetch protocols." });
            }
        }

        /// <summary>
        /// Create a protocol. You may assign an existing investigator (LeadResearcherId),
        /// or create a new one (NewInvestigator). You can also attach study sites.
        /// POST: /api/protocols
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<object>> CreateProtocol([FromBody] CreateProtocolDto request, CancellationToken ct)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ValidationProblem(ModelState);

                if (request.EndDate.HasValue && request.EndDate.Value < request.StartDate)
                    return BadRequest(new { message = "EndDate cannot be earlier than StartDate." });

                await using var tx = await _db.Database.BeginTransactionAsync(ct);

                // 1) Validate LeadResearcherId (existing only)
                int? leadResearcherId = request.LeadResearcherId;
                if (leadResearcherId.HasValue)
                {
                    var leadExists = await _db.Set<ResearcherCredentials>()
                        .AsNoTracking()
                        .AnyAsync(r => r.ResearcherId == leadResearcherId.Value, ct);

                    if (!leadExists)
                        return NotFound(new { message = $"Lead researcher {leadResearcherId.Value} not found." });
                }

                // 2) Validate and load StudySites (existing only)

                var siteIds = (request.StudySiteIds ?? new List<int>()).Distinct().ToList();
                var sitesToAssign = new List<StudySites>();

                if (siteIds.Count > 0)
                {
                    sitesToAssign = await _db.StudySites
                        .Where(s => siteIds.Contains(s.SiteID))
                        .ToListAsync(ct);

                    var missing = siteIds.Except(sitesToAssign.Select(s => s.SiteID)).ToList();
                    if (missing.Count > 0)
                    {
                        return NotFound(new
                        {
                            message = "One or more StudySites do not exist.",
                            missingSiteIds = missing
                        });
                    }

                    // OPTIONAL: if you want to forbid reassignment, use this block:
                    // var alreadyAttached = sitesToAssign
                    //     .Where(s => s.ProtocolID.HasValue)
                    //     .Select(s => new { s.SiteID, s.ProtocolID })
                    //     .ToList();
                    // if (alreadyAttached.Count > 0)
                    // {
                    //     return Conflict(new
                    //     {
                    //         message = "Some StudySites are already linked to another protocol. Reassignment is not allowed.",
                    //         sites = alreadyAttached
                    //     });
                    // }
                }


                // 3) Create Protocol (scalar-only)

                var protocol = _mapper.Map<TrialProtocols>(request);
                protocol.LeadResearcherId = leadResearcherId;

                _db.TrialsProtocols.Add(protocol);
                await _db.SaveChangesAsync(ct); // obtain ProtocolID

                // Assign existing StudySites to this protocol (reassign allowed)
                if (sitesToAssign.Count > 0)
                {
                    foreach (var site in sitesToAssign)
                    {
                        site.ProtocolID = protocol.ProtocolID; // assign/reassign to new protocol
                    }
                    await _db.SaveChangesAsync(ct);
                }


                await tx.CommitAsync(ct);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = protocol.ProtocolID },
                    new { message = "Protocol created successfully", protocolId = protocol.ProtocolID });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "Database error while creating protocol." });
            }
            catch
            {
                return StatusCode(500, new { message = "Failed to create protocol." });
            }
        }

        /// <summary>
        /// Get a protocol by id (includes counts and related IDs in ReadProtocolDto).
        /// GET: /api/protocols/{id}
        /// </summary>
        [HttpGet("{id:int}", Name = nameof(GetById))]
        [ProducesResponseType(typeof(ReadProtocolDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ReadProtocolDto>> GetById(int id, CancellationToken ct)
        {
            try
            {
                // Use your AutoMapper mapping TrialProtocols -> ReadProtocolDto
                var dto = await _db.TrialsProtocols
                    .AsNoTracking()
                    .Where(tp => tp.ProtocolID == id)
                    .ProjectTo<ReadProtocolDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(ct);

                if (dto == null)
                    return NotFound(new { message = $"Protocol {id} not found." });

                return Ok(dto);
            }
            catch
            {
                return StatusCode(500, new { message = "Failed to fetch protocol." });
            }
        }

        /// <summary>
        /// Count protocols where Status = 'ACTIVE' (case-insensitive).
        /// GET: /api/protocols/count/active
        /// </summary>
        [HttpGet("count/active")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<object>> GetCountOfActiveStatusProtocols(CancellationToken ct)
        {
            try
            {
                var count = await _db.TrialsProtocols
                    .AsNoTracking()
                    .CountAsync(p => p.Status.ToUpper() == "ACTIVE", ct);

                return Ok(new { ActiveProtocols = count });
            }
            catch
            {
                return StatusCode(500, new { message = "Failed to count active protocols." });
            }
        }

        /// <summary>
        /// Total count of protocols.
        /// GET: /api/protocols/count/total
        /// </summary>
        [HttpGet("count/total")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<object>> GetTotalCountOfProtocols(CancellationToken ct)
        {
            try
            {
                var total = await _db.TrialsProtocols
                    .AsNoTracking()
                    .CountAsync(ct);

                return Ok(new { TotalProtocols = total });
            }
            catch
            {
                return StatusCode(500, new { message = "Failed to count protocols." });
            }
        }

        /// <summary>
        /// Create a researcher (investigator) record.
        /// POST: /api/protocols/researchers
        /// </summary>
        [HttpPost("researchers")]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<object>> CreateResearcher([FromBody] CreateResearcherDto dto, CancellationToken ct)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ValidationProblem(ModelState);

                // Prevent duplicate email
                var exists = await _db.Set<ResearcherCredentials>()
                    .AsNoTracking()
                    .AnyAsync(r => r.Email == dto.Email, ct);

                if (exists)
                    return Conflict(new { message = "A researcher with this email already exists." });

                var entity = _mapper.Map<ResearcherCredentials>(dto);

                // Your model requires PasswordHash; mapping ignores it -> set empty string here
                entity.PasswordHash = string.Empty;

                _db.Add(entity);
                await _db.SaveChangesAsync(ct);

                var mini = _mapper.Map<ResearcherMiniDto>(entity);

                return Created(string.Empty, new
                {
                    message = "Investigator created successfully",
                    researcher = mini
                });
            }
            catch
            {
                return StatusCode(500, new { message = "Failed to create investigator" });
            }
        }
    }
}