using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OpenSourceHub.Infrastructure.Data.Configuration;

public class ContributionConfiguration : IEntityTypeConfiguration<Contribution>
{
    public void Configure(EntityTypeBuilder<Contribution> builder)
    {
        builder.ToTable("contributions");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.PullRequestId)
            .IsRequired();

        builder.Property(c => c.PullRequestNumber)
            .IsRequired();

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.Description)
            .HasMaxLength(5000);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>(); // Store enum as string in DB

        builder.Property(c => c.AiSummary)
            .HasMaxLength(2000);

        // Indexes
        builder.HasIndex(c => c.PullRequestId).IsUnique();
        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => c.RepositoryId);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => new { c.UserId, c.Status }); // For user's dashboard

        // Relationships
        builder.HasOne(c => c.User)
            .WithMany(u => u.Contributions)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Repository)
            .WithMany(r => r.Contributions)
            .HasForeignKey(c => c.RepositoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
