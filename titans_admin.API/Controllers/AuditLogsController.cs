using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using titans_admin.Models.ViewModels;
using titans_admin.Services.Interfaces;

namespace titans_admin.API.Controllers;

/// <summary>
/// Audit Logs API Controller - Retrieves system audit trail information
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuditLogsController(IAdminService adminService, ILogger<AuditLogsController> logger) : ControllerBase
{
    /// <summary>
    /// Get all audit logs with pagination
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50, max: 100)</param>
    /// <returns>Paginated list of audit logs</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Get audit logs for a specific user with pagination
    /// </summary>
    /// <param name="userId">User ID to filter logs</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50, max: 100)</param>
    /// <returns>Paginated list of user-specific audit logs</returns>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
