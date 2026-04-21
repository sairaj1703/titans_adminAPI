using titans_admin.Models.Entities;
using titans_admin.Models.Enums;

namespace titans_admin.Repositories.Interfaces;

public interface IComplianceRecordRepository : IRepository<ComplianceRecord>
{
    Task<IEnumerable<ComplianceRecord>> GetByProgramIdAsync(int programId);
    Task<IEnumerable<ComplianceRecord>> GetByResultAsync(ComplianceResult result);
    Task<IEnumerable<ComplianceRecord>> GetByReviewerIdAsync(int userId);
    Task<int> GetViolationCountAsync();
}
