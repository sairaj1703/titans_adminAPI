using titans_admin.Models.Entities;
using titans_admin.Models.Enums;
using titans_admin.Models.ViewModels;
using titans_admin.Repositories.Interfaces;
using titans_admin.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace titans_admin.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITradeLicenseRepository _tradeLicenseRepository;
        private readonly ITradeProgramRepository _tradeProgramRepository;
        private readonly IComplianceRecordRepository _complianceRecordRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ILogger<AdminService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminService(
            IUserRepository userRepository,
            ITradeLicenseRepository tradeLicenseRepository,
            ITradeProgramRepository tradeProgramRepository,
            IComplianceRecordRepository complianceRecordRepository,
            IAuditLogRepository auditLogRepository,
            ILogger<AdminService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _tradeLicenseRepository = tradeLicenseRepository;
            _tradeProgramRepository = tradeProgramRepository;
            _complianceRecordRepository = complianceRecordRepository;
            _auditLogRepository = auditLogRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Audit Logging

        private async Task LogAuditAsync(string action, string resource, string details, int? userId = null)
        {
            try
            {
                var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";

                var auditLog = new AuditLog
                {
                    UserId = userId,
                    Action = action,
                    Resource = resource,
                    Details = details,
                    IpAddress = ipAddress,
                    Timestamp = DateTime.UtcNow
                };

                await _auditLogRepository.AddAsync(auditLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create audit log for action {Action}", action);
            }
        }

        #endregion

        #region Dashboard

        public async Task<AdminDashboardViewModel> GetDashboardStatsAsync()
        {
            try
            {
                var licenses = await _tradeLicenseRepository.GetAllAsync();
                var programs = await _tradeProgramRepository.GetActiveAsync();
                var auditLogs = await _auditLogRepository.GetTotalCountAsync();
                var violations = await _complianceRecordRepository.GetViolationCountAsync();

                var users = await _userRepository.GetAllAsync();
                var recentUsers = users.Take(5).Select(u => new UserListItemViewModel
                {
                    UserId = u.UserId,
                    Username = u.Email, // Use email as username
                    Email = u.Email,
                    FullName = $"{u.FirstName} {u.LastName}".Trim(),
                    Role = u.Role,
                    Status = u.Status,
                    CreatedAt = u.CreatedAt,
                    LastLoginAt = u.LastLoginAt
                }).ToList();

                return new AdminDashboardViewModel
                {
                    TotalTradeLicenses = licenses.Count(),
                    ActivePrograms = programs.Count(),
                    TotalTransactions = auditLogs,
                    ViolationCounter = violations,
                    RecentUsers = recentUsers
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dashboard stats");
                return new AdminDashboardViewModel();
            }
        }

        #endregion

        #region User Management

        public async Task<List<UserListItemViewModel>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UserListItemViewModel
            {
                UserId = u.UserId,
                Username = u.Email, // Use email as username
                Email = u.Email,
                FullName = $"{u.FirstName} {u.LastName}".Trim(),
                Role = u.Role,
                Status = u.Status,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt
            }).ToList();
        }

        public async Task<UserEditViewModel?> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            return new UserEditViewModel
            {
                UserId = user.UserId,
                Username = user.Email, // Use email as username
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                Status = user.Status,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            };
        }

        public async Task<(bool Success, string? ErrorMessage, int? UserId)> CreateUserAsync(CreateUserViewModel model)
        {
            try
            {
                if (await _userRepository.EmailExistsAsync(model.Email))
                {
                    return (false, "Email already registered", null);
                }

                // Generate username from email
                var username = GenerateUsernameFromEmail(model.Email);
                var baseUsername = username;
                var counter = 1;
                while (await _userRepository.UsernameExistsAsync(username))
                {
                    username = $"{baseUsername}{counter}";
                    counter++;
                }

                var user = new User
                {
                    Username = username,
                    Email = model.Email.ToLower().Trim(),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PasswordHash = HashPassword(model.Password),
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    Role = model.Role,
                    Status = model.Status
                };

                var createdUser = await _userRepository.AddAsync(user);

                await LogAuditAsync("Create", "User", $"Created user: {user.Email}");

                _logger.LogInformation("User {Email} created successfully", model.Email);
                return (true, null, createdUser.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user {Email}", model.Email);
                return (false, "An error occurred while creating the user", null);
            }
        }

        public async Task<bool> UpdateUserAsync(UserEditViewModel model)
        {
            try
            {
                var user = new User
                {
                    UserId = model.UserId,
                    Username = GenerateUsernameFromEmail(model.Email),
                    Email = model.Email.ToLower().Trim(),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Role = model.Role,
                    Status = model.Status,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address
                };

                await _userRepository.UpdateAsync(user);
                await LogAuditAsync("Update", "User", $"Updated user: {model.Email}", model.UserId);

                _logger.LogInformation("User {UserId} updated successfully", model.UserId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", model.UserId);
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                var result = await _userRepository.DeleteAsync(userId);
                
                if (result)
                {
                    await LogAuditAsync("Delete", "User", $"Deleted user: {user?.Email ?? userId.ToString()}");
                    _logger.LogInformation("User {UserId} deleted successfully", userId);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", userId);
                return false;
            }
        }

        #endregion

        #region Trade Licenses

        public async Task<List<TradeLicenseListViewModel>> GetAllTradeLicensesAsync()
        {
            var licenses = await _tradeLicenseRepository.GetAllAsync();
            return licenses.Select(t => new TradeLicenseListViewModel
            {
                TradeLicenseId = t.TradeLicenseId,
                LicenseNumber = t.LicenseNumber,
                BusinessName = t.BusinessName,
                BusinessType = t.BusinessType,
                OwnerName = t.User != null ? $"{t.User.FirstName} {t.User.LastName}".Trim() : "Unknown",
                IssueDate = t.IssueDate,
                ExpiryDate = t.ExpiryDate,
                Status = t.Status
            }).ToList();
        }

        public async Task<TradeLicenseEditViewModel?> GetTradeLicenseByIdAsync(int licenseId)
        {
            var license = await _tradeLicenseRepository.GetByIdAsync(licenseId);
            if (license == null) return null;

            return new TradeLicenseEditViewModel
            {
                TradeLicenseId = license.TradeLicenseId,
                UserId = license.UserId,
                LicenseNumber = license.LicenseNumber,
                BusinessName = license.BusinessName,
                BusinessType = license.BusinessType,
                IssueDate = license.IssueDate,
                ExpiryDate = license.ExpiryDate,
                Status = license.Status,
                Notes = license.Notes
            };
        }

        public async Task<(bool Success, string? ErrorMessage, int? LicenseId)> CreateTradeLicenseAsync(TradeLicenseEditViewModel model)
        {
            try
            {
                // Auto-generate license number
                var licenseNumber = await GenerateLicenseNumberAsync();

                var license = new TradeLicense
                {
                    UserId = model.UserId,
                    LicenseNumber = licenseNumber,
                    BusinessName = model.BusinessName,
                    BusinessType = model.BusinessType,
                    IssueDate = model.IssueDate,
                    ExpiryDate = model.ExpiryDate,
                    Status = model.Status,
                    Notes = model.Notes
                };

                var createdLicense = await _tradeLicenseRepository.AddAsync(license);

                await LogAuditAsync("Create", "TradeLicense", $"Created license: {licenseNumber} for {model.BusinessName}");

                _logger.LogInformation("Trade License {LicenseNumber} created successfully", licenseNumber);
                return (true, null, createdLicense.TradeLicenseId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating trade license");
                return (false, "An error occurred while creating the license", null);
            }
        }

        public async Task<bool> UpdateTradeLicenseAsync(TradeLicenseEditViewModel model)
        {
            try
            {
                var existingLicense = await _tradeLicenseRepository.GetByIdAsync(model.TradeLicenseId);
                
                var license = new TradeLicense
                {
                    TradeLicenseId = model.TradeLicenseId,
                    UserId = model.UserId,
                    LicenseNumber = existingLicense?.LicenseNumber ?? model.LicenseNumber, // Keep existing license number
                    BusinessName = model.BusinessName,
                    BusinessType = model.BusinessType,
                    IssueDate = model.IssueDate,
                    ExpiryDate = model.ExpiryDate,
                    Status = model.Status,
                    Notes = model.Notes
                };

                await _tradeLicenseRepository.UpdateAsync(license);
                await LogAuditAsync("Update", "TradeLicense", $"Updated license: {license.LicenseNumber}");

                _logger.LogInformation("Trade License {LicenseId} updated successfully", model.TradeLicenseId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating trade license {LicenseId}", model.TradeLicenseId);
                return false;
            }
        }

        public async Task<bool> DeleteTradeLicenseAsync(int licenseId)
        {
            try
            {
                var license = await _tradeLicenseRepository.GetByIdAsync(licenseId);
                var result = await _tradeLicenseRepository.DeleteAsync(licenseId);
                
                if (result)
                {
                    await LogAuditAsync("Delete", "TradeLicense", $"Deleted license: {license?.LicenseNumber ?? licenseId.ToString()}");
                    _logger.LogInformation("Trade License {LicenseId} deleted successfully", licenseId);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting trade license {LicenseId}", licenseId);
                return false;
            }
        }

        public async Task<List<TradeLicenseListViewModel>> GetTradeLicensesByStatusAsync(string status)
        {
            var licenses = await _tradeLicenseRepository.GetByStatusAsync(status);
            return licenses.Select(t => new TradeLicenseListViewModel
            {
                TradeLicenseId = t.TradeLicenseId,
                LicenseNumber = t.LicenseNumber,
                BusinessName = t.BusinessName,
                BusinessType = t.BusinessType,
                OwnerName = t.User != null ? $"{t.User.FirstName} {t.User.LastName}".Trim() : "Unknown",
                IssueDate = t.IssueDate,
                ExpiryDate = t.ExpiryDate,
                Status = t.Status
            }).ToList();
        }

        #endregion

        #region Trade Programs

        public async Task<List<TradeProgramListViewModel>> GetAllTradeProgramsAsync()
        {
            var programs = await _tradeProgramRepository.GetAllAsync();
            return programs.Select(p => new TradeProgramListViewModel
            {
                TradeProgramId = p.TradeProgramId,
                ProgramName = p.ProgramName,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status,
                ProgramType = p.ProgramType,
                Budget = p.Budget,
                ComplianceRecordCount = p.ComplianceRecords?.Count ?? 0
            }).ToList();
        }

        public async Task<TradeProgramEditViewModel?> GetTradeProgramByIdAsync(int programId)
        {
            var program = await _tradeProgramRepository.GetByIdAsync(programId);
            if (program == null) return null;

            return new TradeProgramEditViewModel
            {
                TradeProgramId = program.TradeProgramId,
                ProgramName = program.ProgramName,
                Description = program.Description,
                StartDate = program.StartDate,
                EndDate = program.EndDate,
                Status = program.Status,
                ProgramType = program.ProgramType,
                Budget = program.Budget
            };
        }

        public async Task<(bool Success, string? ErrorMessage, int? ProgramId)> CreateTradeProgramAsync(TradeProgramEditViewModel model)
        {
            try
            {
                var program = new TradeProgram
                {
                    ProgramName = model.ProgramName,
                    Description = model.Description,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Status = model.Status,
                    ProgramType = model.ProgramType,
                    Budget = model.Budget
                };

                var createdProgram = await _tradeProgramRepository.AddAsync(program);

                await LogAuditAsync("Create", "TradeProgram", $"Created program: {model.ProgramName}");

                _logger.LogInformation("Trade Program {ProgramName} created successfully", model.ProgramName);
                return (true, null, createdProgram.TradeProgramId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating trade program");
                return (false, "An error occurred while creating the program", null);
            }
        }

        public async Task<bool> UpdateTradeProgramAsync(TradeProgramEditViewModel model)
        {
            try
            {
                var program = new TradeProgram
                {
                    TradeProgramId = model.TradeProgramId,
                    ProgramName = model.ProgramName,
                    Description = model.Description,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Status = model.Status,
                    ProgramType = model.ProgramType,
                    Budget = model.Budget
                };

                await _tradeProgramRepository.UpdateAsync(program);
                await LogAuditAsync("Update", "TradeProgram", $"Updated program: {model.ProgramName}");

                _logger.LogInformation("Trade Program {ProgramId} updated successfully", model.TradeProgramId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating trade program {ProgramId}", model.TradeProgramId);
                return false;
            }
        }

        public async Task<bool> DeleteTradeProgramAsync(int programId)
        {
            try
            {
                var program = await _tradeProgramRepository.GetByIdAsync(programId);
                var result = await _tradeProgramRepository.DeleteAsync(programId);
                
                if (result)
                {
                    await LogAuditAsync("Delete", "TradeProgram", $"Deleted program: {program?.ProgramName ?? programId.ToString()}");
                    _logger.LogInformation("Trade Program {ProgramId} deleted successfully", programId);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting trade program {ProgramId}", programId);
                return false;
            }
        }

        public async Task<List<TradeProgramListViewModel>> GetActiveTradeProgramsAsync()
        {
            var programs = await _tradeProgramRepository.GetActiveAsync();
            return programs.Select(p => new TradeProgramListViewModel
            {
                TradeProgramId = p.TradeProgramId,
                ProgramName = p.ProgramName,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status,
                ProgramType = p.ProgramType,
                Budget = p.Budget,
                ComplianceRecordCount = p.ComplianceRecords?.Count ?? 0
            }).ToList();
        }

        #endregion

        #region Compliance Records

        public async Task<List<ComplianceRecordItemViewModel>> GetAllComplianceRecordsAsync()
        {
            var records = await _complianceRecordRepository.GetAllAsync();
            return records.Select(c => new ComplianceRecordItemViewModel
            {
                ComplianceRecordId = c.ComplianceRecordId,
                ProgramName = c.TradeProgram?.ProgramName ?? "Unknown",
                Result = c.Result,
                ReviewDate = c.ReviewDate,
                ReviewedBy = c.ReviewedBy != null ? $"{c.ReviewedBy.FirstName} {c.ReviewedBy.LastName}".Trim() : null,
                Findings = c.Findings
            }).ToList();
        }

        public async Task<ComplianceRecordEditViewModel?> GetComplianceRecordByIdAsync(int recordId)
        {
            var record = await _complianceRecordRepository.GetByIdAsync(recordId);
            if (record == null) return null;

            return new ComplianceRecordEditViewModel
            {
                ComplianceRecordId = record.ComplianceRecordId,
                TradeProgramId = record.TradeProgramId,
                ReviewedByUserId = record.ReviewedByUserId,
                Result = record.Result,
                ReviewDate = record.ReviewDate,
                Findings = record.Findings,
                Recommendations = record.Recommendations,
                FollowUpDate = record.FollowUpDate
            };
        }

        public async Task<(bool Success, string? ErrorMessage, int? RecordId)> CreateComplianceRecordAsync(ComplianceRecordEditViewModel model)
        {
            try
            {
                var record = new ComplianceRecord
                {
                    TradeProgramId = model.TradeProgramId,
                    ReviewedByUserId = model.ReviewedByUserId,
                    Result = model.Result,
                    ReviewDate = model.ReviewDate,
                    Findings = model.Findings,
                    Recommendations = model.Recommendations,
                    FollowUpDate = model.FollowUpDate
                };

                var createdRecord = await _complianceRecordRepository.AddAsync(record);

                await LogAuditAsync("Create", "ComplianceRecord", $"Created compliance record for program ID: {model.TradeProgramId}");

                _logger.LogInformation("Compliance Record created successfully for program {ProgramId}", model.TradeProgramId);
                return (true, null, createdRecord.ComplianceRecordId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating compliance record");
                return (false, "An error occurred while creating the record", null);
            }
        }

        public async Task<bool> UpdateComplianceRecordAsync(ComplianceRecordEditViewModel model)
        {
            try
            {
                var record = new ComplianceRecord
                {
                    ComplianceRecordId = model.ComplianceRecordId,
                    TradeProgramId = model.TradeProgramId,
                    ReviewedByUserId = model.ReviewedByUserId,
                    Result = model.Result,
                    ReviewDate = model.ReviewDate,
                    Findings = model.Findings,
                    Recommendations = model.Recommendations,
                    FollowUpDate = model.FollowUpDate
                };

                await _complianceRecordRepository.UpdateAsync(record);
                await LogAuditAsync("Update", "ComplianceRecord", $"Updated compliance record ID: {model.ComplianceRecordId}");

                _logger.LogInformation("Compliance Record {RecordId} updated successfully", model.ComplianceRecordId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating compliance record {RecordId}", model.ComplianceRecordId);
                return false;
            }
        }

        public async Task<bool> DeleteComplianceRecordAsync(int recordId)
        {
            try
            {
                var result = await _complianceRecordRepository.DeleteAsync(recordId);
                
                if (result)
                {
                    await LogAuditAsync("Delete", "ComplianceRecord", $"Deleted compliance record ID: {recordId}");
                    _logger.LogInformation("Compliance Record {RecordId} deleted successfully", recordId);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting compliance record {RecordId}", recordId);
                return false;
            }
        }

        public async Task<ComplianceTrackingViewModel> GetComplianceTrackingAsync()
        {
            var records = await _complianceRecordRepository.GetAllAsync();
            var recordsList = records.ToList();

            return new ComplianceTrackingViewModel
            {
                TotalRecords = recordsList.Count,
                CompliantRecords = recordsList.Count(r => r.Result == ComplianceResult.Compliant),
                NonCompliantRecords = recordsList.Count(r => r.Result == ComplianceResult.NonCompliant),
                UnderReviewRecords = recordsList.Count(r => r.Result == ComplianceResult.UnderReview)
            };
        }

        #endregion

        #region Audit Logs

        public async Task<List<AuditLogViewModel>> GetAuditLogsAsync(int page, int pageSize)
        {
            var logs = await _auditLogRepository.GetPagedAsync(page, pageSize);
            return logs.Select(a => new AuditLogViewModel
            {
                AuditLogId = a.AuditLogId,
                Username = a.User?.Email ?? "System",
                Action = a.Action,
                Resource = a.Resource,
                Details = a.Details,
                Timestamp = a.Timestamp,
                IpAddress = a.IpAddress
            }).ToList();
        }

        public async Task<List<AuditLogViewModel>> GetAuditLogsByUserAsync(int userId, int pageNumber = 1, int pageSize = 50)
        {
            var logs = await _auditLogRepository.GetPagedAsync(pageNumber, pageSize);
            var userLogs = logs.Where(a => a.UserId == userId);
            return userLogs.Select(a => new AuditLogViewModel
            {
                AuditLogId = a.AuditLogId,
                Username = a.User?.Email ?? "System",
                Action = a.Action,
                Resource = a.Resource,
                Details = a.Details,
                Timestamp = a.Timestamp,
                IpAddress = a.IpAddress
            }).ToList();
        }

        public async Task<int> GetViolationCounterAsync()
        {
            return await _complianceRecordRepository.GetViolationCountAsync();
        }

        public async Task<List<UserListItemViewModel>> GetUsersByRoleAsync(UserRole role)
        {
            var users = await _userRepository.GetByRoleAsync(role);
            return users.Select(u => new UserListItemViewModel
            {
                UserId = u.UserId,
                Username = u.Email,
                Email = u.Email,
                FullName = $"{u.FirstName} {u.LastName}".Trim(),
                Role = u.Role,
                Status = u.Status,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt
            }).ToList();
        }

        public async Task<List<UserListItemViewModel>> GetUsersByStatusAsync(UserStatus status)
        {
            var users = await _userRepository.GetByStatusAsync(status);
            return users.Select(u => new UserListItemViewModel
            {
                UserId = u.UserId,
                Username = u.Email,
                Email = u.Email,
                FullName = $"{u.FirstName} {u.LastName}".Trim(),
                Role = u.Role,
                Status = u.Status,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt
            }).ToList();
        }

        public async Task<bool> UpdateUserStatusAsync(int userId, UserStatus newStatus, int modifiedByUserId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null) return false;

                user.Status = newStatus;
                await _userRepository.UpdateAsync(user);
                await LogAuditAsync("Update", "User", $"Changed status to {newStatus} for user: {user.Email}", modifiedByUserId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user status {UserId}", userId);
                return false;
            }
        }

        #endregion

        #region Private Methods

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private static string GenerateUsernameFromEmail(string email)
        {
            var username = email.Split('@')[0].ToLower();
            username = System.Text.RegularExpressions.Regex.Replace(username, "[^a-z0-9_]", "");
            return string.IsNullOrEmpty(username) ? "user" : username;
        }

        private async Task<string> GenerateLicenseNumberAsync()
        {
            var year = DateTime.UtcNow.Year;
            var existingLicenses = await _tradeLicenseRepository.GetAllAsync();
            var count = existingLicenses.Count() + 1;
            return $"TL-{year}-{count:D6}";
        }

        #endregion
    }
}
