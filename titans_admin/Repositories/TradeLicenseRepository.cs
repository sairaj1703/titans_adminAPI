using Microsoft.EntityFrameworkCore;
using titans_admin.Data;
using titans_admin.Models.Entities;
using titans_admin.Repositories.Interfaces;

namespace titans_admin.Repositories
{
    public class TradeLicenseRepository : ITradeLicenseRepository
    {
        private readonly TradeNetDbContext _context;

        public TradeLicenseRepository(TradeNetDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TradeLicense>> GetAllAsync()
        {
            return await _context.TradeLicenses
                .Include(t => t.User)
                .AsNoTracking()
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<TradeLicense?> GetByIdAsync(int id)
        {
            return await _context.TradeLicenses
                .Include(t => t.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TradeLicenseId == id);
        }

        public async Task<TradeLicense> AddAsync(TradeLicense entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            _context.TradeLicenses.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<TradeLicense> UpdateAsync(TradeLicense entity)
        {
            var existingEntity = await _context.TradeLicenses.FindAsync(entity.TradeLicenseId);
            if (existingEntity == null)
            {
                throw new InvalidOperationException($"TradeLicense with ID {entity.TradeLicenseId} not found");
            }

            // Update properties manually
            existingEntity.UserId = entity.UserId;
            existingEntity.LicenseNumber = entity.LicenseNumber;
            existingEntity.BusinessName = entity.BusinessName;
            existingEntity.BusinessType = entity.BusinessType;
            existingEntity.IssueDate = entity.IssueDate;
            existingEntity.ExpiryDate = entity.ExpiryDate;
            existingEntity.Status = entity.Status;
            existingEntity.Notes = entity.Notes;
            existingEntity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingEntity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.TradeLicenses.FindAsync(id);
            if (entity == null) return false;

            _context.TradeLicenses.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.TradeLicenses.AnyAsync(t => t.TradeLicenseId == id);
        }

        public async Task<IEnumerable<TradeLicense>> GetByUserIdAsync(int userId)
        {
            return await _context.TradeLicenses
                .Include(t => t.User)
                .AsNoTracking()
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TradeLicense>> GetByStatusAsync(string status)
        {
            return await _context.TradeLicenses
                .Include(t => t.User)
                .AsNoTracking()
                .Where(t => t.Status == status)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<TradeLicense?> GetByLicenseNumberAsync(string licenseNumber)
        {
            return await _context.TradeLicenses
                .Include(t => t.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.LicenseNumber == licenseNumber);
        }

        public async Task<bool> LicenseNumberExistsAsync(string licenseNumber)
        {
            return await _context.TradeLicenses.AnyAsync(t => t.LicenseNumber == licenseNumber);
        }

        public async Task<IEnumerable<TradeLicense>> GetExpiringSoonAsync(int daysThreshold)
        {
            var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);
            return await _context.TradeLicenses
                .Include(t => t.User)
                .Where(t => t.ExpiryDate <= thresholdDate && t.ExpiryDate >= DateTime.UtcNow)
                .OrderBy(t => t.ExpiryDate)
                .ToListAsync();
        }
    }
}
