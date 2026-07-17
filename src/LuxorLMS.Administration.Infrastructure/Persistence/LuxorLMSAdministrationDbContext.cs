using LuxorLMS.Administration.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Administration.Infrastructure.Persistence;

public class LuxorLMSAdministrationDbContext : DbContext
{
    public LuxorLMSAdministrationDbContext(DbContextOptions<LuxorLMSAdministrationDbContext> options) : base(options)
    {
    }

    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<SystemLog> SystemLogs => Set<SystemLog>();
    public DbSet<BackgroundJobInfo> BackgroundJobInfos => Set<BackgroundJobInfo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SystemSetting>(builder =>
        {
            builder.ToTable("SystemSettings", "administration");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Key).HasMaxLength(128).IsRequired();
            builder.HasIndex(x => x.Key).IsUnique();
            builder.HasIndex(x => x.IsSensitive);
        });

        modelBuilder.Entity<SystemLog>(builder =>
        {
            builder.ToTable("SystemLogs", "administration");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Timestamp);
            builder.HasIndex(x => x.Level);
            builder.HasIndex(x => x.Category);
        });

        modelBuilder.Entity<BackgroundJobInfo>(builder =>
        {
            builder.ToTable("BackgroundJobInfos", "administration");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.JobId).HasMaxLength(128).IsRequired();
            builder.HasIndex(x => x.JobId).IsUnique();
            builder.HasIndex(x => x.State);
        });
    }
}
