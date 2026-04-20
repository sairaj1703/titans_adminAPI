# Titans Admin API - Quick Start Guide

## Overview

The Titans Admin API is now fully implemented in the `titans_admin.API` project. This is an ASP.NET Core Web API built with .NET 10 that provides REST endpoints for managing:

- Users
- Trade Licenses
- Trade Programs
- Compliance Records
- Audit Logs
- Admin Dashboard

## Project Structure

```
titans_admin.API/
├── Controllers/
│   ├── AdminApiController.cs         (Dashboard endpoints)
│   ├── UsersController.cs             (User management)
│   ├── TradeLicensesController.cs     (Trade License management)
│   ├── TradeProgramsController.cs     (Trade Program management)
│   ├── ComplianceRecordsController.cs (Compliance management)
│   ├── AuditLogsController.cs         (Audit log endpoints)
│   └── HealthController.cs            (Health check endpoint)
├── Services/
│   ├── Interfaces/
│   │   └── IAdminService.cs           (Service interface)
│   └── AdminService.cs                (Business logic)
├── Repositories/
│   ├── Interfaces/
│   │   └── I*Repository.cs            (Repository interfaces)
│   └── *Repository.cs                 (Data access implementations)
├── Models/
│   └── ViewModels/
│       └── *ViewModel.cs              (Data transfer objects)
├── Program.cs                         (Configuration & Startup)
└── titans_admin.API.csproj            (Project file)
```

## Running the API

### Prerequisites
- .NET 10 SDK installed
- SQL Server configured
- Connection string configured in `appsettings.json`

### Steps to Run

1. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

2. **Apply database migrations**:
   ```bash
   dotnet ef database update
   ```
   (Migrations are also applied at startup automatically)

3. **Run the API**:
   ```bash
   dotnet run
   ```

   Or from Visual Studio:
   - Set `titans_admin.API` as the startup project
   - Press F5 or click the Run button

4. **Access the API**:
   - Default URL: `https://localhost:5001` (or the configured port)
   - OpenAPI/Swagger UI: `https://localhost:5001/swagger` (if Swagger is enabled)

## Configuration

### appsettings.json

Ensure your connection string is configured:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=TradeNetDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### CORS Configuration

The API is configured to accept requests from all origins. Modify in `Program.cs` if you need to restrict:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
```

## API Endpoints Summary

### Health Check
- `GET /api/health` - Check API status

### Admin Dashboard
- `GET /api/adminapi/dashboard` - Get dashboard statistics
- `GET /api/adminapi/violations-count` - Get violations count

### Users
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user
- `GET /api/users/by-role/{role}` - Get users by role
- `GET /api/users/by-status/{status}` - Get users by status
- `PATCH /api/users/{id}/status` - Update user status

### Trade Licenses
- `GET /api/tradelicenses` - Get all licenses
- `GET /api/tradelicenses/{id}` - Get license by ID
- `POST /api/tradelicenses` - Create license
- `PUT /api/tradelicenses/{id}` - Update license
- `DELETE /api/tradelicenses/{id}` - Delete license
- `GET /api/tradelicenses/by-status/{status}` - Get licenses by status

### Trade Programs
- `GET /api/tradeprograms` - Get all programs
- `GET /api/tradeprograms/active` - Get active programs
- `GET /api/tradeprograms/{id}` - Get program by ID
- `POST /api/tradeprograms` - Create program
- `PUT /api/tradeprograms/{id}` - Update program
- `DELETE /api/tradeprograms/{id}` - Delete program

### Compliance Records
- `GET /api/compliancerecords` - Get all records
- `GET /api/compliancerecords/{id}` - Get record by ID
- `POST /api/compliancerecords` - Create record
- `PUT /api/compliancerecords/{id}` - Update record
- `DELETE /api/compliancerecords/{id}` - Delete record
- `GET /api/compliancerecords/tracking` - Get compliance tracking

### Audit Logs
- `GET /api/auditlogs` - Get audit logs (paginated)
- `GET /api/auditlogs/user/{userId}` - Get logs by user

## Testing the API

### Using Swagger/OpenAPI (Development only)
1. Navigate to `https://localhost:5001/swagger`
2. Explore and test endpoints interactively

### Using curl

Example - Get all users:
```bash
curl -X GET "https://localhost:5001/api/users" \
  -H "Content-Type: application/json"
```

Example - Create a user:
```bash
curl -X POST "https://localhost:5001/api/users" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "password": "SecurePass123",
    "role": "Business",
    "status": "Active"
  }'
```

### Using Postman

1. Import the API endpoints
2. Set base URL: `https://localhost:5001`
3. Use the endpoints listed above

## Key Features

✅ **Full CRUD Operations** - Create, Read, Update, Delete for all resources
✅ **Filtering & Search** - Filter users by role/status, licenses by status, etc.
✅ **Pagination** - Support for paginated results in audit logs
✅ **Error Handling** - Comprehensive error responses with meaningful messages
✅ **Data Validation** - Request validation and model state checking
✅ **Logging** - All operations are logged for audit purposes
✅ **CORS Support** - Cross-origin requests enabled
✅ **RESTful Design** - Standard HTTP methods and status codes

## Project References

The `titans_admin.API` project references the main `titans_admin` project to use:
- Shared entity models
- Enums and constants
- ViewModels
- Data access layer
- Service interfaces

## Dependency Injection

The API uses ASP.NET Core's built-in dependency injection configured in `Program.cs`:

```csharp
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITradeLicenseRepository, TradeLicenseRepository>();
builder.Services.AddScoped<ITradeProgramRepository, TradeProgramRepository>();
builder.Services.AddScoped<IComplianceRecordRepository, ComplianceRecordRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();
```

## Troubleshooting

### Build Issues
If you encounter build errors:
1. Clean solution: `dotnet clean`
2. Restore NuGet packages: `dotnet restore`
3. Rebuild: `dotnet build`

### Database Connection Issues
- Verify connection string in `appsettings.json`
- Ensure SQL Server is running
- Check database credentials

### CORS Issues
If requests are blocked:
- Verify CORS policy in `Program.cs`
- Check request origin matches allowed origins
- Add Authorization headers if needed

## Additional Resources

- [API Documentation](./API_DOCUMENTATION.md)
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [REST API Best Practices](https://restfulapi.net/)

## Notes

- The API project is set as the startup project
- All controllers inherit from `ControllerBase` for API endpoints
- Response codes follow HTTP standards (200, 201, 400, 404, 500)
- All database operations are asynchronous
- Logging is implemented using ILogger interface
