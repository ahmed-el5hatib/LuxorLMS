using LuxorLMS.Registration.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LuxorLMS.Registration.Infrastructure;

public class LuxorLMSRegistrationDbContextFactory : IDesignTimeDbContextFactory<LuxorLMSRegistrationDbContext>
{
    public LuxorLMSRegistrationDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=luxorlms_registration;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<LuxorLMSRegistrationDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new LuxorLMSRegistrationDbContext(optionsBuilder.Options);
    }
}
