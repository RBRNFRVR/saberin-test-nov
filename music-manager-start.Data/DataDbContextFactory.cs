using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace music_manager_starter.Data
{
    public class DataDbContextFactory : IDesignTimeDbContextFactory<DataDbContext>
    {
        public DataDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataDbContext>();

            // Get the connection string from the appsettings.json in the Server folder
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Set the base path of the current directory
                .AddJsonFile("C:\\Users\\19174\\RiderProjects\\main\\Server\\appsettings.json", optional: false, reloadOnChange: true) // Adjusted path to the server folder
                .Build();

            var connectionString = configuration.GetConnectionString("Default");

            optionsBuilder.UseSqlite(connectionString);

            return new DataDbContext(optionsBuilder.Options);
        }
    }
}