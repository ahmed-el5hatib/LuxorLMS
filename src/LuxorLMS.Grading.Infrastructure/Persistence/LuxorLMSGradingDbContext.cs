using LuxorLMS.Grading.Domain.Entities;
using LuxorLMS.Grading.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LuxorLMS.Grading.Infrastructure.Persistence;

public class LuxorLMSGradingDbContext : DbContext
{
    public LuxorLMSGradingDbContext(DbContextOptions<LuxorLMSGradingDbContext> options)
        : base(options)
    {
    }

    public DbSet<GradeCategory> GradeCategories => Set<GradeCategory>();
    public DbSet<GradeComponent> GradeComponents => Set<GradeComponent>();
    public DbSet<StudentGrade> StudentGrades => Set<StudentGrade>();
    public DbSet<GradeAppeal> GradeAppeals => Set<GradeAppeal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var publishStatusConverter = new EnumToNumberConverter<GradePublishStatus, int>();
        var appealStatusConverter = new EnumToNumberConverter<AppealStatus, int>();

        modelBuilder.Entity<GradeCategory>(entity =>
        {
            entity.HasKey(gc => gc.Id);
            entity.Property(gc => gc.Id).ValueGeneratedNever();

            entity.Property(gc => gc.CourseOfferingId).IsRequired();
            entity.Property(gc => gc.Name).IsRequired().HasMaxLength(100);
            entity.Property(gc => gc.Weight).IsRequired().HasColumnType("numeric(5,4)");
            entity.Property(gc => gc.DisplayOrder).IsRequired();
            entity.Property(gc => gc.IsActive).IsRequired();
            entity.Property(gc => gc.CreatedAt).IsRequired();

            entity.HasIndex(gc => gc.CourseOfferingId);
        });

        modelBuilder.Entity<GradeComponent>(entity =>
        {
            entity.HasKey(gc => gc.Id);
            entity.Property(gc => gc.Id).ValueGeneratedNever();

            entity.Property(gc => gc.GradeCategoryId).IsRequired();
            entity.Property(gc => gc.Title).IsRequired().HasMaxLength(150);
            entity.Property(gc => gc.MaxPoints).IsRequired().HasColumnType("numeric(7,2)");
            entity.Property(gc => gc.IsActive).IsRequired();
            entity.Property(gc => gc.CreatedAt).IsRequired();

            entity.HasIndex(gc => gc.GradeCategoryId);
        });

        modelBuilder.Entity<StudentGrade>(entity =>
        {
            entity.HasKey(sg => sg.Id);
            entity.Property(sg => sg.Id).ValueGeneratedNever();

            entity.Property(sg => sg.CourseOfferingId).IsRequired();
            entity.Property(sg => sg.StudentId).IsRequired();
            entity.Property(sg => sg.CourseId).IsRequired();
            entity.Property(sg => sg.SemesterId).IsRequired();
            entity.Property(sg => sg.CreditHours).IsRequired();
            entity.Property(sg => sg.RawScore).IsRequired().HasColumnType("numeric(5,2)");
            entity.Property(sg => sg.GradeLetter).IsRequired().HasMaxLength(10);
            entity.Property(sg => sg.GradePoints).IsRequired().HasColumnType("numeric(4,2)");
            entity.Property(sg => sg.PublishStatus)
                .IsRequired()
                .HasConversion(publishStatusConverter);
            entity.Property(sg => sg.CreatedAt).IsRequired();

            entity.HasIndex(sg => new { sg.CourseOfferingId, sg.StudentId }).IsUnique();
            entity.HasIndex(sg => sg.StudentId);
            entity.HasIndex(sg => sg.SemesterId);
        });

        modelBuilder.Entity<GradeAppeal>(entity =>
        {
            entity.HasKey(ga => ga.Id);
            entity.Property(ga => ga.Id).ValueGeneratedNever();

            entity.Property(ga => ga.StudentGradeId).IsRequired();
            entity.Property(ga => ga.StudentId).IsRequired();
            entity.Property(ga => ga.Reason).IsRequired().HasMaxLength(2000);
            entity.Property(ga => ga.Status)
                .IsRequired()
                .HasConversion(appealStatusConverter);
            entity.Property(ga => ga.Resolution).HasMaxLength(2000);
            entity.Property(ga => ga.CreatedAt).IsRequired();

            entity.HasIndex(ga => ga.StudentGradeId);
            entity.HasIndex(ga => ga.StudentId);
        });
    }
}
