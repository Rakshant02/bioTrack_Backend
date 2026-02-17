using BioTrack.Server.Data;
using BioTrack.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace BioTrack.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogsController : ControllerBase
    {

        private readonly BioDataContext _db;


        public AuditLogsController(BioDataContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken ct)
        {
            try
            {
                var logs = await _db.AuditLogs
                    .AsNoTracking()
                    .OrderByDescending(x => x.Timestamp)
                    .ToListAsync(ct);
                return Ok(logs);
            }
            catch (OperationCanceledException)
            {
                return Problem(statusCode: 499, title: "Client Closed Request");
            }
            catch
            {
                return Problem("An error occurred while retrieving audit logs.");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync(int id, CancellationToken ct)
        {
            try
            {
                var log = await _db.AuditLogs.FindAsync(new object?[] { id }, ct);
                if (log is null) return NotFound();
                return Ok(log);
            }
            catch (OperationCanceledException)
            {
                return Problem(statusCode: 499, title: "Client Closed Request");
            }
            catch
            {
                return Problem($"An error occurred while retrieving audit log #{id}.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] AuditLogs newLog, CancellationToken ct)
        {
            try
            {
                if (newLog is null) return BadRequest("Body is required.");
                if (string.IsNullOrWhiteSpace(newLog.Action)) return BadRequest("Action is required.");
                if (newLog.Action.Length > 50) newLog.Action = newLog.Action[..50];
                if (!string.IsNullOrWhiteSpace(newLog.User) && newLog.User.Length > 256)
                    newLog.User = newLog.User[..256];

                if (newLog.Timestamp.Kind == DateTimeKind.Unspecified)
                    newLog.Timestamp = DateTime.SpecifyKind(newLog.Timestamp, DateTimeKind.Utc);
                else if (newLog.Timestamp.Kind == DateTimeKind.Local)
                    newLog.Timestamp = newLog.Timestamp.ToUniversalTime();

                await _db.AuditLogs.AddAsync(newLog, ct);
                await _db.SaveChangesAsync(ct);

                return CreatedAtAction(nameof(GetByIdAsync), new { id = newLog.LogId }, newLog);
            }
            catch (OperationCanceledException)
            {
                return Problem(statusCode: 499, title: "Client Closed Request");
            }
            catch (DbUpdateException)
            {
                return Problem("Database update failed while creating audit log.");
            }
            catch
            {
                return Problem("An error occurred while creating an audit log.");
            }
        }

        [HttpGet("count")]
                    public async Task<IActionResult> CountAsync(CancellationToken ct)
        {
            try
            {
                var total = await _db.AuditLogs.CountAsync(ct);
                return Ok(total);
    }
            catch (OperationCanceledException)
            {
                return Problem(statusCode: 499, title: "Client Closed Request");
}
            catch
            {
    return Problem("An error occurred while counting audit logs.");
}


        }
    }
}
