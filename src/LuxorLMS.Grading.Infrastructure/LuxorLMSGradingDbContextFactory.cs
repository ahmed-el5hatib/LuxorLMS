using LuxorLMS.Grading.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LuxorLMS.Grading.Infrastructure;

public class LuxorLMSGradingDbContextFactory : IDesignTimeDbContextFactory<LuxorLMSGradingDbContext>
{
    public LuxorLMSGradingDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=luxorlms_grading;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<LuxorLMSGradingDbContext>();
        optionsBuilder.UseSqlite("Data Source=local_luxorlms.db");

        return new LuxorLMSGradingDbContext(optionsBuilder.Options);
    }
}

