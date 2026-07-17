using LuxorLMS.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Analytics.Infrastructure.Persistence;

public class LuxorLMSAnalyticsDbContext : DbContext
{
    public LuxorLMSAnalyticsDbContext(DbContextOptions<LuxorLMSAnalyticsDbContext> options) : base(options)
    {
    }

    public DbSet<AnalyticsKpi> AnalyticsKpis => Set<AnalyticsKpi>();
    public DbSet<GpaTrend> GpaTrends => Set<GpaTrend>();
    public DbSet<GradeDistribution> GradeDistributions => Set<GradeDistribution>();
    public DbSet<ServerHealthMetric> ServerHealthMetrics => Set<ServerHealthMetric>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AnalyticsKpi>(builder =>
        {
            builder.ToTable("AnalyticsKpis", "analytics");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Key).HasMaxLength(128).IsRequired();
            builder.Property(x => x.Name).HasMaxLength(255).IsRequired();
            builder.HasIndex(x => new { x.Key, x.CourseOfferingId, x.TimeRange, x.PeriodStart });
            builder.HasIndex(x => new { x.DepartmentId, x.ProgramId, x.MetricType, x.PeriodStart });
        });

        modelBuilder.Entity<GpaTrend>(builder =>
        {
            builder.ToTable("GpaTrends", "analytics");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new { x.StudentId, x.SemesterNumber });
            builder.HasIndex(x => x.CourseOfferingId);
        });

        modelBuilder.Entity<GradeDistribution>(builder =>
        {
            builder.ToTable("GradeDistributions", "analytics");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.CourseOfferingId);
            builder.HasIndex(x => x.GradeLetter);
        });

        modelBuilder.Entity<ServerHealthMetric>(builder =>
        {
            builder.ToTable("ServerHealthMetrics", "analytics");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.RecordedAt);
            builder.HasIndex(x => x.Status);
        });
    }
}
