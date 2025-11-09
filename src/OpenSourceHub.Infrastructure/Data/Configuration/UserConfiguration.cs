using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenSourceHub.Domain.Entities;

namespace OpenSourceHub.Infrastructure.Data.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.GitHubId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.AvatarUrl)
            .HasMaxLength(500);

        builder.Property(u => u.Bio)
            .HasMaxLength(500);

        builder.Property(u => u.Location)
            .HasMaxLength(100);

        builder.Property(u => u.Company)
            .HasMaxLength(100);

        builder.Property(u => u.AccessToken)
            .IsRequired()
            .HasMaxLength(500);

        // Owned entity for UserPreferences (stored as JSON)
        builder.OwnsOne(u => u.Preferences, prefs =>
        {
            prefs.ToJson();
            prefs.Property(p => p.PreferredLanguages).IsRequired();
            prefs.Property(p => p.PreferredLabels).IsRequired();
            prefs.Property(p => p.MinimumStars).IsRequired();
            prefs.Property(p => p.EmailNotifications).IsRequired();
            prefs.Property(p => p.InAppNotifications).IsRequired();
            prefs.Property(p => p.TimeZone).HasMaxLength(50);
        });

        // Indexes
        builder.HasIndex(u => u.GitHubId).IsUnique();
        builder.HasIndex(u => u.Username);
        builder.HasIndex(u => u.Email);

        // Relationships
        builder.HasMany(u => u.Bookmarks)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Contributions)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Notifications)
            .WithOne(n => n.User)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.SavedSearches)
            .WithOne(s => s.User)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
