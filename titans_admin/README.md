# Titans Admin

Trade License Administration System built with ASP.NET Core MVC (.NET 10).

## Project Structure

```
titans_admin/
├── Controllers/          # MVC Controllers
│   ├── AdminController   # Main admin operations (CRUD for all entities)
│   └── AuthController    # Authentication (login/register)
├── Data/
│   ├── TradeNetDbContext # EF Core database context
│   └── DatabaseSeeder    # Test data seeding
├── Models/
│   ├── Entities/         # Domain models (User, TradeLicense, TradeProgram, etc.)
│   ├── Enums/            # UserRole, UserStatus, ComplianceResult
│   └── ViewModels/       # DTOs for views
├── Repositories/         # Data access layer
│   └── Interfaces/       # Repository contracts
├── Services/             # Business logic layer
│   └── Interfaces/       # Service contracts
├── Utilities/            # Helper classes (PasswordHashGenerator)
├── Views/                # Razor views
│   ├── Admin/            # Admin dashboard and CRUD views
│   └── Shared/           # Layout and shared partials
└── Migrations/           # EF Core migrations
```

## Key Features

- **User Management**: Create, edit, delete users with role-based access
- **Trade Licenses**: Manage business trade license applications
- **Trade Programs**: Configure and track trade programs
- **Compliance Records**: Track compliance reviews and results
- **Audit Logging**: View system activity logs

## Getting Started

1. Update connection string in `appsettings.json`
2. Run: `dotnet ef database update`
3. Run: `dotnet run`
4. Navigate to `https://localhost:5001/Admin`

## Default Test Accounts

| Email                    | Password       | Role              |
|--------------------------|----------------|-------------------|
| admin@tradenet.gov       | Admin@123      | Administrator     |
| officer1@tradenet.gov    | Officer@123    | Trade Officer     |
| compliance1@tradenet.gov | Compliance@123 | Compliance Officer|
| trader@example.com       | Trader@123     | Business/Trader   |

## User Types

| Role       | Description                                      |
|------------|--------------------------------------------------|
| Business   | Business owners and traders                      |
| Officer    | Trade officers handling license applications     |
| Manager    | Program managers overseeing trade programs       |
| Admin      | System administrators with full access           |
| Compliance | Compliance officers conducting reviews           |
| Auditor    | Government auditors for system oversight         |

## Technology Stack

- .NET 10 / ASP.NET Core MVC
- Entity Framework Core (SQL Server)
- Razor Views

## API Reference

### Registration
`POST /Auth/Register`
```json
{
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "password": "Password@123",
    "confirmPassword": "Password@123",
    "role": "Business",
    "status": "PendingApproval",
    "phoneNumber": "+1-555-0100",
    "address": "123 Main St"
}
```

### Helper Endpoints
- `GET /Auth/UserTypes` - Returns available user roles
- `GET /Auth/UserStatuses` - Returns available user statuses
