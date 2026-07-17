using LuxorLMS.Academic.Domain.Entities;
using LuxorLMS.Academic.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LuxorLMS.Academic.Infrastructure.Persistence;

public class LuxorLMSAcademicDbContext : DbContext
{
    public LuxorLMSAcademicDbContext(DbContextOptions<LuxorLMSAcademicDbContext> options)
        : base(options)
    {
    }

    public DbSet<Faculty> Faculties => Set<Faculty>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Program> Programs => Set<Program>();
    public DbSet<StudyPlan> StudyPlans => Set<StudyPlan>();
    public DbSet<StudyPlanCourse> StudyPlanCourses => Set<StudyPlanCourse>();
    public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>();
    public DbSet<Semester> Semesters => Set<Semester>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CoursePrerequisite> CoursePrerequisites => Set<CoursePrerequisite>();
    public DbSet<CourseOffering> CourseOfferings => Set<CourseOffering>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<SectionMember> SectionMembers => Set<SectionMember>();
    public DbSet<Student> Students => Set<Student>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var degreeLevelConverter = new EnumToNumberConverter<DegreeLevel, int>();

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.Property(f => f.Id).ValueGeneratedNever();

            entity.Property(f => f.NameAr).IsRequired().HasMaxLength(200);
            entity.Property(f => f.NameEn).IsRequired().HasMaxLength(200);
            entity.Property(f => f.Code).IsRequired().HasMaxLength(50);
            entity.Property(f => f.IsActive).IsRequired();
            entity.Property(f => f.CreatedAt).IsRequired();

