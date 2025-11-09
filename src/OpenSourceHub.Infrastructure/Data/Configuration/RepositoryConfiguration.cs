using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OpenSourceHub.Infrastructure.Data.Configuration;

public class RepositoryConfiguration : IEntityTypeConfiguration<Repository>
{
    public void Configure(EntityTypeBuilder<Repository> builder)
    {
        builder.ToTable("repositories");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.GitHubId)
            .IsRequired();

        builder.Property(r => r.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Owner)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Description)
            .HasMaxLength(1000);

        builder.Property(r => r.Language)
            .HasMaxLength(50);

        builder.Property(r => r.Homepage)
            .HasMaxLength(500);

        builder.Property(r => r.DefaultBranch)
            .HasMaxLength(100);

        // Store Topics as JSON
        builder.Property(r => r.Topics)
            .HasColumnType("jsonb");

        // Indexes
        builder.HasIndex(r => r.GitHubId).IsUnique();
        builder.HasIndex(r => r.FullName);
        builder.HasIndex(r => r.Language);
        builder.HasIndex(r => r.StarsCount);
        builder.HasIndex(r => new { r.Language, r.StarsCount }); // Composite index for filtering

        // Relationships
        builder.HasMany(r => r.Issues)
            .WithOne(i => i.Repository)
            .HasForeignKey(i => i.RepositoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Contributions)
            .WithOne(c => c.Repository)
            .HasForeignKey(c => c.RepositoryId)
            .OnDelete(DeleteBehavior.Restrict); // Don't delete contributions if repo deleted

        builder.HasMany(r => r.Bookmarks)
            .WithOne(b => b.Repository)
            .HasForeignKey(b => b.RepositoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
