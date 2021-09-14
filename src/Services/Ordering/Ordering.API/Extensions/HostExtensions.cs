using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ordering.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(
            this IHost host, 
            Action<TContext, IServiceProvider> seeder,
            int? retry = 0) where TContext : DbContext
        {
            var retryForAvailability = retry.Value;

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<DbContext>>();
                var context = services.GetRequiredService<TContext>();

                try
                {
                    logger.LogInformation("Migrating db associated with context {DbContextName}", typeof(TContext).Name);

                    InvokeSeeder(seeder, context, services);

                    logger.LogInformation("Migrated db associated with context {DbContextName}", typeof(TContext).Name);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "There was an error when migrating to the database using context {DbMigrateName}", typeof(TContext).Name);

                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, seeder, retryForAvailability);
                    }
                }
            }

            return host;
        }

        public static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context,
            IServiceProvider services) where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context, services);
        }
    }
}
