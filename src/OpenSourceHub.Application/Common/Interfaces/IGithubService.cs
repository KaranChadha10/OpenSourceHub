namespace OpenSourceHub.Application.Common.Interfaces;

public interface IGitHubService
{
    Task<GitHubUserDto> GetUserAsync(string accessToken);
    Task<string> GetAccessTokenAsync(string code);
}