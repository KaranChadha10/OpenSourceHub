using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenSourceHub.Domain.Entities;

namespace OpenSourceHub.Infrastructure.Data.Configuration;

public class BookmarkConfiguration : IEntityTypeConfiguration<Bookmark>
{
    public void Configure(EntityTypeBuilder<Bookmark> builder)
    {
        builder.ToTable("bookmarks");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Notes)
            .HasMaxLength(1000);

        // Store Tags as JSON
        builder.Property(b => b.Tags)
            .HasColumnType("jsonb");

        // Indexes
        builder.HasIndex(b => b.UserId);
        builder.HasIndex(b => b.RepositoryId);
        builder.HasIndex(b => new { b.UserId, b.RepositoryId }).IsUnique(); // User can bookmark a repo only once

        // Relationships
        builder.HasOne(b => b.User)
            .WithMany(u => u.Bookmarks)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Repository)
            .WithMany(r => r.Bookmarks)
            .HasForeignKey(b => b.RepositoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
