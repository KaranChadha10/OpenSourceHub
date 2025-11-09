using Microsoft.EntityFrameworkCore;
using OpenSourceHub.Domain.Entities;

namespace OpenSourceHub.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Repository> Repositories { get; }
    DbSet<Issue> Issues { get; }
    DbSet<Contribution> Contributions { get; }
    DbSet<Bookmark> Bookmarks { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<SavedSearch> SavedSearches { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}