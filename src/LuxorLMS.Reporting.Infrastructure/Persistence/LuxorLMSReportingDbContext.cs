using LuxorLMS.Reporting.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Reporting.Infrastructure.Persistence;

public class LuxorLMSReportingDbContext : DbContext
{
    public LuxorLMSReportingDbContext(DbContextOptions<LuxorLMSReportingDbContext> options) : base(options)
    {
    }

    public DbSet<ReportJob> ReportJobs => Set<ReportJob>();
    public DbSet<ReportTemplate> ReportTemplates => Set<ReportTemplate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ReportJob>(builder =>
        {
            builder.ToTable("ReportJobs", "reporting");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new { x.RequestedBy, x.RequestedAt });
            builder.HasIndex(x => x.Status);
        });

        modelBuilder.Entity<ReportTemplate>(builder =>
        {
            builder.ToTable("ReportTemplates", "reporting");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Code).HasMaxLength(128).IsRequired();
            builder.HasIndex(x => x.Code).IsUnique();
        });
    }
}
