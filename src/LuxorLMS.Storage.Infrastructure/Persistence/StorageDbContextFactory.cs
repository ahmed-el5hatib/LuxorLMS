using LuxorLMS.Storage.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LuxorLMS.Storage.Infrastructure.Persistence;

public class StorageDbContextFactory : IDesignTimeDbContextFactory<LuxorLMSStorageDbContext>
{
    public LuxorLMSStorageDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LuxorLMSStorageDbContext>();
        optionsBuilder.UseSqlite("Data Source=local_luxorlms.db");

        return new LuxorLMSStorageDbContext(optionsBuilder.Options);
    }
}

