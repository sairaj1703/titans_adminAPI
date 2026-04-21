using Microsoft.AspNetCore.Mvc;
using titans_admin.Services.Interfaces;
using titans_admin.Models.ViewModels;

namespace titans_admin.Controllers;

/// <summary>
/// Administrator Controller - Handles all admin operations
/// </summary>
public class AdminController(IAdminService adminService, ILogger<AdminController> logger) : Controller
{
    #region Dashboard

    public async Task<IActionResult> Index()
    {
        var dashboardStats = await adminService.GetDashboardStatsAsync();
        return View(dashboardStats);
    }

    [HttpGet]
    public async Task<IActionResult> GetStats()
    {
        var stats = await adminService.GetDashboardStatsAsync();
        return Json(new
        {
            totalLicenses = stats.TotalTradeLicenses,
            activePrograms = stats.ActivePrograms,
            totalTransactions = stats.TotalTransactions,
            violations = stats.ViolationCounter
        });
    }

    #endregion

    #region User Management

    public async Task<IActionResult> Users() =>
        View(await adminService.GetAllUsersAsync());

    public IActionResult CreateUser() =>
        View(new CreateUserViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(CreateUserViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var (success, errorMessage, _) = await adminService.CreateUserAsync(model);

        if (success)
        {
            TempData["SuccessMessage"] = $"User '{model.Email}' created successfully.";
            return RedirectToAction(nameof(Users));
        }

        ModelState.AddModelError(string.Empty, errorMessage ?? "Failed to create user.");
        return View(model);
    }

    public async Task<IActionResult> EditUser(int id)
    {
        var user = await adminService.GetUserByIdAsync(id);
        return user == null ? NotFound() : View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(UserEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var success = await adminService.UpdateUserAsync(model);

        if (success)
        {
            TempData["SuccessMessage"] = "User updated successfully.";
            return RedirectToAction(nameof(Users));
        }

        ModelState.AddModelError(string.Empty, "Failed to update user.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var success = await adminService.DeleteUserAsync(id);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] = success
                ? "User removed successfully."
                : "Failed to delete user.";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting user {UserId}", id);
            TempData["ErrorMessage"] = "An error occurred while deleting the user.";
        }

        return RedirectToAction(nameof(Users));
    }

    [HttpGet]
    public async Task<IActionResult> GetUsersJson() =>
        Json(await adminService.GetAllUsersAsync());

    #endregion

    #region Trade Licenses

    public async Task<IActionResult> TradeLicenses() =>
        View(await adminService.GetAllTradeLicensesAsync());

    public async Task<IActionResult> CreateTradeLicense()
    {
        ViewBag.Users = await adminService.GetAllUsersAsync();
        return View(new TradeLicenseEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTradeLicense(TradeLicenseEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Users = await adminService.GetAllUsersAsync();
            return View(model);
        }

        var (success, errorMessage, _) = await adminService.CreateTradeLicenseAsync(model);

        if (success)
        {
            TempData["SuccessMessage"] = "Trade License created successfully.";
            return RedirectToAction(nameof(TradeLicenses));
        }

        ViewBag.Users = await adminService.GetAllUsersAsync();
        ModelState.AddModelError(string.Empty, errorMessage ?? "Failed to create trade license.");
        return View(model);
    }

    public async Task<IActionResult> EditTradeLicense(int id)
    {
        var license = await adminService.GetTradeLicenseByIdAsync(id);

        if (license == null)
            return NotFound();

        ViewBag.Users = await adminService.GetAllUsersAsync();
        return View(license);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTradeLicense(TradeLicenseEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Users = await adminService.GetAllUsersAsync();
            return View(model);
        }

        var success = await adminService.UpdateTradeLicenseAsync(model);

        if (success)
        {
            TempData["SuccessMessage"] = "Trade License updated successfully.";
            return RedirectToAction(nameof(TradeLicenses));
        }

        ViewBag.Users = await adminService.GetAllUsersAsync();
        ModelState.AddModelError(string.Empty, "Failed to update trade license.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTradeLicense(int id)
    {
        var success = await adminService.DeleteTradeLicenseAsync(id);
        TempData[success ? "SuccessMessage" : "ErrorMessage"] = success
            ? "Trade License deleted successfully."
            : "Failed to delete trade license.";

        return RedirectToAction(nameof(TradeLicenses));
    }

    #endregion

    #region Trade Programs

    public async Task<IActionResult> TradePrograms() =>
        View(await adminService.GetAllTradeProgramsAsync());

    public IActionResult CreateTradeProgram() =>
        View(new TradeProgramEditViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTradeProgram(TradeProgramEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var (success, errorMessage, _) = await adminService.CreateTradeProgramAsync(model);

        if (success)
        {
            TempData["SuccessMessage"] = $"Trade Program '{model.ProgramName}' created successfully.";
            return RedirectToAction(nameof(TradePrograms));
        }

        ModelState.AddModelError(string.Empty, errorMessage ?? "Failed to create trade program.");
        return View(model);
    }

    public async Task<IActionResult> EditTradeProgram(int id)
    {
        var program = await adminService.GetTradeProgramByIdAsync(id);
        return program == null ? NotFound() : View(program);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTradeProgram(TradeProgramEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var success = await adminService.UpdateTradeProgramAsync(model);

        if (success)
        {
            TempData["SuccessMessage"] = "Trade Program updated successfully.";
            return RedirectToAction(nameof(TradePrograms));
        }

        ModelState.AddModelError(string.Empty, "Failed to update trade program.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTradeProgram(int id)
    {
        var success = await adminService.DeleteTradeProgramAsync(id);
        TempData[success ? "SuccessMessage" : "ErrorMessage"] = success
            ? "Trade Program deleted successfully."
            : "Failed to delete trade program.";

        return RedirectToAction(nameof(TradePrograms));
    }

    #endregion

    #region Compliance Records

    public async Task<IActionResult> ComplianceRecords() =>
        View(await adminService.GetAllComplianceRecordsAsync());

    public async Task<IActionResult> CreateComplianceRecord()
    {
        await LoadComplianceDropdowns();
        return View(new ComplianceRecordEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateComplianceRecord(ComplianceRecordEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadComplianceDropdowns();
            return View(model);
        }

        var (success, errorMessage, _) = await adminService.CreateComplianceRecordAsync(model);

        if (success)
        {
            TempData["SuccessMessage"] = "Compliance Record created successfully.";
            return RedirectToAction(nameof(ComplianceRecords));
        }

        await LoadComplianceDropdowns();
        ModelState.AddModelError(string.Empty, errorMessage ?? "Failed to create compliance record.");
        return View(model);
    }

    public async Task<IActionResult> EditComplianceRecord(int id)
    {
        var record = await adminService.GetComplianceRecordByIdAsync(id);

        if (record == null)
            return NotFound();

        await LoadComplianceDropdowns();
        return View(record);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditComplianceRecord(ComplianceRecordEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadComplianceDropdowns();
            return View(model);
        }

        var success = await adminService.UpdateComplianceRecordAsync(model);

        if (success)
        {
            TempData["SuccessMessage"] = "Compliance Record updated successfully.";
            return RedirectToAction(nameof(ComplianceRecords));
        }

        await LoadComplianceDropdowns();
        ModelState.AddModelError(string.Empty, "Failed to update compliance record.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteComplianceRecord(int id)
    {
        var success = await adminService.DeleteComplianceRecordAsync(id);
        TempData[success ? "SuccessMessage" : "ErrorMessage"] = success
            ? "Compliance Record deleted successfully."
            : "Failed to delete compliance record.";

        return RedirectToAction(nameof(ComplianceRecords));
    }

    public async Task<IActionResult> Compliance() =>
        View(await adminService.GetComplianceTrackingAsync());

    #endregion

    #region Audit Logs

    public async Task<IActionResult> AuditLogs(int page = 1)
    {
        var logs = await adminService.GetAuditLogsAsync(page, 50);
        ViewBag.CurrentPage = page;
        return View(logs);
    }

    #endregion

    #region Private Methods

    private async Task LoadComplianceDropdowns()
    {
        ViewBag.Programs = await adminService.GetAllTradeProgramsAsync();
        ViewBag.Users = await adminService.GetAllUsersAsync();
    }

    #endregion
}
