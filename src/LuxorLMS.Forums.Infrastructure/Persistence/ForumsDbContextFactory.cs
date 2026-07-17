using LuxorLMS.Forums.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LuxorLMS.Forums.Infrastructure.Persistence;

public class ForumsDbContextFactory : IDesignTimeDbContextFactory<LuxorLMSForumsDbContext>
{
    public LuxorLMSForumsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LuxorLMSForumsDbContext>();
        optionsBuilder.UseSqlite("Data Source=local_luxorlms.db");

        return new LuxorLMSForumsDbContext(optionsBuilder.Options);
    }
}

