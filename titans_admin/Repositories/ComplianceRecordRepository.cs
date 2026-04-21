using Microsoft.EntityFrameworkCore;
using titans_admin.Data;
using titans_admin.Models.Entities;
using titans_admin.Models.Enums;
using titans_admin.Repositories.Interfaces;

namespace titans_admin.Repositories
{
    public class ComplianceRecordRepository : IComplianceRecordRepository
    {
        private readonly TradeNetDbContext _context;

        public ComplianceRecordRepository(TradeNetDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ComplianceRecord>> GetAllAsync()
        {
            return await _context.ComplianceRecords
                .Include(c => c.TradeProgram)
                .Include(c => c.ReviewedBy)
                .AsNoTracking()
                .OrderByDescending(c => c.ReviewDate)
                .ToListAsync();
        }

        public async Task<ComplianceRecord?> GetByIdAsync(int id)
        {
            return await _context.ComplianceRecords
                .Include(c => c.TradeProgram)
                .Include(c => c.ReviewedBy)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ComplianceRecordId == id);
        }

        public async Task<ComplianceRecord> AddAsync(ComplianceRecord entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            _context.ComplianceRecords.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ComplianceRecord> UpdateAsync(ComplianceRecord entity)
        {
            var existingEntity = await _context.ComplianceRecords.FindAsync(entity.ComplianceRecordId);
            if (existingEntity == null)
            {
                throw new InvalidOperationException($"ComplianceRecord with ID {entity.ComplianceRecordId} not found");
            }

            // Update properties manually
            existingEntity.TradeProgramId = entity.TradeProgramId;
            existingEntity.ReviewedByUserId = entity.ReviewedByUserId;
            existingEntity.Result = entity.Result;
            existingEntity.ReviewDate = entity.ReviewDate;
            existingEntity.Findings = entity.Findings;
            existingEntity.Recommendations = entity.Recommendations;
            existingEntity.FollowUpDate = entity.FollowUpDate;
            existingEntity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingEntity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.ComplianceRecords.FindAsync(id);
            if (entity == null) return false;

            _context.ComplianceRecords.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.ComplianceRecords.AnyAsync(c => c.ComplianceRecordId == id);
        }

        public async Task<IEnumerable<ComplianceRecord>> GetByProgramIdAsync(int programId)
        {
            return await _context.ComplianceRecords
                .Include(c => c.TradeProgram)
                .Include(c => c.ReviewedBy)
                .AsNoTracking()
                .Where(c => c.TradeProgramId == programId)
                .OrderByDescending(c => c.ReviewDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ComplianceRecord>> GetByResultAsync(ComplianceResult result)
        {
            return await _context.ComplianceRecords
                .Include(c => c.TradeProgram)
                .Include(c => c.ReviewedBy)
                .AsNoTracking()
                .Where(c => c.Result == result)
                .OrderByDescending(c => c.ReviewDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ComplianceRecord>> GetByReviewerIdAsync(int userId)
        {
            return await _context.ComplianceRecords
                .Include(c => c.TradeProgram)
                .Include(c => c.ReviewedBy)
                .AsNoTracking()
                .Where(c => c.ReviewedByUserId == userId)
                .OrderByDescending(c => c.ReviewDate)
                .ToListAsync();
        }

        public async Task<int> GetViolationCountAsync()
        {
            return await _context.ComplianceRecords
                .CountAsync(c => c.Result == ComplianceResult.NonCompliant);
        }
    }
}
