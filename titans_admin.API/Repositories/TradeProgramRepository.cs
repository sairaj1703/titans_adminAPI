using Microsoft.EntityFrameworkCore;
using titans_admin.Data;
using titans_admin.Models.Entities;
using titans_admin.Repositories.Interfaces;

namespace titans_admin.Repositories
{
    public class TradeProgramRepository : ITradeProgramRepository
    {
        private readonly TradeNetDbContext _context;

        public TradeProgramRepository(TradeNetDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TradeProgram>> GetAllAsync()
        {
            return await _context.TradePrograms
                .Include(p => p.ComplianceRecords)
                .AsNoTracking()
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();
        }

        public async Task<TradeProgram?> GetByIdAsync(int id)
        {
            return await _context.TradePrograms
                .Include(p => p.ComplianceRecords)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.TradeProgramId == id);
        }

        public async Task<TradeProgram> AddAsync(TradeProgram entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            _context.TradePrograms.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<TradeProgram> UpdateAsync(TradeProgram entity)
        {
            var existingEntity = await _context.TradePrograms.FindAsync(entity.TradeProgramId);
            if (existingEntity == null)
            {
                throw new InvalidOperationException($"TradeProgram with ID {entity.TradeProgramId} not found");
            }

            // Update properties manually
            existingEntity.ProgramName = entity.ProgramName;
            existingEntity.Description = entity.Description;
            existingEntity.StartDate = entity.StartDate;
            existingEntity.EndDate = entity.EndDate;
            existingEntity.Status = entity.Status;
            existingEntity.ProgramType = entity.ProgramType;
            existingEntity.Budget = entity.Budget;
            existingEntity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingEntity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.TradePrograms
                .Include(p => p.ComplianceRecords)
                .FirstOrDefaultAsync(p => p.TradeProgramId == id);

            if (entity == null) return false;

            // Delete related compliance records (cascade)
            _context.ComplianceRecords.RemoveRange(entity.ComplianceRecords);
            _context.TradePrograms.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.TradePrograms.AnyAsync(p => p.TradeProgramId == id);
        }

        public async Task<IEnumerable<TradeProgram>> GetActiveAsync()
        {
            return await _context.TradePrograms
                .Include(p => p.ComplianceRecords)
                .AsNoTracking()
                .Where(p => p.Status == "Active")
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TradeProgram>> GetByStatusAsync(string status)
        {
            return await _context.TradePrograms
                .Include(p => p.ComplianceRecords)
                .AsNoTracking()
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TradeProgram>> GetByTypeAsync(string programType)
        {
            return await _context.TradePrograms
                .Include(p => p.ComplianceRecords)
                .AsNoTracking()
                .Where(p => p.ProgramType == programType)
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();
        }
    }
}
