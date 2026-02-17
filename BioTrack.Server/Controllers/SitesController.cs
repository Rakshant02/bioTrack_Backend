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
                var sites = await _db.StudySites
                    .AsNoTracking()
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
        /// <summary>
        /// Create a study site under a specific protocol (body only contains location).
        /// POST: /api/studysites/create/{protocolId}
        /// </summary>
        // Controllers/StudySitesController.cs

        /// <summary>
        /// Create a study site WITHOUT assigning a protocol (body only contains location).
        /// POST: /api/studysites/create
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] StudySiteCreateDto dto, CancellationToken ct)
        {
            if (dto == null)
                return BadRequest("Request body is required.");

            if (string.IsNullOrWhiteSpace(dto.Location))
                return BadRequest("Location is required.");

            try
            {
                var entity = _mapper.Map<StudySites>(dto);
                entity.ProtocolID = null;
                entity.Location = dto.Location.Trim();

                _db.StudySites.Add(entity);
                await _db.SaveChangesAsync(ct);

                var readDto = _mapper.Map<StudySiteReadDto>(entity);
                return CreatedAtAction(nameof(GetAll), routeValues: null, value: readDto);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB update error while creating StudySite. payload={@dto}", dto);
                return BadRequest("Could not create StudySite due to a database constraint.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating StudySite. payload={@dto}", dto);
                return StatusCode(500, "An unexpected error occurred while creating the study site.");
            }
        }




    }
}