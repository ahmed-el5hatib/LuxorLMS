using LuxorLMS.Registration.Domain.Entities;
using LuxorLMS.Registration.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LuxorLMS.Registration.Infrastructure.Persistence;

public class LuxorLMSRegistrationDbContext : DbContext
{
    public LuxorLMSRegistrationDbContext(DbContextOptions<LuxorLMSRegistrationDbContext> options)
        : base(options)
    {
    }

    public DbSet<RegistrationPeriod> RegistrationPeriods => Set<RegistrationPeriod>();
    public DbSet<StudentProgramEnrollment> StudentProgramEnrollments => Set<StudentProgramEnrollment>();
    public DbSet<CourseEnrollment> CourseEnrollments => Set<CourseEnrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var enrollmentStatusConverter = new EnumToNumberConverter<EnrollmentStatus, int>();
        var studentProgramStatusConverter = new EnumToNumberConverter<StudentProgramStatus, int>();
        var enrollmentTypeConverter = new EnumToNumberConverter<EnrollmentType, int>();

        modelBuilder.Entity<RegistrationPeriod>(entity =>
        {
            entity.HasKey(rp => rp.Id);
            entity.Property(rp => rp.Id).ValueGeneratedNever();

            entity.Property(rp => rp.SemesterId).IsRequired();
            entity.Property(rp => rp.AcademicYearId).IsRequired();
            entity.Property(rp => rp.StartDate).IsRequired();
            entity.Property(rp => rp.EndDate).IsRequired();
            entity.Property(rp => rp.MinCreditHours).IsRequired();
            entity.Property(rp => rp.MaxCreditHours).IsRequired();
            entity.Property(rp => rp.GpaCapForMax).IsRequired();
            entity.Property(rp => rp.IsActive).IsRequired();
            entity.Property(rp => rp.CreatedAt).IsRequired();

            entity.HasIndex(rp => rp.SemesterId);
            entity.HasIndex(rp => rp.AcademicYearId);
            entity.HasIndex(rp => rp.IsActive);
        });

        modelBuilder.Entity<StudentProgramEnrollment>(entity =>
        {
            entity.HasKey(spe => spe.Id);
            entity.Property(spe => spe.Id).ValueGeneratedNever();

            entity.Property(spe => spe.StudentId).IsRequired();
            entity.Property(spe => spe.ProgramId).IsRequired();
            entity.Property(spe => spe.EnrollmentDate).IsRequired();
            entity.Property(spe => spe.Status)
                .IsRequired()
                .HasConversion(studentProgramStatusConverter);
            entity.Property(spe => spe.CreatedAt).IsRequired();

            entity.HasIndex(spe => new { spe.StudentId, spe.ProgramId });
        });

        modelBuilder.Entity<CourseEnrollment>(entity =>
        {
            entity.HasKey(ce => ce.Id);
            entity.Property(ce => ce.Id).ValueGeneratedNever();

            entity.Property(ce => ce.StudentId).IsRequired();
            entity.Property(ce => ce.CourseId).IsRequired();
            entity.Property(ce => ce.SemesterId).IsRequired();
            entity.Property(ce => ce.EnrollmentType)
                .IsRequired()
                .HasConversion(enrollmentTypeConverter);
            entity.Property(ce => ce.Status)
                .IsRequired()
                .HasConversion(enrollmentStatusConverter);
            entity.Property(ce => ce.CreditHours).IsRequired();
            entity.Property(ce => ce.GradeLetter).HasMaxLength(10);
            entity.Property(ce => ce.RequestedAt).IsRequired();
            entity.Property(ce => ce.CreatedAt).IsRequired();

            entity.HasIndex(ce => new { ce.StudentId, ce.CourseId, ce.SemesterId }).IsUnique();
            entity.HasIndex(ce => ce.StudentId);
            entity.HasIndex(ce => ce.SemesterId);
        });
    }
}
