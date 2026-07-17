using LuxorLMS.Attendance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LuxorLMS.Attendance.Infrastructure;

public class LuxorLMSAttendanceDbContextFactory : IDesignTimeDbContextFactory<LuxorLMSAttendanceDbContext>
{
    public LuxorLMSAttendanceDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=luxorlms_attendance;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<LuxorLMSAttendanceDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new LuxorLMSAttendanceDbContext(optionsBuilder.Options);
    }
}
