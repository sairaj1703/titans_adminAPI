using Microsoft.AspNetCore.Mvc;
using titans_admin.Models.ViewModels;
using titans_admin.Services.Interfaces;

namespace titans_admin.API.Controllers;

/// <summary>
/// Trade license management endpoints for CRUD operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TradeLicensesController(IAdminService adminService, ILogger<TradeLicensesController> logger) : ControllerBase
{
    /// <summary>
    /// Get all trade licenses
    /// </summary>
    /// <returns>List of all trade licenses</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<TradeLicenseListViewModel>>> GetAllTradeLicenses()
    {
        try
        {
            var licenses = await adminService.GetAllTradeLicensesAsync();
            return Ok(licenses);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching all trade licenses");
            return StatusCode(500, new { message = "An error occurred while fetching trade licenses" });
        }
    }

    /// <summary>
    /// Get a trade license by ID
    /// </summary>
    /// <param name="id">The trade license ID</param>
    /// <returns>Trade license details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TradeLicenseEditViewModel>> GetTradeLicenseById(int id)
    {
        try
        {
            var license = await adminService.GetTradeLicenseByIdAsync(id);
            if (license == null)
                return NotFound(new { message = "Trade license not found" });

            return Ok(license);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching trade license {LicenseId}", id);
            return StatusCode(500, new { message = "An error occurred while fetching the trade license" });
        }
    }

    [HttpPost]
    public async Task<ActionResult<int>> CreateTradeLicense([FromBody] TradeLicenseEditViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, errorMessage, licenseId) = await adminService.CreateTradeLicenseAsync(model);

            if (!success)
                return BadRequest(new { message = errorMessage ?? "Failed to create trade license" });

            return CreatedAtAction(nameof(GetTradeLicenseById), new { id = licenseId }, new { licenseId });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating trade license {BusinessName}", model.BusinessName);
            return StatusCode(500, new { message = "An error occurred while creating the trade license" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTradeLicense(int id, [FromBody] TradeLicenseEditViewModel model)
    {
        try
        {
            if (id != model.TradeLicenseId)
                return BadRequest(new { message = "Trade License ID mismatch" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await adminService.UpdateTradeLicenseAsync(model);

            if (!success)
                return BadRequest(new { message = "Failed to update trade license" });

            return Ok(new { message = "Trade license updated successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating trade license {LicenseId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the trade license" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTradeLicense(int id)
    {
        try
        {
            var success = await adminService.DeleteTradeLicenseAsync(id);

            if (!success)
                return BadRequest(new { message = "Failed to delete trade license" });

            return Ok(new { message = "Trade license deleted successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting trade license {LicenseId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the trade license" });
        }
    }

    [HttpGet("by-status/{status}")]
    public async Task<ActionResult<List<TradeLicenseListViewModel>>> GetTradeLicensesByStatus(string status)
    {
        try
        {
            var licenses = await adminService.GetTradeLicensesByStatusAsync(status);
            return Ok(licenses);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching trade licenses by status {Status}", status);
            return StatusCode(500, new { message = "An error occurred while fetching trade licenses" });
        }
    }
}
