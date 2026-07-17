using LuxorLMS.Identity.Domain.Entities;
using LuxorLMS.Identity.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LuxorLMS.Identity.Infrastructure.Persistence;

public class LuxorLMSIdentityDbContext : DbContext
{
    public LuxorLMSIdentityDbContext(DbContextOptions<LuxorLMSIdentityDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<DeviceSession> DeviceSessions => Set<DeviceSession>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var userRoleConverter = new EnumToNumberConverter<UserRole, int>();

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).ValueGeneratedNever();

            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Username).IsUnique();

            entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.PasswordSalt).IsRequired();
            entity.Property(u => u.FirstNameAr).HasMaxLength(100);
            entity.Property(u => u.LastNameAr).HasMaxLength(100);
            entity.Property(u => u.FirstNameEn).HasMaxLength(100);
            entity.Property(u => u.LastNameEn).HasMaxLength(100);
            entity.Property(u => u.MfaSecret).HasMaxLength(256);
            entity.Property(u => u.Role)
                .IsRequired()
                .HasConversion(userRoleConverter)
                .HasMaxLength(50);

            entity.Property(u => u.CreatedAt).IsRequired();
            entity.Property(u => u.UpdatedAt).IsRequired();

            entity.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.DeviceSessions)
                .WithOne(ds => ds.User)
                .HasForeignKey(ds => ds.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.AuditLogs)
                .WithOne(al => al.User)
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);
            entity.Property(rt => rt.Id).ValueGeneratedNever();

            entity.Property(rt => rt.Token).IsRequired();
            entity.Property(rt => rt.ExpiresAt).IsRequired();
            entity.Property(rt => rt.CreatedAt).IsRequired();

            entity.HasIndex(rt => rt.Token).IsUnique();
        });

        modelBuilder.Entity<DeviceSession>(entity =>
        {
            entity.HasKey(ds => ds.Id);
            entity.Property(ds => ds.Id).ValueGeneratedNever();

            entity.Property(ds => ds.DeviceName).IsRequired().HasMaxLength(256);
            entity.Property(ds => ds.UserAgent).IsRequired().HasMaxLength(512);
            entity.Property(ds => ds.IpAddress).IsRequired().HasMaxLength(64);
            entity.Property(ds => ds.LastActiveAt).IsRequired();
            entity.Property(ds => ds.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(al => al.Id);
            entity.Property(al => al.Id).ValueGeneratedNever();

            entity.Property(al => al.UserId).IsRequired();
            entity.Property(al => al.Action).IsRequired().HasMaxLength(256);
            entity.Property(al => al.EntityName).IsRequired().HasMaxLength(256);
            entity.Property(al => al.EntityId).IsRequired().HasMaxLength(100);
            entity.Property(al => al.OldValues).HasMaxLength(4000);
            entity.Property(al => al.NewValues).HasMaxLength(4000);
            entity.Property(al => al.Timestamp).IsRequired();
            entity.Property(al => al.IpAddress).IsRequired().HasMaxLength(64);
            entity.Property(al => al.UserAgent).IsRequired().HasMaxLength(512);

            entity.HasIndex(al => new { al.UserId, al.Timestamp });
            entity.HasIndex(al => new { al.EntityName, al.EntityId });
            entity.HasIndex(al => al.Timestamp);
        });
    }
}
