using LuxorLMS.Quizzes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LuxorLMS.Quizzes.Infrastructure;

public class LuxorLMSQuizzesDbContextFactory : IDesignTimeDbContextFactory<LuxorLMSQuizzesDbContext>
{
    public LuxorLMSQuizzesDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=luxorlms_quizzes;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<LuxorLMSQuizzesDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new LuxorLMSQuizzesDbContext(optionsBuilder.Options);
    }
}
