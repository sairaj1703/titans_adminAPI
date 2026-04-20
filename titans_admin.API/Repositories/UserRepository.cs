using Microsoft.EntityFrameworkCore;
using titans_admin.Data;
using titans_admin.Models.Entities;
using titans_admin.Models.Enums;
using titans_admin.Repositories.Interfaces;

namespace titans_admin.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TradeNetDbContext _context;

        public UserRepository(TradeNetDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User> AddAsync(User entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            _context.Users.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<User> UpdateAsync(User entity)
        {
            var existingEntity = await _context.Users.FindAsync(entity.UserId);
            if (existingEntity == null)
            {
                throw new InvalidOperationException($"User with ID {entity.UserId} not found");
            }

            // Update properties manually
            existingEntity.Username = entity.Username;
            existingEntity.Email = entity.Email;
            existingEntity.FirstName = entity.FirstName;
            existingEntity.LastName = entity.LastName;
            existingEntity.Role = entity.Role;
            existingEntity.Status = entity.Status;
            existingEntity.PhoneNumber = entity.PhoneNumber;
            existingEntity.Address = entity.Address;
            existingEntity.LastLoginAt = entity.LastLoginAt;
            // Keep PasswordHash and CreatedAt unchanged

            await _context.SaveChangesAsync();
            return existingEntity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Users
                .Include(u => u.AuditLogs)
                .FirstOrDefaultAsync(u => u.UserId == id);
            
            if (entity == null) return false;

            // If user has audit logs, deactivate instead of delete
            if (entity.AuditLogs.Any())
            {
                entity.Status = UserStatus.Inactive;
                entity.Username = $"[DELETED]_{entity.Username}_{DateTime.UtcNow.Ticks}";
                entity.Email = $"deleted_{id}_{DateTime.UtcNow.Ticks}@deleted.com";
                await _context.SaveChangesAsync();
                return true;
            }

            // Delete related trade licenses
            var licenses = await _context.TradeLicenses
                .Where(t => t.UserId == id)
                .ToListAsync();
            _context.TradeLicenses.RemoveRange(licenses);

            // Update compliance records
            var complianceRecords = await _context.ComplianceRecords
                .Where(c => c.ReviewedByUserId == id)
                .ToListAsync();
            foreach (var record in complianceRecords)
            {
                record.ReviewedByUserId = null;
            }

            _context.Users.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Users.AnyAsync(u => u.UserId == id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role)
        {
            return await _context.Users
                .Where(u => u.Role == role)
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByStatusAsync(UserStatus status)
        {
            return await _context.Users
                .Where(u => u.Status == status)
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }
    }
}
