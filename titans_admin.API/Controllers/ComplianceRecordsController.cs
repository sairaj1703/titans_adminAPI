using Microsoft.AspNetCore.Mvc;
using titans_admin.Models.ViewModels;
using titans_admin.Services.Interfaces;

namespace titans_admin.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComplianceRecordsController(IAdminService adminService, ILogger<ComplianceRecordsController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ComplianceRecordItemViewModel>>> GetAllComplianceRecords()
    {
        try
        {
            var records = await adminService.GetAllComplianceRecordsAsync();
            return Ok(records);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching all compliance records");
            return StatusCode(500, new { message = "An error occurred while fetching compliance records" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ComplianceRecordEditViewModel>> GetComplianceRecordById(int id)
    {
        try
        {
            var record = await adminService.GetComplianceRecordByIdAsync(id);
            if (record == null)
                return NotFound(new { message = "Compliance record not found" });

            return Ok(record);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching compliance record {RecordId}", id);
            return StatusCode(500, new { message = "An error occurred while fetching the compliance record" });
        }
    }

    [HttpPost]
    public async Task<ActionResult<int>> CreateComplianceRecord([FromBody] ComplianceRecordEditViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, errorMessage, recordId) = await adminService.CreateComplianceRecordAsync(model);

            if (!success)
                return BadRequest(new { message = errorMessage ?? "Failed to create compliance record" });

            return CreatedAtAction(nameof(GetComplianceRecordById), new { id = recordId }, new { recordId });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating compliance record for program {ProgramId}", model.TradeProgramId);
            return StatusCode(500, new { message = "An error occurred while creating the compliance record" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComplianceRecord(int id, [FromBody] ComplianceRecordEditViewModel model)
    {
        try
        {
            if (id != model.ComplianceRecordId)
                return BadRequest(new { message = "Compliance Record ID mismatch" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await adminService.UpdateComplianceRecordAsync(model);

            if (!success)
                return BadRequest(new { message = "Failed to update compliance record" });

            return Ok(new { message = "Compliance record updated successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating compliance record {RecordId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the compliance record" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComplianceRecord(int id)
    {
        try
        {
            var success = await adminService.DeleteComplianceRecordAsync(id);

            if (!success)
                return BadRequest(new { message = "Failed to delete compliance record" });

            return Ok(new { message = "Compliance record deleted successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting compliance record {RecordId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the compliance record" });
        }
    }

    [HttpGet("tracking")]
    public async Task<ActionResult<ComplianceTrackingViewModel>> GetComplianceTracking()
    {
        try
        {
            var tracking = await adminService.GetComplianceTrackingAsync();
            return Ok(tracking);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching compliance tracking");
            return StatusCode(500, new { message = "An error occurred while fetching compliance tracking data" });
        }
    }
}
