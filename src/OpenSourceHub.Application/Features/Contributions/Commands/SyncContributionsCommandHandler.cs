using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenSourceHub.Application.Common.Interfaces;

namespace OpenSourceHub.Application.Features.Contributions.Commands;

public class SyncContributionsCommandHandler : IRequestHandler<SyncContributionsCommand, SyncContributionsResultDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IGitHubService _gitHubService;

    public SyncContributionsCommandHandler(
        IApplicationDbContext context,
        IGitHubService gitHubService)
    {
        _context = context;
        _gitHubService = gitHubService;
    }

    public async Task<SyncContributionsResultDto> Handle(
        SyncContributionsCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Contributions)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            throw new Exception("User not found");
        }

        var prDtos = await _gitHubService.FetchUserPullRequestsAsync(
            user.Username,
            user.AccessToken,
            cancellationToken);

        int newCount = 0;
        int updatedCount = 0;

        foreach (var prDto in prDtos)
        {
            var existing = await _context.Contributions
                .FirstOrDefaultAsync(c => c.PullRequestId == prDto.PullRequestId, cancellationToken);

            if (existing == null)
            {
                var repository = await _context.Repositories
                    .FirstOrDefaultAsync(r => r.FullName == prDto.RepositoryFullName, cancellationToken);

                if (repository == null) continue;

                var contribution = Contribution.Create(
                    user.Id,
                    repository.Id,
                    prDto.PullRequestId,
                    prDto.PullRequestNumber,
                    prDto.Title,
                    prDto.GitHubCreatedAt
                );

                contribution.UpdateDetails(
                    prDto.Title,
                    prDto.Description,
                    prDto.Status,
                    prDto.FilesChanged,
                    prDto.Additions,
                    prDto.Deletions,
                    prDto.CommentsCount,
                    prDto.GitHubMergedAt,
                    prDto.GitHubClosedAt
                );

                _context.Contributions.Add(contribution);
                newCount++;
            }
            else
            {
                existing.UpdateDetails(
                    prDto.Title,
                    prDto.Description,
                    prDto.Status,
                    prDto.FilesChanged,
                    prDto.Additions,
                    prDto.Deletions,
                    prDto.CommentsCount,
                    prDto.GitHubMergedAt,
                    prDto.GitHubClosedAt
                );
                updatedCount++;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        var stats = await CalculateStatsAsync(user.Id, cancellationToken);

        user.UpdateStats(
            stats.TotalContributions,
            stats.MergedContributions,
            stats.CurrentStreak,
            stats.LongestStreak,
            stats.LastContributionDate
        );

        user.UpdateLastSynced();

        await _context.SaveChangesAsync(cancellationToken);

        return new SyncContributionsResultDto
        {
            NewContributions = newCount,
            UpdatedContributions = updatedCount,
            TotalContributions = stats.TotalContributions,
            Stats = stats,
            SyncedAt = DateTime.UtcNow
        };
    }

    private async Task<ContributionStatsDTO> CalculateStatsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var contributions = await _context.Contributions
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.GitHubCreatedAt)
            .ToListAsync(cancellationToken);

        var total = contributions.Count;
        var merged = contributions.Count(c => c.IsMerged());
        var open = contributions.Count(c => c.IsOpen());
        var lastContribution = contributions.FirstOrDefault()?.GitHubCreatedAt;

        var (currentStreak, longestStreak) = CalculateStreaks(contributions);

        return new ContributionStatsDTO
        {
            TotalContributions = total,
            MergedContributions = merged,
            OpenContributions = open,
            CurrentStreak = currentStreak,
            LongestStreak = longestStreak,
            LastContributionDate = lastContribution,
            LastSyncedAt = DateTime.UtcNow
        };
    }

    private (int currentStreak, int longestStreak) CalculateStreaks(List<Contribution> contributions)
    {
        if (!contributions.Any()) return (0, 0);

        var contributionDates = contributions
            .Select(c => c.GitHubCreatedAt.Date)
            .Distinct()
            .OrderByDescending(d => d)
            .ToList();

        if (!contributionDates.Any()) return (0, 0);

        int currentStreak = 1;
        for (int i = 0; i < contributionDates.Count - 1; i++)
        {
            var dayDiff = (contributionDates[i] - contributionDates[i + 1]).Days;
            if (dayDiff == 1) currentStreak++;
            else break;
        }

        int longestStreak = 1;
        int tempStreak = 1;
        for (int i = 0; i < contributionDates.Count - 1; i++)
        {
            var dayDiff = (contributionDates[i] - contributionDates[i + 1]).Days;
            if (dayDiff == 1)
            {
                tempStreak++;
                longestStreak = Math.Max(longestStreak, tempStreak);
            }
            else tempStreak = 1;
        }

        return (currentStreak, longestStreak);
    }
}