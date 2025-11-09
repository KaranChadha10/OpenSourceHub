namespace OpenSourceHub.Application.Common.Interfaces;

public interface IGitHubService
{
    //Authentication
    Task<GitHubUserDto> GetUserAsync(string accessToken);
    Task<string> GetAccessTokenAsync(string code);

    //Issues
    Task<IssueSearchResultDto> SearchIssuesAsync(IssueSearchFilters filters, CancellationToken cancellationToken = default);
    Task<Guid> EnsureRepositoryExistsAsync(string fullName, CancellationToken cancellationToken = default);
}