using MediatR;

public record SearchIssueQuery(IssueSearchFilters Filters) : IRequest<IssueSearchResultDto>;