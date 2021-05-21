using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Discount.API.Extension
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {

            var retryConnection = 0;

            if (retry != null)
            {
                retryConnection = retry.Value;
            }

            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var configuration = services.GetRequiredService<IConfiguration>();
            var logger = services.GetRequiredService<ILogger<TContext>>();

            try
            {
                logger.LogInformation($"Migrating database");

                using var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                connection.Open();

                using var command = new NpgsqlCommand
                {
                    CommandText = "DROP TABLE IF EXISTS Coupon", Connection = connection
                };

                Seed(command);

                logger.LogInformation("Successfully migrated to database");
            }
            catch (NpgsqlException ex)
            {
                logger.LogError(ex, "An error occurred when migrating to db");

                if (retryConnection < 50)
                {
                    retryConnection++;
                    System.Threading.Thread.Sleep(2000);
                    MigrateDatabase<TContext>(host, retryConnection);
                }
            }

            return host;
        }

        private static void Seed(NpgsqlCommand command)
        {
            command.ExecuteNonQuery();

            command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY, ProductName VARCHAR(24) NOT NULL, Description TEXT, Amount INT)";
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Playstation 4', 'Playstation 4 discount', 500)";
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Playstation 5', 'Playstation 5 discount', 400)";
            command.ExecuteNonQuery();
        }
    }
}
