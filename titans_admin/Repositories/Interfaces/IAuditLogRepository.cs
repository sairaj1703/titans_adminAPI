using titans_admin.Models.Entities;

namespace titans_admin.Repositories.Interfaces;

public interface IAuditLogRepository : IRepository<AuditLog>
{
    Task<IEnumerable<AuditLog>> GetByUserIdAsync(int userId);
    Task<IEnumerable<AuditLog>> GetPagedAsync(int page, int pageSize);
    Task<int> GetTotalCountAsync();
}
