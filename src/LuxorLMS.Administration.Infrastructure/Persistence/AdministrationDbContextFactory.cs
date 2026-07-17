using LuxorLMS.Administration.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LuxorLMS.Administration.Infrastructure.Persistence;

public class AdministrationDbContextFactory : IDesignTimeDbContextFactory<LuxorLMSAdministrationDbContext>
{
    public LuxorLMSAdministrationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LuxorLMSAdministrationDbContext>();
        optionsBuilder.UseSqlite("Data Source=local_luxorlms.db");

        return new LuxorLMSAdministrationDbContext(optionsBuilder.Options);
    }
}

