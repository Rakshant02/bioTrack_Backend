using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BioTrack.Server.Data;
using BioTrack.Server.Models;
using BioTrack.Server.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace BioTrack.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantsController : ControllerBase
    {
        private readonly BioDataContext _context;
        private readonly IMapper _mapper;

        public ParticipantsController(BioDataContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // GET: api/participants/getAllParticipants
        [HttpGet("getAllParticipants")]
        [ProducesResponseType(typeof(List<ParticipantsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllParticipants(CancellationToken cancellationToken = default)
        {
            try
            {
                var list = await _context.Participants
                    .AsNoTracking()
                    .OrderBy(p => p.ParticipantID)
                    .ToListAsync(cancellationToken);

                var participantDtos = _mapper.Map<List<ParticipantsDto>>(list);
                return Ok(participantDtos);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred while retrieving participants." });
            }
        }

        // GET: api/participants/getParticipant/5
        [HttpGet("getParticipant/{id:int}")]
        [ProducesResponseType(typeof(ParticipantsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetParticipantById(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var participant = await _context.Participants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.ParticipantID == id, cancellationToken);

                if (participant is null)
                    return NotFound(new { message = $"Participant with id '{id}' was not found." });

                var participantDto = _mapper.Map<ParticipantsDto>(participant);
                return Ok(participantDto);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred while retrieving the participant." });
            }
        }

        // GET: api/participants/getParticipantByName/john
        [HttpGet("getParticipantByName/{name}")]
        [ProducesResponseType(typeof(List<ParticipantsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetParticipantsByName(string name, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return BadRequest(new { message = "Name must be provided." });

                var normalized = name.Trim();

                var participants = await _context.Participants
                    .AsNoTracking()
                    .Where(p => p.Name != null && EF.Functions.Like(p.Name, $"%{normalized}%"))
                    .OrderBy(p => p.Name)
                    .ToListAsync(cancellationToken);

                var participantDtos = _mapper.Map<List<ParticipantsDto>>(participants);
                return Ok(participantDtos);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred while retrieving participants by name." });
            }
        }

        // POST: api/participants/createParticipant
        [HttpPost("createParticipant")]
        [ProducesResponseType(typeof(ParticipantsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateParticipant(
            [FromBody] CreateParticipantDto createDto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (createDto is null)
                    return BadRequest(new { message = "Participant payload is required." });

                if (!ModelState.IsValid)
                    return ValidationProblem(ModelState);

                // FK checks: Protocol (required)
                var protocolExists = await _context.TrialsProtocols
                    .AsNoTracking()
                    .AnyAsync(tp => tp.ProtocolID == createDto.ProtocolID, cancellationToken);

                if (!protocolExists)
                    return NotFound(new { message = $"Protocol {createDto.ProtocolID} not found." });

                // FK checks: Site (optional)
                if (createDto.SiteID.HasValue)
                {
                    var siteExists = await _context.StudySites
                        .AsNoTracking()
                        .AnyAsync(s => s.SiteID == createDto.SiteID.Value, cancellationToken);

                    if (!siteExists)
                        return NotFound(new { message = $"Site {createDto.SiteID.Value} not found." });
                }

                var participant = _mapper.Map<Participants>(createDto);

                await _context.Participants.AddAsync(participant, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                var participantDto = _mapper.Map<ParticipantsDto>(participant);
                return CreatedAtAction(nameof(GetParticipantById), new { id = participant.ParticipantID }, participantDto);
            }
            catch (DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "A database error occurred while creating the participant." });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred while creating the participant." });
            }
        }

        // PUT: api/participants/updateParticipant/{id}
        [HttpPut("updateParticipant/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateParticipant(
            int id,
            [FromBody] UpdateParticipantsDto updateDto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (updateDto is null)
                    return BadRequest(new { message = "Participant payload is required." });

                if (!ModelState.IsValid)
                    return ValidationProblem(ModelState);

                // Optional: enforce route/body id consistency since Update DTO contains ParticipantID
                if (updateDto.ParticipantID != id)
                    return BadRequest(new { message = "Route id and body ParticipantID do not match." });

                // FK checks: Protocol exists (if changing)
                var protocolExists = await _context.TrialsProtocols
                    .AsNoTracking()
                    .AnyAsync(tp => tp.ProtocolID == updateDto.ProtocolID, cancellationToken);

                if (!protocolExists)
                    return NotFound(new { message = $"Protocol {updateDto.ProtocolID} not found." });

                // FK checks: Site (optional)
                if (updateDto.SiteID.HasValue)
                {
                    var siteExists = await _context.StudySites
                        .AsNoTracking()
                        .AnyAsync(s => s.SiteID == updateDto.SiteID.Value, cancellationToken);

                    if (!siteExists)
                        return NotFound(new { message = $"Site {updateDto.SiteID.Value} not found." });
                }

                var participant = await _context.Participants
                    .FirstOrDefaultAsync(p => p.ParticipantID == id, cancellationToken);

                if (participant is null)
                    return NotFound(new { message = $"Participant with id '{id}' was not found." });

                // Map updates onto the tracked entity
                _mapper.Map(updateDto, participant);

                // No need to call _context.Participants.Update(participant) for tracked entities
                await _context.SaveChangesAsync(cancellationToken);

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "A database error occurred while updating the participant." });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred while updating the participant." });
            }
        }

        // DELETE: api/participants/removeParticipant/5
        [HttpDelete("removeParticipant/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveParticipant(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var participant = await _context.Participants
                    .FirstOrDefaultAsync(p => p.ParticipantID == id, cancellationToken);

                if (participant is null)
                    return NotFound(new { message = $"Participant with id '{id}' was not found." });

                _context.Participants.Remove(participant);
                await _context.SaveChangesAsync(cancellationToken);

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "A database error occurred while removing the participant." });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred while removing the participant." });
            }
        }

        // GET: api/participants/totalParticipants
        [HttpGet("totalParticipants")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> TotalParticipants(CancellationToken cancellationToken = default)
        {
            try
            {
                var total = await _context.Participants.CountAsync(cancellationToken);
                return Ok(new { totalParticipants = total });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred while retrieving total participants." });
            }
        }

        // GET: api/participants/enrolledParticipants
        [HttpGet("enrolledParticipants")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EnrolledParticipants(CancellationToken cancellationToken = default)
        {
            try
            {
                var total = await _context.Participants
                    .CountAsync(p => p.Status != null && p.Status.ToUpper() == "ENROLLED", cancellationToken);

                return Ok(new { enrolledParticipants = total });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred while retrieving enrolled participants." });
            }
        }

        // GET: api/participants/withdrawnParticipants
        [HttpGet("withdrawnParticipants")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> WithdrawnParticipants(CancellationToken cancellationToken = default)
        {
            try
            {
                var total = await _context.Participants
                    .CountAsync(p => p.Status != null && p.Status.ToUpper() == "WITHDRAWN", cancellationToken);

                return Ok(new { withdrawnParticipants = total });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred while retrieving withdrawn participants." });
            }
        }

        // GET: api/participants/getParticipantsByProtocol?protocolId=123&phase=1
        [HttpGet("getParticipantsByProtocol")]
        public async Task<IActionResult> GetParticipantsByProtocol(
       [FromQuery] int protocolId,
       [FromQuery] int phase,
       CancellationToken ct = default)
        {
            try
            {
                if (protocolId <= 0)
                    return BadRequest(new { message = "protocolId must be a positive integer." });

                if (phase <= 0 /* optionally: || phase > 3 */)
                    return BadRequest(new { message = "phase must be a positive integer." });

                // Ensure the protocol exists (optional but user-friendly)
                var protocolExists = await _context.TrialsProtocols
                    .AsNoTracking()
                    .AnyAsync(tp => tp.ProtocolID == protocolId, ct);

                if (!protocolExists)
                    return NotFound(new { message = $"Protocol {protocolId} not found." });

                // Filter via navigation property (EF will translate to an INNER JOIN)
                var participants = await _context.Participants
                    .AsNoTracking()
                    .Where(p => p.ProtocolID == protocolId && p.TrialProtocol.Phase == phase)
                    .OrderBy(p => p.ParticipantID)
                    .ToListAsync(ct);

                var participantDtos = _mapper.Map<List<ParticipantsDto>>(participants);
                return Ok(participantDtos);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred while retrieving participants by protocol and phase." });
            }
        }
        

        /// Create/append a consent record for the participant.
        /// POST /api/participants/{participantId}/consents
        /// </summary>
        [HttpPost("{participantId:int}/consents")]
        public async Task<ActionResult<ReadConsent>> CreateConsent(int participantId, [FromBody] CreateConsent request)
        {
            try
            {
                // Ensure route & body align (avoid tampering)
                if (request.ParticipantID != 0 && request.ParticipantID != participantId)
                {
                    return BadRequest(new { message = "ParticipantID in body must match the route or be omitted." });
                }

                // Validate participant exists
                var exists = await _context.Participants
                    .AsNoTracking()
                    .AnyAsync(p => p.ParticipantID == participantId);

                if (!exists)
                    return NotFound(new { message = $"Participant {participantId} not found." });

                // Map request -> entity (model has only ParticipantID, Status)
                var consent = _mapper.Map<ConsentForm>(request);
                consent.ParticipantID = participantId;

                _context.ConsentForms.Add(consent);
                await _context.SaveChangesAsync();

                var dto = _mapper.Map<ReadConsent>(consent);
                return CreatedAtAction(nameof(GetConsentHistory), new { participantId }, dto);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Failed to create consent." });
            }
        }

        /// <summary>
        /// Get consent history for a participant (newest first by ConsentID).
        /// GET /api/participants/{participantId}/consents
        /// </summary>
        [HttpGet("{participantId:int}/consents")]
        public async Task<ActionResult<IEnumerable<ReadConsent>>> GetConsentHistory(int participantId)
        {
            try
            {
                // Ensure participant exists
                var exists = await _context.Participants
                    .AsNoTracking()
                    .AnyAsync(p => p.ParticipantID == participantId);

                if (!exists)
                    return NotFound(new { message = $"Participant {participantId} not found." });

                // Since model has no timestamps/versions, sort by ConsentID (descending)
                var history = await _context.ConsentForms
                    .AsNoTracking()
                    .Where(c => c.ParticipantID == participantId)
                    .OrderByDescending(c => c.ConsentID)
                    .ProjectTo<ReadConsent>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return Ok(history);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Failed to fetch consent history." });
            }

        }

        }
    }