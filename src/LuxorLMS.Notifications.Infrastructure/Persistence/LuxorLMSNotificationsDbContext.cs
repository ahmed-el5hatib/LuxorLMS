using LuxorLMS.Notifications.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Notifications.Infrastructure.Persistence;

public class LuxorLMSNotificationsDbContext : DbContext
{
    public LuxorLMSNotificationsDbContext(DbContextOptions<LuxorLMSNotificationsDbContext> options) : base(options)
    {
    }

    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public DbSet<NotificationMessage> NotificationMessages => Set<NotificationMessage>();
    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NotificationTemplate>(builder =>
        {
            builder.ToTable("NotificationTemplates", "notifications");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Code).HasMaxLength(128).IsRequired();
            builder.Property(x => x.Subject).HasMaxLength(255).IsRequired();
            builder.Property(x => x.BodyTemplate).IsRequired();
            builder.Property(x => x.Culture).HasMaxLength(16).IsRequired();
        });

        modelBuilder.Entity<NotificationMessage>(builder =>
        {
            builder.ToTable("NotificationMessages", "notifications");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).HasMaxLength(255).IsRequired();
            builder.Property(x => x.Body).IsRequired();
            builder.Property(x => x.Error).HasMaxLength(512);

            builder.HasOne(x => x.Template)
                .WithMany()
                .HasForeignKey(x => x.TemplateId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<NotificationPreference>(builder =>
        {
            builder.ToTable("NotificationPreferences", "notifications");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new { x.UserId, x.Channel }).IsUnique();
        });
    }
}
