using MediatR;

public record GetContributionStatsQuery(Guid UserId) : IRequest<ContributionStatsDTO>;
