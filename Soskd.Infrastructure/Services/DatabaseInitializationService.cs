// Soskd.Infrastructure/Services/DatabaseInitializationService.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Soskd.Infrastructure.Data;

namespace Soskd.Infrastructure.Services
{
    public static class DatabaseInitializationService
    {
        public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider, ILogger logger)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                logger.LogInformation("Starting database initialization...");

                // Ensure database is created and migrations are applied
                await context.Database.MigrateAsync();

                logger.LogInformation("Database initialization completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }
    }
}