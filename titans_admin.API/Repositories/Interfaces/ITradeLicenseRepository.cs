using titans_admin.Models.Entities;

namespace titans_admin.Repositories.Interfaces;

public interface ITradeLicenseRepository : IRepository<TradeLicense>
{
    Task<IEnumerable<TradeLicense>> GetByUserIdAsync(int userId);
    Task<IEnumerable<TradeLicense>> GetByStatusAsync(string status);
    Task<TradeLicense?> GetByLicenseNumberAsync(string licenseNumber);
    Task<bool> LicenseNumberExistsAsync(string licenseNumber);
    Task<IEnumerable<TradeLicense>> GetExpiringSoonAsync(int daysThreshold);
}
