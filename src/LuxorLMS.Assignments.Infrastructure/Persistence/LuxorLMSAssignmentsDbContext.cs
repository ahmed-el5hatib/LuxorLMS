using LuxorLMS.Assignments.Domain.Entities;
using LuxorLMS.Assignments.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LuxorLMS.Assignments.Infrastructure.Persistence;

public class LuxorLMSAssignmentsDbContext : DbContext
{
    public LuxorLMSAssignmentsDbContext(DbContextOptions<LuxorLMSAssignmentsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<AssignmentRubric> AssignmentRubrics => Set<AssignmentRubric>();
    public DbSet<AssignmentSubmission> AssignmentSubmissions => Set<AssignmentSubmission>();
    public DbSet<AssignmentFile> AssignmentFiles => Set<AssignmentFile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var assignmentStatusConverter = new EnumToNumberConverter<AssignmentStatus, int>();
        var submissionStatusConverter = new EnumToNumberConverter<SubmissionStatus, int>();

        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedNever();

            entity.Property(a => a.CourseOfferingId).IsRequired();
            entity.Property(a => a.Title).IsRequired().HasMaxLength(200);
            entity.Property(a => a.Description).HasMaxLength(4000);
            entity.Property(a => a.DueDate).IsRequired();
            entity.Property(a => a.MaxScore).IsRequired().HasColumnType("numeric(7,2)");
            entity.Property(a => a.AllowLateSubmission).IsRequired();
            entity.Property(a => a.Status)
                .IsRequired()
                .HasConversion(assignmentStatusConverter);
            entity.Property(a => a.IsActive).IsRequired();
            entity.Property(a => a.CreatedAt).IsRequired();
            entity.Property(a => a.CreatedBy).IsRequired();

            entity.HasIndex(a => a.CourseOfferingId);
            entity.HasIndex(a => a.Status);
        });

        modelBuilder.Entity<AssignmentRubric>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Id).ValueGeneratedNever();

            entity.Property(r => r.AssignmentId).IsRequired();
            entity.Property(r => r.Criteria).IsRequired().HasMaxLength(200);
            entity.Property(r => r.MaxPoints).IsRequired().HasColumnType("numeric(7,2)");
            entity.Property(r => r.Description).HasMaxLength(2000);
            entity.Property(r => r.DisplayOrder).IsRequired();
            entity.Property(r => r.IsActive).IsRequired();
            entity.Property(r => r.CreatedAt).IsRequired();
            entity.Property(r => r.CreatedBy).IsRequired();

            entity.HasIndex(r => r.AssignmentId);
        });

        modelBuilder.Entity<AssignmentSubmission>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id).ValueGeneratedNever();

            entity.Property(s => s.AssignmentId).IsRequired();
            entity.Property(s => s.StudentId).IsRequired();
            entity.Property(s => s.SubmittedAt).IsRequired();
            entity.Property(s => s.Score).HasColumnType("numeric(7,2)");
            entity.Property(s => s.Feedback).HasMaxLength(4000);
            entity.Property(s => s.Status)
                .IsRequired()
                .HasConversion(submissionStatusConverter);
            entity.Property(s => s.PlagiarismReportUrl).HasMaxLength(2000);
            entity.Property(s => s.PlagiarismScore).HasColumnType("numeric(5,2)");
            entity.Property(s => s.IsActive).IsRequired();
            entity.Property(s => s.CreatedAt).IsRequired();
            entity.Property(s => s.CreatedBy).IsRequired();

            entity.HasIndex(s => s.AssignmentId);
            entity.HasIndex(s => s.StudentId);
            entity.HasIndex(s => new { s.AssignmentId, s.StudentId }).IsUnique();
        });

        modelBuilder.Entity<AssignmentFile>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.Property(f => f.Id).ValueGeneratedNever();

            entity.Property(f => f.AssignmentSubmissionId).IsRequired();
            entity.Property(f => f.FileName).IsRequired().HasMaxLength(255);
            entity.Property(f => f.FileUrl).IsRequired().HasMaxLength(2000);
            entity.Property(f => f.ContentType).HasMaxLength(200);
            entity.Property(f => f.FileSizeBytes).IsRequired();
            entity.Property(f => f.Version).IsRequired();
            entity.Property(f => f.UploadedAt).IsRequired();
            entity.Property(f => f.IsActive).IsRequired();
            entity.Property(f => f.CreatedAt).IsRequired();
            entity.Property(f => f.CreatedBy).IsRequired();

            entity.HasIndex(f => f.AssignmentSubmissionId);
        });
    }
}
