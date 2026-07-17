using LuxorLMS.Forums.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Forums.Infrastructure.Persistence;

public class LuxorLMSForumsDbContext : DbContext
{
    public LuxorLMSForumsDbContext(DbContextOptions<LuxorLMSForumsDbContext> options) : base(options)
    {
    }

    public DbSet<ForumTopic> ForumTopics => Set<ForumTopic>();
    public DbSet<ForumPost> ForumPosts => Set<ForumPost>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ForumTopic>(builder =>
        {
            builder.ToTable("ForumTopics", "forums");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).HasMaxLength(255).IsRequired();
            builder.HasIndex(x => new { x.CourseOfferingId, x.CreatedAt });
            builder.HasIndex(x => x.IsDeleted);
            builder.HasIndex(x => new { x.CourseOfferingId, x.IsPinned, x.CreatedAt });

            builder.HasMany(x => x.Posts)
                .WithOne(p => p.Topic)
                .HasForeignKey(p => p.TopicId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ForumPost>(builder =>
        {
            builder.ToTable("ForumPosts", "forums");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Body).IsRequired();
            builder.HasIndex(x => new { x.TopicId, x.CreatedAt });
            builder.HasIndex(x => x.ModerationStatus);
            builder.HasIndex(x => x.IsDeleted);

            builder.HasOne(x => x.ParentPost)
                .WithMany(p => p.Replies)
                .HasForeignKey(x => x.ParentPostId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
