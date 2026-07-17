using LuxorLMS.Analytics.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LuxorLMS.Analytics.Infrastructure.Persistence;

public class AnalyticsDbContextFactory : IDesignTimeDbContextFactory<LuxorLMSAnalyticsDbContext>
{
    public LuxorLMSAnalyticsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LuxorLMSAnalyticsDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=luxorlms_analytics;Username=postgres;Password=postgres");

        return new LuxorLMSAnalyticsDbContext(optionsBuilder.Options);
    }
}
