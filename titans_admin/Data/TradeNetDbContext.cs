using Microsoft.EntityFrameworkCore;
using titans_admin.Models.Entities;

namespace titans_admin.Data;

public class TradeNetDbContext(DbContextOptions<TradeNetDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<TradeLicense> TradeLicenses { get; set; }
    public DbSet<TradeProgram> TradePrograms { get; set; }
    public DbSet<ComplianceRecord> ComplianceRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Username).IsUnique();
            e.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<AuditLog>()
            .HasOne(a => a.User)
            .WithMany(u => u.AuditLogs)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TradeLicense>(e =>
        {
            e.HasIndex(t => t.LicenseNumber).IsUnique();
            e.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TradeProgram>()
            .Property(p => p.Budget)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ComplianceRecord>(e =>
        {
            e.HasOne(c => c.TradeProgram)
                .WithMany(t => t.ComplianceRecords)
                .HasForeignKey(c => c.TradeProgramId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(c => c.ReviewedBy)
                .WithMany()
                .HasForeignKey(c => c.ReviewedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
