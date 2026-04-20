
using Microsoft.EntityFrameworkCore;
using titans_admin.Data;
using titans_admin.Repositories;
using titans_admin.Repositories.Interfaces;
using titans_admin.Services;
using titans_admin.Services.Interfaces;

namespace titans_admin.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers();

            // Add Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register the HTTP context accessor
            builder.Services.AddHttpContextAccessor();

            // Configure Entity Framework Core
            builder.Services.AddDbContext<TradeNetDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register repository and service dependencies
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ITradeLicenseRepository, TradeLicenseRepository>();
            builder.Services.AddScoped<ITradeProgramRepository, TradeProgramRepository>();
            builder.Services.AddScoped<IComplianceRecordRepository, ComplianceRecordRepository>();
            builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            builder.Services.AddScoped<IAdminService, AdminService>();

            // Add CORS support
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                // Swagger UI
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Titans Admin API v1");
                    c.RoutePrefix = "swagger"; // serve at /swagger
                });
            }

            app.UseHttpsRedirection();

            // Enable CORS
            app.UseCors("AllowAll");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
