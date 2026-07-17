using LuxorLMS.Academic.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LuxorLMS.Academic.Infrastructure;

public class LuxorLMSAcademicDbContextFactory : IDesignTimeDbContextFactory<LuxorLMSAcademicDbContext>
{
    public LuxorLMSAcademicDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=luxorlms_academic;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<LuxorLMSAcademicDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new LuxorLMSAcademicDbContext(optionsBuilder.Options);
    }
}
