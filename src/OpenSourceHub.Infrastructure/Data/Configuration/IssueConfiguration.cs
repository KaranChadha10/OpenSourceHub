using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OpenSourceHub.Infrastructure.Data.Configuration;

public class IssueConfiguration : IEntityTypeConfiguration<Issue>
{
    public void Configure(EntityTypeBuilder<Issue> builder)
    {
        builder.ToTable("issues");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.GitHubId)
            .IsRequired();

        builder.Property(i => i.Number)
            .IsRequired();

        builder.Property(i => i.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(i => i.Body)
            .HasMaxLength(10000);

        builder.Property(i => i.State)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(i => i.AuthorUsername)
            .HasMaxLength(100);

        // Store Labels as JSON
        builder.Property(i => i.Labels)
            .HasColumnType("jsonb");

        // Indexes
        builder.HasIndex(i => i.GitHubId).IsUnique();
        builder.HasIndex(i => i.RepositoryId);
        builder.HasIndex(i => i.State);
        builder.HasIndex(i => new { i.RepositoryId, i.Number }); // Composite

        // GIN index for Labels (PostgreSQL specific - for efficient array searches)
        builder.HasIndex(i => i.Labels)
            .HasMethod("gin");

        // Relationship
        builder.HasOne(i => i.Repository)
            .WithMany(r => r.Issues)
            .HasForeignKey(i => i.RepositoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
