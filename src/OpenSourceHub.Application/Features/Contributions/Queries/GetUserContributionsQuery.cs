using MediatR;

public record GetUserContributionQuery(Guid UserId, int Page = 1, int PageSize = 20) : IRequest<List<ContributionDTO>>;