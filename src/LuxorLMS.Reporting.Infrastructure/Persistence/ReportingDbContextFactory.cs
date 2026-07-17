using LuxorLMS.Reporting.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LuxorLMS.Reporting.Infrastructure.Persistence;

public class ReportingDbContextFactory : IDesignTimeDbContextFactory<LuxorLMSReportingDbContext>
{
    public LuxorLMSReportingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LuxorLMSReportingDbContext>();
        optionsBuilder.UseSqlite("Data Source=local_luxorlms.db");

        return new LuxorLMSReportingDbContext(optionsBuilder.Options);
    }
}

