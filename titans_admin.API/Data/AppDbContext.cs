using Microsoft.EntityFrameworkCore;
using titans_admin.Models.Entities;

namespace titans_admin.API.Data;

/// <summary>
/// Entity Framework Core DbContext for the Titans Admin API
/// Inherits from TradeNetDbContext to ensure consistency with the main application
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    #region DbSets

    public DbSet<User> Users { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<TradeLicense> TradeLicenses { get; set; }
    public DbSet<TradeProgram> TradePrograms { get; set; }
    public DbSet<ComplianceRecord> ComplianceRecords { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        #region User Configuration

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.UserId);

            entity.HasIndex(u => u.Username)
                .IsUnique()
                .HasDatabaseName("IX_User_Username");

            entity.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_User_Email");

            entity.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(512);

            entity.Property(u => u.PhoneNumber)
                .HasMaxLength(20);

            entity.Property(u => u.Address)
                .HasMaxLength(500);

            entity.Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        });

        #endregion

        #region AuditLog Configuration

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(a => a.AuditLogId);

            entity.Property(a => a.Action)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.Resource)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.Details)
                .HasMaxLength(1000);

            entity.Property(a => a.IpAddress)
                .HasMaxLength(50);

            entity.Property(a => a.Timestamp)
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();

            entity.HasIndex(a => a.Timestamp)
                .HasDatabaseName("IX_AuditLog_Timestamp");

            entity.HasIndex(a => a.UserId)
                .HasDatabaseName("IX_AuditLog_UserId");

            // Foreign key relationship
            entity.HasOne(a => a.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        #endregion

        #region TradeLicense Configuration

        modelBuilder.Entity<TradeLicense>(entity =>
        {
            entity.HasKey(t => t.TradeLicenseId);

            entity.HasIndex(t => t.LicenseNumber)
                .IsUnique()
                .HasDatabaseName("IX_TradeLicense_LicenseNumber");

            entity.Property(t => t.LicenseNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(t => t.BusinessName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(t => t.BusinessType)
                .HasMaxLength(100);

            entity.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.Property(t => t.Notes)
                .HasMaxLength(500);

            entity.HasIndex(t => t.Status)
                .HasDatabaseName("IX_TradeLicense_Status");

            entity.HasIndex(t => t.UserId)
                .HasDatabaseName("IX_TradeLicense_UserId");

            // Foreign key relationship
            entity.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        #endregion

        #region TradeProgram Configuration

        modelBuilder.Entity<TradeProgram>(entity =>
        {
            entity.HasKey(p => p.TradeProgramId);

            entity.Property(p => p.ProgramName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(p => p.Description)
                .HasMaxLength(1000);

            entity.Property(p => p.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Active");

            entity.Property(p => p.ProgramType)
                .HasMaxLength(100);

            entity.Property(p => p.Budget)
                .HasPrecision(18, 2);

            entity.HasIndex(p => p.Status)
                .HasDatabaseName("IX_TradeProgram_Status");

            entity.HasIndex(p => p.ProgramName)
                .HasDatabaseName("IX_TradeProgram_ProgramName");
        });

        #endregion

        #region ComplianceRecord Configuration

        modelBuilder.Entity<ComplianceRecord>(entity =>
        {
            entity.HasKey(c => c.ComplianceRecordId);

            entity.Property(c => c.Findings)
                .HasMaxLength(1000);

            entity.Property(c => c.Recommendations)
                .HasMaxLength(1000);

            entity.HasIndex(c => c.TradeProgramId)
                .HasDatabaseName("IX_ComplianceRecord_TradeProgramId");

            entity.HasIndex(c => c.ReviewedByUserId)
                .HasDatabaseName("IX_ComplianceRecord_ReviewedByUserId");

            entity.HasIndex(c => c.ReviewDate)
                .HasDatabaseName("IX_ComplianceRecord_ReviewDate");

            // Foreign key relationships
            entity.HasOne(c => c.TradeProgram)
                .WithMany(t => t.ComplianceRecords)
                .HasForeignKey(c => c.TradeProgramId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.ReviewedBy)
                .WithMany()
                .HasForeignKey(c => c.ReviewedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        #endregion
    }
}
