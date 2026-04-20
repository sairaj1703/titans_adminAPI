using Microsoft.EntityFrameworkCore;
using titans_admin.Data;
using titans_admin.Models.Entities;
using titans_admin.Repositories.Interfaces;

namespace titans_admin.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly TradeNetDbContext _context;

        public AuditLogRepository(TradeNetDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AuditLog>> GetAllAsync()
        {
            return await _context.AuditLogs
                .Include(a => a.User)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<AuditLog?> GetByIdAsync(int id)
        {
            return await _context.AuditLogs
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AuditLogId == id);
        }

        public async Task<AuditLog> AddAsync(AuditLog entity)
        {
            entity.Timestamp = DateTime.UtcNow;
            _context.AuditLogs.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<AuditLog> UpdateAsync(AuditLog entity)
        {
            _context.AuditLogs.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Audit logs should not be deleted (immutable)
            return false;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.AuditLogs.AnyAsync(a => a.AuditLogId == id);
        }

        public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(int userId)
        {
            return await _context.AuditLogs
                .Include(a => a.User)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetPagedAsync(int page, int pageSize)
        {
            return await _context.AuditLogs
                .Include(a => a.User)
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.AuditLogs.CountAsync();
        }
    }
}
