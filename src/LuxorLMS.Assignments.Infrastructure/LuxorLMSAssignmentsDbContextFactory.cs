using LuxorLMS.Assignments.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LuxorLMS.Assignments.Infrastructure;

public class LuxorLMSAssignmentsDbContextFactory : IDesignTimeDbContextFactory<LuxorLMSAssignmentsDbContext>
{
    public LuxorLMSAssignmentsDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=luxorlms_assignments;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<LuxorLMSAssignmentsDbContext>();
        optionsBuilder.UseSqlite("Data Source=local_luxorlms.db");

        return new LuxorLMSAssignmentsDbContext(optionsBuilder.Options);
    }
}

