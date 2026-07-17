using LuxorLMS.Quizzes.Domain.Entities;
using LuxorLMS.Quizzes.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LuxorLMS.Quizzes.Infrastructure.Persistence;

public class LuxorLMSQuizzesDbContext : DbContext
{
    public LuxorLMSQuizzesDbContext(DbContextOptions<LuxorLMSQuizzesDbContext> options)
        : base(options)
    {
    }

    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<QuizQuestion> QuizQuestions => Set<QuizQuestion>();
    public DbSet<QuizOption> QuizOptions => Set<QuizOption>();
    public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();
    public DbSet<QuizAnswer> QuizAnswers => Set<QuizAnswer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var questionTypeConverter = new EnumToNumberConverter<QuestionType, int>();
        var attemptStatusConverter = new EnumToNumberConverter<QuizAttemptStatus, int>();

        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.HasKey(q => q.Id);
            entity.Property(q => q.Id).ValueGeneratedNever();

            entity.Property(q => q.CourseOfferingId).IsRequired();
            entity.Property(q => q.Title).IsRequired().HasMaxLength(200);
            entity.Property(q => q.Description).HasMaxLength(2000);
            entity.Property(q => q.TimeLimitMinutes).IsRequired();
            entity.Property(q => q.IsPublished).IsRequired();
            entity.Property(q => q.IsActive).IsRequired();
            entity.Property(q => q.CreatedAt).IsRequired();

            entity.HasIndex(q => q.CourseOfferingId);
        });

        modelBuilder.Entity<QuizQuestion>(entity =>
        {
            entity.HasKey(q => q.Id);
            entity.Property(q => q.Id).ValueGeneratedNever();

            entity.Property(q => q.QuizId).IsRequired();
            entity.Property(q => q.QuestionType)
                .IsRequired()
                .HasConversion(questionTypeConverter);
            entity.Property(q => q.Text).IsRequired().HasMaxLength(4000);
            entity.Property(q => q.Points).IsRequired().HasColumnType("numeric(7,2)");
            entity.Property(q => q.DisplayOrder).IsRequired();
            entity.Property(q => q.IsActive).IsRequired();
            entity.Property(q => q.CreatedAt).IsRequired();

            entity.HasIndex(q => q.QuizId);
            entity.HasIndex(q => new { q.QuizId, q.DisplayOrder });
        });

        modelBuilder.Entity<QuizOption>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Id).ValueGeneratedNever();

            entity.Property(o => o.QuizQuestionId).IsRequired();
            entity.Property(o => o.Text).IsRequired().HasMaxLength(2000);
            entity.Property(o => o.IsCorrect).IsRequired();
            entity.Property(o => o.DisplayOrder).IsRequired();
            entity.Property(o => o.IsActive).IsRequired();
            entity.Property(o => o.CreatedAt).IsRequired();

            entity.HasIndex(o => o.QuizQuestionId);
            entity.HasIndex(o => new { o.QuizQuestionId, o.DisplayOrder });
        });

        modelBuilder.Entity<QuizAttempt>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedNever();

            entity.Property(a => a.QuizId).IsRequired();
            entity.Property(a => a.StudentId).IsRequired();
            entity.Property(a => a.StartedAt).IsRequired();
            entity.Property(a => a.Score).HasColumnType("numeric(7,2)");
            entity.Property(a => a.Status)
                .IsRequired()
                .HasConversion(attemptStatusConverter);
            entity.Property(a => a.IsActive).IsRequired();
            entity.Property(a => a.CreatedAt).IsRequired();

            entity.HasIndex(a => a.QuizId);
            entity.HasIndex(a => a.StudentId);
            entity.HasIndex(a => new { a.QuizId, a.StudentId });
        });

        modelBuilder.Entity<QuizAnswer>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedNever();

            entity.Property(a => a.QuizAttemptId).IsRequired();
            entity.Property(a => a.QuizQuestionId).IsRequired();
            entity.Property(a => a.TextAnswer).HasMaxLength(8000);
            entity.Property(a => a.IsCorrect).IsRequired();
            entity.Property(a => a.IsActive).IsRequired();
            entity.Property(a => a.CreatedAt).IsRequired();

            entity.HasIndex(a => a.QuizAttemptId);
            entity.HasIndex(a => new { a.QuizAttemptId, a.QuizQuestionId }).IsUnique();
        });
    }
}
