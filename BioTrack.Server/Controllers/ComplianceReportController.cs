using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioTrack.Server.Data;
using BioTrack.Server.DTOs;
using BioTrack.Server.Models;

namespace BioTrack.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComplianceReportsController : ControllerBase
    {
        private readonly BioDataContext _db;
        private readonly IMapper _mapper;

        public ComplianceReportsController(BioDataContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        /// <summary>
        /// Get current deviation count for a protocol (live from ProtocolDeviations)
        /// GET /api/compliance-reports/count/5
        /// </summary>
        [HttpGet("count/{protocolId:int}")]
        public async Task<ActionResult<object>> GetCount(int protocolId)
        {
            var exists = await _db.TrialsProtocols
                .AsNoTracking()
                .AnyAsync(tp => tp.ProtocolID == protocolId);

            if (!exists)
                return NotFound(new { message = $"Protocol {protocolId} not found." });

            var deviationCount = await _db.ProtocolDeviations
                .AsNoTracking()
                .Where(d => d.ProtocolID == protocolId)
                .CountAsync();

            return Ok(new { ProtocolID = protocolId, DeviationCount = deviationCount });
        }

        /// <summary>
        /// Get all saved compliance reports (snapshots)
        /// GET /api/compliance-reports
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadComplianceReportDto>>> GetAll()
        {
            var list = await _db.ComplianceReports
                .AsNoTracking()
                .OrderByDescending(cr => cr.GeneratedDate)
                .ProjectTo<ReadComplianceReportDto>(_mapper.ConfigurationProvider) // AutoMapper projection
                .ToListAsync();

            return Ok(list);
        }

        /// <summary>
        /// Create a compliance report snapshot for a protocol.
        /// POST /api/compliance-reports
        /// Body: { "protocolID": 5, "adherenceRate": 97.5 }
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ReadComplianceReportDto>> CreateComplianceReport([FromBody] CreateComplianceReport request)
        {
            // Optional model validation
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            // Validate protocol exists
            var exists = await _db.TrialsProtocols
                .AsNoTracking()
                .AnyAsync(tp => tp.ProtocolID == request.ProtocolID);

            if (!exists)
                return NotFound(new { message = $"Protocol {request.ProtocolID} not found." });

            // Compute current deviation count for the protocol
            var deviationCount = await _db.ProtocolDeviations
                .AsNoTracking()
                .Where(d => d.ProtocolID == request.ProtocolID)
                .CountAsync();

            // Map Create DTO -> Entity
            var report = _mapper.Map<ComplianceReports>(request);

            // Set business fields
            report.DeviationCount = deviationCount;
            report.GeneratedDate = DateTime.UtcNow;

            _db.ComplianceReports.Add(report);
            await _db.SaveChangesAsync();

            // Map Entity -> Read DTO
            var dto = _mapper.Map<ReadComplianceReportDto>(report);

            return CreatedAtAction(nameof(GetAll), new { id = dto.ReportID }, dto);
        }

        [HttpGet("total")]
        public async Task<ActionResult<object>> GetTotalComplianceReports()
        {
            try
            {
                // Defensive check (rare but protects against misconfigured DbContext)
                if (_db.ComplianceReports == null)
                {
                    return StatusCode(500, new { message = "ComplianceReports table is not available." });
                }

                // Main logic
                var total = await _db.ComplianceReports
                    .AsNoTracking()
                    .CountAsync();

                return Ok(new { TotalComplianceReports = total });
            }
            catch (Exception)
            {
                // Generic safety buffer — keep it simple as you requested
                return StatusCode(500, new { message = "An error occurred while retrieving compliance report count." });
            }

        }
    }
    }