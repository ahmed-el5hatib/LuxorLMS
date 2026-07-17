using LuxorLMS.Analytics.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LuxorLMS.Analytics.Infrastructure.Persistence;

public class AnalyticsDbContextFactory : IDesignTimeDbContextFactory<LuxorLMSAnalyticsDbContext>
{
    public LuxorLMSAnalyticsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LuxorLMSAnalyticsDbContext>();
        optionsBuilder.UseSqlite("Data Source=local_luxorlms.db");

        return new LuxorLMSAnalyticsDbContext(optionsBuilder.Options);
    }
}

