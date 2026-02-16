using AutoMapper;
using AutoMapper.QueryableExtensions;
using BioTrack.Server.Data;
using BioTrack.Server.Dtos;
using BioTrack.Server.DTOs;
using BioTrack.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BioTrack.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudySitesController : ControllerBase
    {
        private readonly BioDataContext _db;
        private readonly ILogger<StudySitesController> _logger;
        private readonly IMapper _mapper;

        public StudySitesController(BioDataContext db, ILogger<StudySitesController> logger, IMapper mapper)
        {
            _db = db;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all study sites (projected via AutoMapper).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                // ProjectTo translates to SQL selecting only needed columns
                var sites = await _db.StudySites
                    .AsNoTracking()
                    .Include(s => s.PrincipalInvestigator) // still okay; ProjectTo can also handle if you configure projection-only
                    .ProjectTo<StudySiteReadDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return Ok(sites);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching StudySites");
                return StatusCode(500, "An error occurred while fetching study sites.");
            }
        }

        /// <summary>
        /// Create a study site.
        /// Uses the first available TrialProtocol and matches PI by InvestigatorName (FullName).
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] StudySiteCreateDto dto)
        {
            if (dto == null)
                return BadRequest("Request body is required.");

            if (string.IsNullOrWhiteSpace(dto.Location))
                return BadRequest("Location is required.");

            if (string.IsNullOrWhiteSpace(dto.InvestigatorName))
                return BadRequest("InvestigatorName is required.");

            try
            {
                // 1) Get a protocol (require one because ProtocolID is [Required])
                var protocol = await _db.TrialsProtocols
                    .AsNoTracking()
                    .OrderBy(p => p.ProtocolID)
                    .FirstOrDefaultAsync();

                if (protocol == null)
                    return BadRequest("No TrialProtocol found. Create a protocol before creating a site.");

                // 2) Find PI by FullName (case-insensitive)
                var normalized = dto.InvestigatorName.Trim().ToLower();
                var pi = await _db.Set<ResearcherCredentials>()
                    .FirstOrDefaultAsync(r => r.FullName.ToLower() == normalized);

                if (pi == null)
                    return NotFound($"No Researcher found with FullName '{dto.InvestigatorName}'. Create the researcher first or use an existing name.");

                // 3) Map DTO -> Entity and set required foreign keys
                var entity = _mapper.Map<StudySites>(dto);
                entity.ProtocolID = protocol.ProtocolID;
                entity.PrincipalInvestigatorId = pi.ResearcherId;
                entity.Location = dto.Location.Trim();
                entity.InvestigatorName = dto.InvestigatorName.Trim();

                _db.StudySites.Add(entity);
                await _db.SaveChangesAsync();

                // 4) Load with PI for return, then map to Read DTO
                await _db.Entry(entity).Reference(s => s.PrincipalInvestigator).LoadAsync();
                var readDto = _mapper.Map<StudySiteReadDto>(entity);

                // Return 201 with resource
                return CreatedAtAction(nameof(GetAll), new { id = entity.SiteID }, readDto);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB update error while creating StudySite. payload={@dto}", dto);
                return BadRequest("Could not create StudySite due to a database constraint. Verify SiteID/identity and foreign keys.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating StudySite. payload={@dto}", dto);
                return StatusCode(500, "An unexpected error occurred while creating the study site.");
            }
        }

        /// <summary>
        /// Returns the total number of study sites.
        /// </summary>
        [HttpGet("count")]
        public async Task<IActionResult> CountStudySites()
        {
            try
            {
                var count = await _db.StudySites.AsNoTracking().CountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while counting StudySites");
                return StatusCode(500, "An error occurred while counting study sites.");
            }
        }
    }
}