            entity.HasIndex(f => f.Code).IsUnique();
            entity.HasIndex(f => f.IsActive);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Id).ValueGeneratedNever();

            entity.Property(d => d.NameAr).IsRequired().HasMaxLength(200);
            entity.Property(d => d.NameEn).IsRequired().HasMaxLength(200);
            entity.Property(d => d.Code).IsRequired().HasMaxLength(50);
            entity.Property(d => d.IsActive).IsRequired();
            entity.Property(d => d.CreatedAt).IsRequired();

            entity.HasIndex(d => d.Code).IsUnique();
            entity.HasIndex(d => d.FacultyId);

            entity.HasOne(d => d.Faculty)
                .WithMany(f => f.Departments)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(d => d.Programs)
                .WithOne(p => p.Department)
                .HasForeignKey(p => p.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(d => d.Courses)
                .WithOne(c => c.Department)
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Program>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).ValueGeneratedNever();

            entity.Property(p => p.NameAr).IsRequired().HasMaxLength(200);
            entity.Property(p => p.NameEn).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Code).IsRequired().HasMaxLength(50);
            entity.Property(p => p.DegreeLevel)
                .IsRequired()
                .HasConversion(degreeLevelConverter);
            entity.Property(p => p.TotalCreditsRequired).IsRequired();
            entity.Property(p => p.IsActive).IsRequired();
            entity.Property(p => p.CreatedAt).IsRequired();

            entity.HasIndex(p => p.Code).IsUnique();
            entity.HasIndex(p => p.DepartmentId);

            entity.HasMany(p => p.StudyPlans)
                .WithOne(sp => sp.Program)
                .HasForeignKey(sp => sp.ProgramId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StudyPlan>(entity =>
        {
            entity.HasKey(sp => sp.Id);
            entity.Property(sp => sp.Id).ValueGeneratedNever();

            entity.Property(sp => sp.VersionCode).IsRequired().HasMaxLength(50);
            entity.Property(sp => sp.EffectiveFrom).IsRequired();
            entity.Property(sp => sp.MinimumCredits).IsRequired();
            entity.Property(sp => sp.IsActive).IsRequired();
            entity.Property(sp => sp.CreatedAt).IsRequired();

            entity.HasIndex(sp => sp.VersionCode).IsUnique();
            entity.HasIndex(sp => sp.ProgramId);

            entity.HasMany(sp => sp.StudyPlanCourses)
                .WithOne(spc => spc.StudyPlan)
                .HasForeignKey(spc => spc.StudyPlanId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StudyPlanCourse>(entity =>
        {
            entity.HasKey(spc => spc.Id);
            entity.Property(spc => spc.Id).ValueGeneratedNever();

            entity.Property(spc => spc.SemesterNumber).IsRequired();
            entity.Property(spc => spc.IsRequired).IsRequired();
            entity.Property(spc => spc.CreatedAt).IsRequired();

            entity.HasIndex(spc => new { spc.StudyPlanId, spc.CourseId }).IsUnique();

            entity.HasOne(spc => spc.Course)
                .WithMany(c => c.StudyPlanCourses)
                .HasForeignKey(spc => spc.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AcademicYear>(entity =>
        {
            entity.HasKey(ay => ay.Id);
            entity.Property(ay => ay.Id).ValueGeneratedNever();

            entity.Property(ay => ay.Label).IsRequired().HasMaxLength(50);
            entity.Property(ay => ay.StartDate).IsRequired();
            entity.Property(ay => ay.EndDate).IsRequired();
            entity.Property(ay => ay.IsActive).IsRequired();
            entity.Property(ay => ay.CreatedAt).IsRequired();

            entity.HasIndex(ay => ay.Label).IsUnique();

            entity.HasMany(ay => ay.Semesters)
                .WithOne(s => s.AcademicYear)
                .HasForeignKey(s => s.AcademicYearId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id).ValueGeneratedNever();

            entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
            entity.Property(s => s.Code).IsRequired().HasMaxLength(50);
            entity.Property(s => s.StartDate).IsRequired();
            entity.Property(s => s.EndDate).IsRequired();
            entity.Property(s => s.RegistrationStart).IsRequired();
            entity.Property(s => s.RegistrationEnd).IsRequired();
            entity.Property(s => s.IsActive).IsRequired();
            entity.Property(s => s.CreatedAt).IsRequired();

            entity.HasIndex(s => s.AcademicYearId);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).ValueGeneratedNever();

            entity.Property(c => c.CourseCode).IsRequired().HasMaxLength(50);
            entity.Property(c => c.NameAr).IsRequired().HasMaxLength(200);
            entity.Property(c => c.NameEn).IsRequired().HasMaxLength(200);
            entity.Property(c => c.CreditHours).IsRequired();
            entity.Property(c => c.Description).HasMaxLength(2000);
            entity.Property(c => c.IsActive).IsRequired();
            entity.Property(c => c.CreatedAt).IsRequired();

            entity.HasIndex(c => c.CourseCode).IsUnique();
            entity.HasIndex(c => c.DepartmentId);
        });

        modelBuilder.Entity<CoursePrerequisite>(entity =>
        {
            entity.HasKey(cp => cp.Id);
            entity.Property(cp => cp.Id).ValueGeneratedNever();

            entity.Property(cp => cp.IsActive).IsRequired();
            entity.Property(cp => cp.CreatedAt).IsRequired();

            entity.HasIndex(cp => new { cp.CourseId, cp.PrerequisiteCourseId });

            entity.HasOne(cp => cp.Course)
                .WithMany(c => c.Prerequisites)
                .HasForeignKey(cp => cp.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(cp => cp.PrerequisiteCourse)
                .WithMany(c => c.DependentCourses)
                .HasForeignKey(cp => cp.PrerequisiteCourseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CourseOffering>(entity =>
        {
            entity.HasKey(co => co.Id);
            entity.Property(co => co.Id).ValueGeneratedNever();

            entity.Property(co => co.PrimaryTeacherId).IsRequired();
            entity.Property(co => co.Capacity).IsRequired();
            entity.Property(co => co.RegistrationStart).IsRequired();
            entity.Property(co => co.RegistrationEnd).IsRequired();
            entity.Property(co => co.Status)
                .IsRequired()
                .HasConversion(new EnumToNumberConverter<OfferingStatus, int>());
            entity.Property(co => co.IsActive).IsRequired();
            entity.Property(co => co.CreatedAt).IsRequired();

            entity.HasIndex(co => co.CourseId);
            entity.HasIndex(co => co.SemesterId);

            entity.HasOne(co => co.Course)
                .WithMany()
                .HasForeignKey(co => co.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(co => co.Semester)
                .WithMany()
                .HasForeignKey(co => co.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(co => co.Sections)
                .WithOne(s => s.CourseOffering)
                .HasForeignKey(s => s.CourseOfferingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Section>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id).ValueGeneratedNever();

            entity.Property(s => s.SectionType)
                .IsRequired()
                .HasConversion(new EnumToNumberConverter<SectionType, int>());
            entity.Property(s => s.AssignedStaffId).IsRequired();
            entity.Property(s => s.Capacity).IsRequired();
            entity.Property(s => s.IsActive).IsRequired();
            entity.Property(s => s.CreatedAt).IsRequired();

            entity.HasIndex(s => s.CourseOfferingId);

            entity.HasMany(s => s.Members)
                .WithOne(sm => sm.Section)
                .HasForeignKey(sm => sm.SectionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SectionMember>(entity =>
        {
            entity.HasKey(sm => sm.Id);
            entity.Property(sm => sm.Id).ValueGeneratedNever();

            entity.Property(sm => sm.StudentId).IsRequired();
            entity.Property(sm => sm.EnrolledAt).IsRequired();
            entity.Property(sm => sm.CreatedAt).IsRequired();

            entity.HasIndex(sm => new { sm.SectionId, sm.StudentId }).IsUnique();
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id).ValueGeneratedNever();

            entity.Property(s => s.UserId).IsRequired();
            entity.Property(s => s.StudentNumber).IsRequired().HasMaxLength(50);
            entity.Property(s => s.Gpa).IsRequired();
            entity.Property(s => s.IsActive).IsRequired();
            entity.Property(s => s.CreatedAt).IsRequired();

            entity.HasIndex(s => s.UserId).IsUnique();
            entity.HasIndex(s => s.StudentNumber).IsUnique();
        });
    }
}
