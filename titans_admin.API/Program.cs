using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using titans_admin.Data;
using titans_admin.Repositories;
using titans_admin.Repositories.Interfaces;
using titans_admin.Services;
using titans_admin.Services.Interfaces;

// Create a builder for the web application
var builder = WebApplication.CreateBuilder(args);

// Add services to the container for API controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Fix circular reference exceptions in JSON serialization
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TradeNet Admin API",
        Version = "v1",
        Description = "REST API for TradeNet Administration",
        Contact = new OpenApiContact
        {
            Name = "TradeNet Support"
        }
    });

    // Fix duplicate schema names by using fully qualified type names
    options.CustomSchemaIds(type => type.FullName);

    // Add JWT Bearer security definition
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
    });

    // Add security requirement to all endpoints
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
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
        policy.WithOrigins("http://localhost:5005", "http://localhost:5182")
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

// Configure Swagger UI in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TradeNet Admin API v1");
        options.DefaultModelsExpandDepth(2);
    });
}

// Configure error handling and HSTS for non-development environments
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/api/error");
    app.UseHsts();
}

// Redirect HTTP requests to HTTPS
app.UseHttpsRedirection();

// Middleware pipeline in specific order: Routing, CORS, Authentication, Authorization
app.UseRouting();

// Apply CORS policy BEFORE authentication
app.UseCors("TradeNetPolicy");

// Apply Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Run the application
app.Run();
