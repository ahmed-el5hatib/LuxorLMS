using LuxorLMS.Administration.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LuxorLMS.Administration.Infrastructure.Persistence;

public class AdministrationDbContextFactory : IDesignTimeDbContextFactory<LuxorLMSAdministrationDbContext>
{
    public LuxorLMSAdministrationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LuxorLMSAdministrationDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=luxorlms_administration;Username=postgres;Password=postgres");

        return new LuxorLMSAdministrationDbContext(optionsBuilder.Options);
    }
}
