using LuxorLMS.Reporting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LuxorLMS.Reporting.Infrastructure.Persistence;

public class ReportingDbContextFactory : IDesignTimeDbContextFactory<LuxorLMSReportingDbContext>
{
    public LuxorLMSReportingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LuxorLMSReportingDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=luxorlms_reporting;Username=postgres;Password=postgres");

        return new LuxorLMSReportingDbContext(optionsBuilder.Options);
    }
}
