using Microsoft.AspNetCore.Mvc;
using titans_admin.Models.ViewModels;
using titans_admin.Services.Interfaces;

namespace titans_admin.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditLogsController(IAdminService adminService, ILogger<AuditLogsController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AuditLogViewModel>>> GetAuditLogs([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
    {
        try
        {
            if (pageNumber < 1)
                return BadRequest(new { message = "Page number must be greater than 0" });

            if (pageSize < 1 || pageSize > 100)
                return BadRequest(new { message = "Page size must be between 1 and 100" });

            var logs = await adminService.GetAuditLogsAsync(pageNumber, pageSize);
            return Ok(logs);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching audit logs");
            return StatusCode(500, new { message = "An error occurred while fetching audit logs" });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<AuditLogViewModel>>> GetAuditLogsByUser(int userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
    {
        try
        {
            if (pageNumber < 1)
                return BadRequest(new { message = "Page number must be greater than 0" });

            if (pageSize < 1 || pageSize > 100)
                return BadRequest(new { message = "Page size must be between 1 and 100" });

            var logs = await adminService.GetAuditLogsByUserAsync(userId, pageNumber, pageSize);
            return Ok(logs);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching audit logs for user {UserId}", userId);
            return StatusCode(500, new { message = "An error occurred while fetching audit logs" });
        }
    }
}
