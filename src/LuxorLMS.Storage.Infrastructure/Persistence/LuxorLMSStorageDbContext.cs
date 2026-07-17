using LuxorLMS.Storage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LuxorLMS.Storage.Infrastructure.Persistence;

public class LuxorLMSStorageDbContext : DbContext
{
    public LuxorLMSStorageDbContext(DbContextOptions<LuxorLMSStorageDbContext> options) : base(options)
    {
    }

    public DbSet<StoredFile> StoredFiles => Set<StoredFile>();
    public DbSet<FileVersion> FileVersions => Set<FileVersion>();
    public DbSet<StorageProviderConfig> StorageProviderConfigs => Set<StorageProviderConfig>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StoredFile>(builder =>
        {
            builder.ToTable("StoredFiles", "storage");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FileName).HasMaxLength(255).IsRequired();
            builder.Property(x => x.ContentType).HasMaxLength(128).IsRequired();
            builder.Property(x => x.Container).HasMaxLength(128).IsRequired();
            builder.Property(x => x.ObjectKey).HasMaxLength(512).IsRequired();
            builder.Property(x => x.ContentHash).HasMaxLength(128);

            builder.HasMany(x => x.Versions)
                .WithOne(v => v.StoredFile)
                .HasForeignKey(v => v.StoredFileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FileVersion>(builder =>
        {
            builder.ToTable("FileVersions", "storage");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ObjectKey).HasMaxLength(512).IsRequired();
            builder.Property(x => x.ContentHash).HasMaxLength(128);
        });

        modelBuilder.Entity<StorageProviderConfig>(builder =>
        {
            builder.ToTable("StorageProviderConfigs", "storage");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.BucketOrContainer).HasMaxLength(128).IsRequired();
            builder.Property(x => x.Region).HasMaxLength(64);
            builder.Property(x => x.Endpoint).HasMaxLength(255);
        });
    }
}
