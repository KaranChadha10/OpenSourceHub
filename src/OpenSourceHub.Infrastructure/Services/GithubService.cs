using Microsoft.Extensions.Options;
using Octokit;
using OpenSourceHub.Application.Common.Interfaces;

public class GitHubService : IGitHubService
{
    private readonly GithubOptions _options;

    public GitHubService(IOptions<GithubOptions> options)
    {
        _options = options.Value;
    }

    public async Task<string> GetAccessTokenAsync(string code)
    {
        var request = new OauthTokenRequest(_options.ClientId, _options.ClientSecret, code);

        var client = new GitHubClient(new ProductHeaderValue("OpenSourceHub"));
        var token = await client.Oauth.CreateAccessToken(request);

        return token.AccessToken;
    }

    public async Task<GitHubUserDto> GetUserAsync(string accessToken)
    {
        var client = new GitHubClient(new ProductHeaderValue("OpenSourceHub"))
        {
            Credentials = new Credentials(accessToken)
        };

        var user = await client.User.Current();

        // Get primary email if not public
        var email = user.Email;
        if (string.IsNullOrEmpty(email))
        {
            var emails = await client.User.Email.GetAll();
            email = emails.FirstOrDefault(e => e.Primary)?.Email ?? emails.FirstOrDefault()?.Email ?? string.Empty;
        }

        return new GitHubUserDto
        {
            GitHubId = user.Id.ToString(),
            Login = user.Login,
            Email = email,
            Name = user.Name,
            AvatarUrl = user.AvatarUrl,
            Bio = user.Bio,
            Location = user.Location,
            Company = user.Company
        };
    }
}