using Microsoft.AspNetCore.Mvc;
using titans_admin.Models.ViewModels;
using titans_admin.Services.Interfaces;

namespace titans_admin.API.Controllers;

/// <summary>
/// Trade program management endpoints for CRUD operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TradeProgramsController(IAdminService adminService, ILogger<TradeProgramsController> logger) : ControllerBase
{
    /// <summary>
    /// Get all trade programs
    /// </summary>
    /// <returns>List of all trade programs</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<TradeProgramListViewModel>>> GetAllTradePrograms()
    {
        try
        {
            var programs = await adminService.GetAllTradeProgramsAsync();
            return Ok(programs);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching all trade programs");
            return StatusCode(500, new { message = "An error occurred while fetching trade programs" });
        }
    }

    /// <summary>
    /// Get all active trade programs
    /// </summary>
    /// <returns>List of active trade programs</returns>
    [HttpGet("active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<TradeProgramListViewModel>>> GetActiveTradePrograms()
    {
        try
        {
            var programs = await adminService.GetActiveTradeProgramsAsync();
            return Ok(programs);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching active trade programs");
            return StatusCode(500, new { message = "An error occurred while fetching active trade programs" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TradeProgramEditViewModel>> GetTradeProgramById(int id)
    {
        try
        {
            var program = await adminService.GetTradeProgramByIdAsync(id);
            if (program == null)
                return NotFound(new { message = "Trade program not found" });

            return Ok(program);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching trade program {ProgramId}", id);
            return StatusCode(500, new { message = "An error occurred while fetching the trade program" });
        }
    }

    [HttpPost]
    public async Task<ActionResult<int>> CreateTradeProgram([FromBody] TradeProgramEditViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, errorMessage, programId) = await adminService.CreateTradeProgramAsync(model);

            if (!success)
                return BadRequest(new { message = errorMessage ?? "Failed to create trade program" });

            return CreatedAtAction(nameof(GetTradeProgramById), new { id = programId }, new { programId });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating trade program {ProgramName}", model.ProgramName);
            return StatusCode(500, new { message = "An error occurred while creating the trade program" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTradeProgram(int id, [FromBody] TradeProgramEditViewModel model)
    {
        try
        {
            if (id != model.TradeProgramId)
                return BadRequest(new { message = "Trade Program ID mismatch" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await adminService.UpdateTradeProgramAsync(model);

            if (!success)
                return BadRequest(new { message = "Failed to update trade program" });

            return Ok(new { message = "Trade program updated successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating trade program {ProgramId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the trade program" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTradeProgram(int id)
    {
        try
        {
            var success = await adminService.DeleteTradeProgramAsync(id);

            if (!success)
                return BadRequest(new { message = "Failed to delete trade program" });

            return Ok(new { message = "Trade program deleted successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting trade program {ProgramId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the trade program" });
        }
    }
}
