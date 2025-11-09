using MediatR;
using OpenSourceHub.Application.Common.Interfaces;

public class SearchIssueQueryHandler : IRequestHandler<SearchIssueQuery, IssueSearchResultDto>
{
    private readonly IGitHubService _githubService;
    public SearchIssueQueryHandler(IGitHubService gitHubService)
    {
        _githubService = gitHubService;
    }

    public async Task<IssueSearchResultDto> Handle(SearchIssueQuery request, CancellationToken cancellationToken)
    {
        return await _githubService.SearchIssuesAsync(request.Filters, cancellationToken);
    }
}