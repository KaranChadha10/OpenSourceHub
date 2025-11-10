using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenSourceHub.Application.Common.Interfaces;
using OpenSourceHub.Domain.Enum;

public class GetContributionStatsQueryHandler : IRequestHandler<GetContributionStatsQuery, ContributionStatsDTO>
{
    private readonly IApplicationDbContext _context;

    public GetContributionStatsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ContributionStatsDTO> Handle(GetContributionStatsQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            throw new Exception("User not found");
        }

        return new ContributionStatsDTO
        {
            TotalContributions = user.TotalContributions,
            MergedContributions = user.MergedContributions,
            OpenContributions = await _context.Contributions
                .CountAsync(c => c.UserId == request.UserId && c.Status == ContributionStatus.Open, cancellationToken),
            CurrentStreak = user.CurrentStreak,
            LongestStreak = user.LongestStreak,
            LastContributionDate = user.LastContributionDate,
            LastSyncedAt = user.LastSyncedAt
        };
    }
}