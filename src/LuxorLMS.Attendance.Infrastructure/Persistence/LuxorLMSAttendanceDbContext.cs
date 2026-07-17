using LuxorLMS.Attendance.Domain.Entities;
using LuxorLMS.Attendance.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LuxorLMS.Attendance.Infrastructure.Persistence;

public class LuxorLMSAttendanceDbContext : DbContext
{
    public LuxorLMSAttendanceDbContext(DbContextOptions<LuxorLMSAttendanceDbContext> options)
        : base(options)
    {
    }

    public DbSet<AttendanceSession> AttendanceSessions => Set<AttendanceSession>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var sessionTypeConverter = new EnumToNumberConverter<AttendanceSessionType, int>();
        var statusConverter = new EnumToNumberConverter<AttendanceStatus, int>();

        modelBuilder.Entity<AttendanceSession>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id).ValueGeneratedNever();

            entity.Property(s => s.CourseOfferingId).IsRequired();
            entity.Property(s => s.SessionDate).IsRequired();
            entity.Property(s => s.SessionType)
                .IsRequired()
                .HasConversion(sessionTypeConverter);
            entity.Property(s => s.TokenHash).IsRequired().HasMaxLength(128);
            entity.Property(s => s.ExpiresAt).IsRequired();
            entity.Property(s => s.IsActive).IsRequired();
            entity.Property(s => s.CreatedAt).IsRequired();
            entity.Property(s => s.CreatedBy).IsRequired();

            entity.HasIndex(s => s.CourseOfferingId);
            entity.HasIndex(s => s.SectionId);
            entity.HasIndex(s => s.ExpiresAt);
        });

        modelBuilder.Entity<AttendanceRecord>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Id).ValueGeneratedNever();

            entity.Property(r => r.AttendanceSessionId).IsRequired();
            entity.Property(r => r.StudentId).IsRequired();
            entity.Property(r => r.Status)
                .IsRequired()
                .HasConversion(statusConverter);
            entity.Property(r => r.Notes).HasMaxLength(2000);
            entity.Property(r => r.IsActive).IsRequired();
            entity.Property(r => r.CreatedAt).IsRequired();
            entity.Property(r => r.CreatedBy).IsRequired();

            entity.HasIndex(r => new { r.AttendanceSessionId, r.StudentId }).IsUnique();
            entity.HasIndex(r => r.StudentId);
            entity.HasIndex(r => r.AttendanceSessionId);
        });
    }
}
