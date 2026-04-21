using titans_admin.Models.Entities;
using titans_admin.Models.Enums;
using titans_admin.Utilities;

namespace titans_admin.Data;

public static class DatabaseSeeder
{
    public static void SeedTestData(TradeNetDbContext context)
    {
        if (context.Users.Any())
            return;

        var users = CreateUsers();
        context.Users.AddRange(users);
        context.SaveChanges();

        var programs = CreateTradePrograms();
        context.TradePrograms.AddRange(programs);
        context.SaveChanges();

        var trader = users.First(u => u.Role == UserRole.Business && u.Status == UserStatus.Active);
        var licenses = CreateTradeLicenses(trader.UserId);
        context.TradeLicenses.AddRange(licenses);
        context.SaveChanges();

        var complianceOfficer = users.First(u => u.Role == UserRole.Compliance);
        var records = CreateComplianceRecords(programs, complianceOfficer.UserId);
        context.ComplianceRecords.AddRange(records);
        context.SaveChanges();

        var auditLogs = CreateAuditLogs(users);
        context.AuditLogs.AddRange(auditLogs);
        context.SaveChanges();
    }

    private static List<User> CreateUsers() =>
    [
        new()
        {
            Username = "admin",
            Email = "admin@tradenet.gov",
            PasswordHash = PasswordHashGenerator.GenerateHash("Admin@123"),
            FirstName = "System",
            LastName = "Administrator",
            Role = UserRole.Admin,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow
        },
        new()
        {
            Username = "officer1",
            Email = "officer1@tradenet.gov",
            PasswordHash = PasswordHashGenerator.GenerateHash("Officer@123"),
            FirstName = "Trade",
            LastName = "Officer",
            Role = UserRole.Officer,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow
        },
        new()
        {
            Username = "compliance1",
            Email = "compliance1@tradenet.gov",
            PasswordHash = PasswordHashGenerator.GenerateHash("Compliance@123"),
            FirstName = "Compliance",
            LastName = "Officer",
            Role = UserRole.Compliance,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow
        },
        new()
        {
            Username = "trader1",
            Email = "trader@example.com",
            PasswordHash = PasswordHashGenerator.GenerateHash("Trader@123"),
            FirstName = "John",
            LastName = "Trader",
            Role = UserRole.Business,
            Status = UserStatus.Active,
            PhoneNumber = "+1-555-0101",
            Address = "123 Business St, Commerce City",
            CreatedAt = DateTime.UtcNow
        }
    ];

    private static List<TradeProgram> CreateTradePrograms() =>
    [
        new()
        {
            ProgramName = "Export Incentive Program 2024",
            Description = "Financial support for businesses engaging in export activities",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 12, 31),
            Status = "Active",
            ProgramType = "Export",
            Budget = 5000000m,
            CreatedAt = DateTime.UtcNow
        },
        new()
        {
            ProgramName = "Import Trade Compliance Initiative",
            Description = "Ensuring compliance with import regulations and standards",
            StartDate = new DateTime(2024, 1, 1),
            Status = "Active",
            ProgramType = "Import",
            Budget = 2500000m,
            CreatedAt = DateTime.UtcNow
        },
        new()
        {
            ProgramName = "SME Trade Development",
            Description = "Supporting small and medium enterprises in international trade",
            StartDate = new DateTime(2024, 6, 1),
            EndDate = new DateTime(2025, 5, 31),
            Status = "Active",
            ProgramType = "Development",
            Budget = 3000000m,
            CreatedAt = DateTime.UtcNow
        } 
    ];

    private static List<TradeLicense> CreateTradeLicenses(int traderId) =>
    [
        new()
        {
            UserId = traderId,
            LicenseNumber = "TL-2024-00001",
            BusinessName = "ABC Trading Co.",
            BusinessType = "Import/Export",
            IssueDate = DateTime.UtcNow.AddMonths(-6),
            ExpiryDate = DateTime.UtcNow.AddMonths(18),
            Status = "Active",
            Notes = "General trade license",
            CreatedAt = DateTime.UtcNow
        },
        new()
        {
            UserId = traderId,
            LicenseNumber = "TL-2024-00002",
            BusinessName = "XYZ Importers Ltd.",
            BusinessType = "Import",
            IssueDate = DateTime.UtcNow.AddMonths(-3),
            ExpiryDate = DateTime.UtcNow.AddMonths(21),
            Status = "Active",
            Notes = "Specialized import license",
            CreatedAt = DateTime.UtcNow
        },
        new()
        {
            UserId = traderId,
            LicenseNumber = "TL-2024-00003",
            BusinessName = "Pending Business Inc.",
            BusinessType = "Export",
            IssueDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddMonths(24),
            Status = "Pending",
            Notes = "Application under review",
            CreatedAt = DateTime.UtcNow
        }
    ];

    private static List<ComplianceRecord> CreateComplianceRecords(List<TradeProgram> programs, int reviewerId)
    {
        var records = new List<ComplianceRecord>();

        foreach (var program in programs)
        {
            records.Add(new ComplianceRecord
            {
                TradeProgramId = program.TradeProgramId,
                ReviewedByUserId = reviewerId,
                Result = ComplianceResult.Compliant,
                ReviewDate = DateTime.UtcNow.AddDays(-15),
                Findings = "All program requirements are being met. Documentation is complete.",
                Recommendations = "Continue monitoring quarterly reports.",
                CreatedAt = DateTime.UtcNow
            });

            records.Add(new ComplianceRecord
            {
                TradeProgramId = program.TradeProgramId,
                ReviewedByUserId = reviewerId,
                Result = ComplianceResult.NonCompliant,
                ReviewDate = DateTime.UtcNow.AddDays(-7),
                Findings = "Missing quarterly financial reports. Late submission of documentation.",
                Recommendations = "Submit outstanding reports within 14 days. Implement document tracking system.",
                FollowUpDate = DateTime.UtcNow.AddDays(14),
                CreatedAt = DateTime.UtcNow
            });
        }

        records.Add(new ComplianceRecord
        {
            TradeProgramId = programs[0].TradeProgramId,
            ReviewedByUserId = reviewerId,
            Result = ComplianceResult.UnderReview,
            ReviewDate = DateTime.UtcNow.AddDays(-2),
            Findings = "Initial review in progress. Awaiting additional documentation.",
            Recommendations = "Pending final assessment.",
            FollowUpDate = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        });

        return records;
    }

    private static List<AuditLog> CreateAuditLogs(List<User> users) =>
    [
        new()
        {
            UserId = users[0].UserId,
            Action = "User Login",
            Resource = "Authentication",
            Details = "Admin user logged in successfully",
            Timestamp = DateTime.UtcNow.AddHours(-2),
            IpAddress = "192.168.1.100"
        },
        new()
        {
            UserId = users[1].UserId,
            Action = "License Approved",
            Resource = "TradeLicense",
            Details = "Approved license TL-2024-00001",
            Timestamp = DateTime.UtcNow.AddDays(-5),
            IpAddress = "192.168.1.101"
        },
        new()
        {
            UserId = users[2].UserId,
            Action = "Compliance Review Created",
            Resource = "ComplianceRecord",
            Details = "Created compliance review for Export Incentive Program",
            Timestamp = DateTime.UtcNow.AddDays(-7),
            IpAddress = "192.168.1.102"
        },
        new()
        {
            UserId = users[3].UserId,
            Action = "License Application Submitted",
            Resource = "TradeLicense",
            Details = "Submitted new license application",
            Timestamp = DateTime.UtcNow.AddDays(-10),
            IpAddress = "203.45.67.89"
        }
    ];
}
