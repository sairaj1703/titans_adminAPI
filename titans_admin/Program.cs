using Microsoft.EntityFrameworkCore;
using titans_admin.Data;
using titans_admin.Repositories;
using titans_admin.Repositories.Interfaces;
using titans_admin.Services;
using titans_admin.Services.Interfaces;

// Create a builder for the web application
var builder = WebApplication.CreateBuilder(args);

// Add services to the container for controllers with views (Razor Pages/MVC)
builder.Services.AddControllersWithViews();

// Register the HTTP context accessor for dependency injection
builder.Services.AddHttpContextAccessor();

// Configure Entity Framework Core to use SQL Server with the connection string from configuration
builder.Services.AddDbContext<TradeNetDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repository and service dependencies for dependency injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITradeLicenseRepository, TradeLicenseRepository>();
builder.Services.AddScoped<ITradeProgramRepository, TradeProgramRepository>();
builder.Services.AddScoped<IComplianceRecordRepository, ComplianceRecordRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();

// Build the application
var app = builder.Build();

// Apply any pending database migrations at startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TradeNetDbContext>();
    try
    {
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Log any errors that occur during migration
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while applying migrations.");
    }
}

// Configure error handling and HSTS for non-development environments
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Admin/Index");
    app.UseHsts();
}

// Redirect HTTP requests to HTTPS
app.UseHttpsRedirection();

// Enable routing
app.UseRouting();

// Map static assets (e.g., CSS, JS, images)
app.MapStaticAssets();

// Configure the default route for controllers (Razor Pages/MVC)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Index}/{id?}")
    .WithStaticAssets();


// Run the application
app.Run();
