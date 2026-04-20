using Microsoft.AspNetCore.Mvc;

namespace titans_admin.API.Controllers;

/// <summary>
/// Health check endpoint for API availability monitoring
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Check the health status of the API
    /// </summary>
    /// <returns>Health status with timestamp</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get() => Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
}
