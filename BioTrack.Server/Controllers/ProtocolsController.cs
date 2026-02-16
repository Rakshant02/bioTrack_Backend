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

                // Disallow ambiguous input (both provided)
                if (request.LeadResearcherId.HasValue && request.NewInvestigator != null)
                    return BadRequest(new { message = "Provide either LeadResearcherId or NewInvestigator, not both." });

                await using var tx = await _db.Database.BeginTransactionAsync(ct);

                // Resolve leadResearcherId: either use existing, or create a new investigator
                int? leadResearcherId = request.LeadResearcherId;

                if (request.NewInvestigator != null)
                {
                    // prevent duplicate email
                    var emailExists = await _db.Set<ResearcherCredentials>()
                        .AsNoTracking()
                        .AnyAsync(r => r.Email == request.NewInvestigator.Email, ct);

                    if (emailExists)
                        return Conflict(new { message = "Investigator with this email already exists. Use LeadResearcherId instead." });

                    var newR = _mapper.Map<ResearcherCredentials>(request.NewInvestigator);
                    // Your mapping ignores PasswordHash and your model requires it => set to empty
                    newR.PasswordHash = string.Empty;

                    _db.Add(newR);
                    await _db.SaveChangesAsync(ct);
                    leadResearcherId = newR.ResearcherId;
                }
                else if (leadResearcherId.HasValue)
                {
                    var exists = await _db.Set<ResearcherCredentials>()
                        .AsNoTracking()
                        .AnyAsync(r => r.ResearcherId == leadResearcherId.Value, ct);
                    if (!exists)
                        return NotFound(new { message = $"Lead researcher {leadResearcherId.Value} not found." });
                }

                // Create protocol
                var protocol = _mapper.Map<TrialProtocols>(request);
                protocol.LeadResearcherId = leadResearcherId;

                _db.TrialsProtocols.Add(protocol);
                await _db.SaveChangesAsync(ct); // obtain ProtocolID

                // Attach study sites (if any)
                if (request.StudySites != null && request.StudySites.Count > 0)
                {
                    // Because StudySites.PrincipalInvestigatorId is REQUIRED and StudySiteCreateDto lacks this,
                    // we default PI to the protocol's LeadResearcherId. If none, we cannot continue.
                    if (!leadResearcherId.HasValue)
                        return BadRequest(new { message = "To create study sites during protocol creation, provide a LeadResearcher (or add PrincipalInvestigatorId to StudySiteCreateDto)." });

                    foreach (var siteDto in request.StudySites)
                    {
                        var site = _mapper.Map<StudySites>(siteDto);

                        // Enforce protocol linkage & required PI
                        site.ProtocolID = protocol.ProtocolID;
                        site.PrincipalInvestigatorId = leadResearcherId.Value;

                        _db.StudySites.Add(site);
                    }

                    await _db.SaveChangesAsync(ct);
                }

                await tx.CommitAsync(ct);

                // 201 Created with Location header
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = protocol.ProtocolID },
                    new { message = "Protocol created successfully", protocolId = protocol.ProtocolID });
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