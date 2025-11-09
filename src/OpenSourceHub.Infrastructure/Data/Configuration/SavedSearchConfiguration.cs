using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenSourceHub.Domain.Entities;

namespace OpenSourceHub.Infrastructure.Data.Configuration;

public class SavedSearchConfiguration : IEntityTypeConfiguration<SavedSearch>
{
    public void Configure(EntityTypeBuilder<SavedSearch> builder)
    {
        builder.ToTable("saved_searches");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Language)
            .HasMaxLength(50);

        // Store Labels as JSON
        builder.Property(s => s.Labels)
            .HasColumnType("jsonb");

        // Indexes
        builder.HasIndex(s => s.UserId);

        // Relationship
        builder.HasOne(s => s.User)
            .WithMany(u => u.SavedSearches)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
