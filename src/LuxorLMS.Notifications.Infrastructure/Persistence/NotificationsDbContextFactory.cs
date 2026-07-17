using LuxorLMS.Notifications.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LuxorLMS.Notifications.Infrastructure.Persistence;

public class NotificationsDbContextFactory : IDesignTimeDbContextFactory<LuxorLMSNotificationsDbContext>
{
    public LuxorLMSNotificationsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LuxorLMSNotificationsDbContext>();
        optionsBuilder.UseSqlite("Data Source=local_luxorlms.db");

        return new LuxorLMSNotificationsDbContext(optionsBuilder.Options);
    }
}

