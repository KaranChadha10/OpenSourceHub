using Microsoft.EntityFrameworkCore;
using OpenSourceHub.Application.Common.Interfaces;
using OpenSourceHub.Domain.Common;
using OpenSourceHub.Domain.Entities;

namespace OpenSourceHub.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Repository> Repositories => Set<Repository>();
    public DbSet<Issue> Issues => Set<Issue>();
    public DbSet<Contribution> Contributions => Set<Contribution>();
    public DbSet<Bookmark> Bookmarks => Set<Bookmark>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<SavedSearch> SavedSearches => Set<SavedSearch>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdateTimeStamp();
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}