using titans_admin.Models.ViewModels;
using titans_admin.Models.Enums;

namespace titans_admin.Services.Interfaces
{
    public interface IAdminService
    {
        // Dashboard
        Task<AdminDashboardViewModel> GetDashboardStatsAsync();
        Task<int> GetViolationCounterAsync();

        // User CRUD
        Task<List<UserListItemViewModel>> GetAllUsersAsync();
        Task<List<UserListItemViewModel>> GetUsersByRoleAsync(UserRole role);
        Task<List<UserListItemViewModel>> GetUsersByStatusAsync(UserStatus status);
        Task<UserEditViewModel?> GetUserByIdAsync(int userId);
        Task<(bool Success, string? ErrorMessage, int? UserId)> CreateUserAsync(CreateUserViewModel model);
        Task<bool> UpdateUserAsync(UserEditViewModel model);
        Task<bool> UpdateUserStatusAsync(int userId, UserStatus newStatus, int modifiedByUserId);
        Task<bool> DeleteUserAsync(int userId);

        // Trade License CRUD
        Task<List<TradeLicenseListViewModel>> GetAllTradeLicensesAsync();
        Task<TradeLicenseEditViewModel?> GetTradeLicenseByIdAsync(int licenseId);
        Task<(bool Success, string? ErrorMessage, int? LicenseId)> CreateTradeLicenseAsync(TradeLicenseEditViewModel model);
        Task<bool> UpdateTradeLicenseAsync(TradeLicenseEditViewModel model);
        Task<bool> DeleteTradeLicenseAsync(int licenseId);
        Task<List<TradeLicenseListViewModel>> GetTradeLicensesByStatusAsync(string status);

        // Trade Program CRUD
        Task<List<TradeProgramListViewModel>> GetAllTradeProgramsAsync();
        Task<TradeProgramEditViewModel?> GetTradeProgramByIdAsync(int programId);
        Task<(bool Success, string? ErrorMessage, int? ProgramId)> CreateTradeProgramAsync(TradeProgramEditViewModel model);
        Task<bool> UpdateTradeProgramAsync(TradeProgramEditViewModel model);
        Task<bool> DeleteTradeProgramAsync(int programId);
        Task<List<TradeProgramListViewModel>> GetActiveTradeProgramsAsync();

        // Compliance Record CRUD
        Task<List<ComplianceRecordItemViewModel>> GetAllComplianceRecordsAsync();
        Task<ComplianceRecordEditViewModel?> GetComplianceRecordByIdAsync(int recordId);
        Task<(bool Success, string? ErrorMessage, int? RecordId)> CreateComplianceRecordAsync(ComplianceRecordEditViewModel model);
        Task<bool> UpdateComplianceRecordAsync(ComplianceRecordEditViewModel model);
        Task<bool> DeleteComplianceRecordAsync(int recordId);

        // Audit Logs
        Task<List<AuditLogViewModel>> GetAuditLogsAsync(int pageNumber = 1, int pageSize = 50);
        Task<List<AuditLogViewModel>> GetAuditLogsByUserAsync(int userId, int pageNumber = 1, int pageSize = 50);

        // Compliance Tracking
        Task<ComplianceTrackingViewModel> GetComplianceTrackingAsync();
    }
}
