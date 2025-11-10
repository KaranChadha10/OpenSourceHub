using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenSourceHub.Application.Common.Interfaces;

public class GetUserContributionsQueryHandler : IRequestHandler<GetUserContributionQuery, List<ContributionDTO>>
{
    private readonly IApplicationDbContext _context;

    public GetUserContributionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ContributionDTO>> Handle(
    GetUserContributionQuery request,
    CancellationToken cancellationToken)
    {
        var contributions = await _context.Contributions
            .Include(c => c.Repository)
            .Where(c => c.UserId == request.UserId)
            .OrderByDescending(c => c.GitHubCreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new ContributionDTO
            {
                Id = c.Id,
                PullRequestId = c.PullRequestId,
                PullRequestNumber = c.PullRequestNumber,
                Title = c.Title,
                Description = c.Description,
                Status = c.Status,
                FilesChanged = c.FilesChanged,
                Additions = c.Additions,
                Deletions = c.Deletions,
                CommentsCount = c.CommentsCount,
                RepositoryFullName = c.Repository.FullName,
                RepositoryUrl = $"https://github.com/{c.Repository.FullName}",
                PrUrl = $"https://github.com/{c.Repository.FullName}/pull/{c.PullRequestNumber}",
                GitHubCreatedAt = c.GitHubCreatedAt,
                GitHubMergedAt = c.GitHubMergedAt,
                GitHubClosedAt = c.GitHubClosedAt,
                LastSyncedAt = c.LastSyncedAt,
                AiSummary = c.AiSummary
            })
            .ToListAsync(cancellationToken);

        return contributions;
    }
}