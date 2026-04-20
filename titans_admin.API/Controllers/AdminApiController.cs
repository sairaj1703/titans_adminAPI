using Microsoft.AspNetCore.Mvc;
using titans_admin.Models.ViewModels;
using titans_admin.Services.Interfaces;

namespace titans_admin.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminApiController(IAdminService adminService, ILogger<AdminApiController> logger) : ControllerBase
{
    #region Dashboard

    [HttpGet("dashboard")]
    public async Task<ActionResult<AdminDashboardViewModel>> GetDashboard()
    {
        try
        {
            var dashboard = await adminService.GetDashboardStatsAsync();
            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching dashboard stats");
            return StatusCode(500, new { message = "An error occurred while fetching dashboard statistics" });
        }
    }

    [HttpGet("violations-count")]
    public async Task<ActionResult<int>> GetViolationsCount()
    {
        try
        {
            var count = await adminService.GetViolationCounterAsync();
            return Ok(new { violationCount = count });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching violations count");
            return StatusCode(500, new { message = "An error occurred while fetching violations count" });
        }
    }

    #endregion
}
