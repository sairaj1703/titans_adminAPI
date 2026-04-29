using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using titans_admin.Data;
using titans_admin.Repositories;
using titans_admin.Repositories.Interfaces;
using titans_admin.Services;
using titans_admin.Services.Interfaces;

// Create a builder for the web application
var builder = WebApplication.CreateBuilder(args);

// Add services to the container for controllers with views (Razor Pages/MVC)
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Fix circular reference exceptions in JSON serialization
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

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

// Configure CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("TradeNetPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5005")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured in appsettings.json");
var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured in appsettings.json");
var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience not configured in appsettings.json");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Configure Authorization
builder.Services.AddAuthorization();

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

// Middleware pipeline in specific order
app.UseRouting();

// Apply CORS policy
app.UseCors("TradeNetPolicy");

// Apply Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map static assets (e.g., CSS, JS, images)
app.MapStaticAssets();

// Configure the default route for controllers (Razor Pages/MVC)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Index}/{id?}")
    .WithStaticAssets();

// Run the application
app.Run();
