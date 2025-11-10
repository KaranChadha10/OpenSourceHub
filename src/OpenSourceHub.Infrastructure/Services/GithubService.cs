using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Octokit;
using OpenSourceHub.Application.Common;
using OpenSourceHub.Application.Common.Interfaces;
using OpenSourceHub.Application.Features;
using OpenSourceHub.Application.Features.Auth;
using OpenSourceHub.Domain.Enum;

namespace OpenSourceHub.Infrastructure.Services;

public class GitHubService : IGitHubService
{
    private readonly GithubOptions _options;
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cacheService;

    public GitHubService(
        IOptions<GithubOptions> options,
        IApplicationDbContext context,
        ICacheService cacheService)
    {
        _options = options.Value;
        _context = context;
        _cacheService = cacheService;
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

    public async Task<IssueSearchResultDto> SearchIssuesAsync(
     IssueSearchFilters filters,
     CancellationToken cancellationToken = default)
    {
        // Generate cache key based on filters
        var cacheKey = GenerateCacheKey(filters);

        // Try to get from cache
        var cachedResult = await _cacheService.GetAsync<IssueSearchResultDto>(cacheKey, cancellationToken);
        if (cachedResult != null)
        {
            Console.WriteLine("⚡ Returning cached result");
            return cachedResult;
        }

        // If not in cache, fetch from GitHub
        var client = new GitHubClient(new ProductHeaderValue("OpenSourceHub"));

        // Build search terms FIRST
        var searchTerms = new List<string>();

        // Add language filter to search terms
        if (!string.IsNullOrEmpty(filters.Language))
        {
            searchTerms.Add($"language:{filters.Language}");
        }

        // Add labels filter to search terms
        foreach (var label in filters.Labels)
        {
            searchTerms.Add($"label:{label}");
        }

        // Add minimum stars filter to search terms
        if (filters.MinimumStars.HasValue && filters.MinimumStars.Value > 0)
        {
            searchTerms.Add($"stars:>={filters.MinimumStars.Value}");
        }

        // Add type qualifier (issue vs pull request)
        searchTerms.Add("type:issue");

        // Combine all search terms
        var searchTerm = string.Join(" ", searchTerms);
        Console.WriteLine($"🔍 Search term: {searchTerm}");

        // Create SearchIssuesRequest with the term in constructor
        var searchRequest = new SearchIssuesRequest(searchTerm)
        {
            Page = filters.Page,
            PerPage = filters.PageSize
        };

        // Handle state filter
        if (filters.State == "closed")
        {
            searchRequest.State = ItemState.Closed;
        }
        else if (filters.State == "open")
        {
            searchRequest.State = ItemState.Open;
        }

        // Search issues
        var searchResult = await client.Search.SearchIssues(searchRequest);

        Console.WriteLine($"📊 GitHub returned {searchResult.Items.Count} issues out of {searchResult.TotalCount} total");

        // Map to DTOs with null checks
        var issues = new List<IssueDto>();

        foreach (var issue in searchResult.Items)
        {
            try
            {
                Console.WriteLine($"\n--- Processing Issue #{issue.Number} ---");
                Console.WriteLine($"Issue URL: {issue.HtmlUrl}");

                // ✅ Extract repository info from issue URL
                string repoFullName = "";
                string repoUrl = "";

                if (!string.IsNullOrEmpty(issue.HtmlUrl))
                {
                    Console.WriteLine($"Parsing URL: {issue.HtmlUrl}");

                    // URL format: https://github.com/{owner}/{repo}/issues/{number}
                    var uri = new Uri(issue.HtmlUrl);
                    Console.WriteLine($"URI Path: {uri.AbsolutePath}");

                    var pathParts = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    Console.WriteLine($"Path parts count: {pathParts.Length}");

                    for (int i = 0; i < pathParts.Length; i++)
                    {
                        Console.WriteLine($"  [{i}] = '{pathParts[i]}'");
                    }

                    if (pathParts.Length >= 3 && pathParts[2] == "issues")
                    {
                        var owner = pathParts[0];
                        var repo = pathParts[1];
                        repoFullName = $"{owner}/{repo}";
                        repoUrl = $"https://github.com/{owner}/{repo}";

                        Console.WriteLine($"✅ Extracted: owner='{owner}', repo='{repo}'");
                        Console.WriteLine($"✅ repoFullName='{repoFullName}'");
                        Console.WriteLine($"✅ repoUrl='{repoUrl}'");
                    }
                    else
                    {
                        Console.WriteLine($"❌ Failed condition: pathParts.Length={pathParts.Length}, pathParts[2]={pathParts.ElementAtOrDefault(2)}");
                    }
                }
                else
                {
                    Console.WriteLine("❌ issue.HtmlUrl is null or empty");
                }

                // Skip if we couldn't determine the repository
                if (string.IsNullOrEmpty(repoFullName))
                {
                    Console.WriteLine($"⚠️ SKIPPING - repoFullName is empty");
                    continue;
                }

                Console.WriteLine($"🎯 Creating DTO with repoFullName='{repoFullName}', repoUrl='{repoUrl}'");

                var dto = new IssueDto
                {
                    GitHubId = issue.Id,
                    Number = issue.Number,
                    Title = issue.Title ?? string.Empty,
                    Body = issue.Body,
                    State = issue.State.StringValue ?? "open",
                    Labels = issue.Labels?.Select(l => l.Name).ToList() ?? new List<string>(),
                    RepositoryFullName = repoFullName,
                    RepositoryUrl = repoUrl,
                    RepositoryStars = issue.Repository?.StargazersCount ?? 0,
                    RepositoryLanguage = issue.Repository?.Language,
                    IssueUrl = issue.HtmlUrl ?? string.Empty,
                    AuthorUsername = issue.User?.Login ?? "unknown",
                    CommentsCount = issue.Comments,
                    CreatedAt = issue.CreatedAt.UtcDateTime,
                    UpdatedAt = issue.UpdatedAt?.UtcDateTime
                };

                Console.WriteLine($"✅ DTO created: RepositoryFullName='{dto.RepositoryFullName}', RepositoryUrl='{dto.RepositoryUrl}'");

                issues.Add(dto);

                // Store repository in database if it doesn't exist
                try
                {
                    await EnsureRepositoryExistsAsync(repoFullName, cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error storing repository {repoFullName}: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR processing issue #{issue.Number}: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        Console.WriteLine($"\n✅ Successfully processed {issues.Count} issues");

        var result = new IssueSearchResultDto
        {
            Issues = issues,
            TotalCount = searchResult.TotalCount,
            Page = filters.Page,
            PageSize = filters.PageSize,
            HasNextPage = searchResult.TotalCount > filters.Page * filters.PageSize,
            HasPreviousPage = filters.Page > 1
        };

        // Cache the result for 15 minutes
        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15), cancellationToken);

        return result;
    }
    public async Task<Guid> EnsureRepositoryExistsAsync(
               string fullName,
               CancellationToken cancellationToken = default)
    {
        // Check if repository exists in database
        var existingRepo = await _context.Repositories
            .FirstOrDefaultAsync(r => r.FullName == fullName, cancellationToken);

        if (existingRepo != null)
        {
            return existingRepo.Id;
        }

        // If not, fetch from GitHub and create
        var client = new GitHubClient(new ProductHeaderValue("OpenSourceHub"));
        var parts = fullName.Split('/');
        var owner = parts[0];
        var name = parts[1];

        var githubRepo = await client.Repository.Get(owner, name);

        // ✅ FIX 4: Use Domain.Entities.Repository.Create()
        var repository = Repository.Create(
            githubRepo.Id,
            githubRepo.FullName,
            githubRepo.Name,
            githubRepo.Owner.Login,
            githubRepo.Language
        );

        repository.UpdateMetadata(
            githubRepo.Description,
            githubRepo.StargazersCount,
            githubRepo.ForksCount,
            githubRepo.OpenIssuesCount,
            githubRepo.StargazersCount, // ✅ Fixed: was StargazersCount again
            githubRepo.Archived,
            githubRepo.Fork,
            githubRepo.DefaultBranch,
            githubRepo.CreatedAt.UtcDateTime,
            githubRepo.UpdatedAt.UtcDateTime,
            githubRepo.Topics?.ToList(),
            githubRepo.Homepage
        );

        _context.Repositories.Add(repository);
        await _context.SaveChangesAsync(cancellationToken);

        return repository.Id;
    }

    private string GenerateCacheKey(IssueSearchFilters filters)
    {
        var parts = new List<string>
        {
            "issues",
            filters.Language ?? "all",
            string.Join("-", filters.Labels.OrderBy(l => l)),
            filters.MinimumStars?.ToString() ?? "0",
            filters.State ?? "open",
            filters.Page.ToString(),
            filters.PageSize.ToString()
        };

        return string.Join(":", parts);
    }

    private (string owner, string repo) ParseRepositoryFromIssueUrl(string issueUrl)
    {
        // URL format: https://github.com/{owner}/{repo}/issues/{number}
        try
        {
            var uri = new Uri(issueUrl);
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length >= 2)
            {
                return (segments[0], segments[1]);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing issue URL {issueUrl}: {ex.Message}");
        }

        return (string.Empty, string.Empty);
    }

    public async Task<List<ContributionDTO>> FetchUserPullRequestsAsync(
     string username,
     string accessToken,
     CancellationToken cancellationToken = default)
    {
        var client = new GitHubClient(new ProductHeaderValue("OpenSourceHub"))
        {
            Credentials = new Credentials(accessToken)
        };

        var contributions = new List<ContributionDTO>();

        try
        {
            var searchRequest = new SearchIssuesRequest
            {
                Author = username,
                Type = IssueTypeQualifier.PullRequest,
                PerPage = 100
            };

            Console.WriteLine($"🔍 Fetching PRs for user: {username}");

            var searchResult = await client.Search.SearchIssues(searchRequest);

            Console.WriteLine($"📊 Found {searchResult.TotalCount} total PRs");

            foreach (var pr in searchResult.Items)
            {
                try
                {
                    // Extract repository info from PR URL
                    string repoFullName = "";
                    string repoUrl = "";

                    if (!string.IsNullOrEmpty(pr.HtmlUrl))
                    {
                        var uri = new Uri(pr.HtmlUrl);
                        var pathParts = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

                        if (pathParts.Length >= 3 && pathParts[2] == "pull")
                        {
                            var owner = pathParts[0];
                            var repo = pathParts[1];
                            repoFullName = $"{owner}/{repo}";
                            repoUrl = $"https://github.com/{owner}/{repo}";
                        }
                    }

                    if (string.IsNullOrEmpty(repoFullName))
                    {
                        Console.WriteLine($"⚠️ Skipping PR #{pr.Number} - could not parse repository");
                        continue;
                    }

                    // Determine status
                    var status = pr.State.Value == ItemState.Open
                        ? ContributionStatus.Open
                        : (pr.PullRequest?.MergedAt != null ? ContributionStatus.Merged : ContributionStatus.Closed);

                    contributions.Add(new ContributionDTO
                    {
                        PullRequestId = pr.Id,
                        PullRequestNumber = pr.Number,
                        Title = pr.Title ?? string.Empty,
                        Description = pr.Body,
                        Status = status,
                        RepositoryFullName = repoFullName,
                        RepositoryUrl = repoUrl,
                        PrUrl = pr.HtmlUrl ?? string.Empty,
                        GitHubCreatedAt = pr.CreatedAt.UtcDateTime,
                        GitHubMergedAt = pr.PullRequest?.MergedAt?.UtcDateTime,
                        GitHubClosedAt = pr.ClosedAt?.UtcDateTime,
                        CommentsCount = pr.Comments,
                        // Note: GitHub Search API doesn't provide file change details
                        // We'd need to fetch individual PR details for that
                        FilesChanged = 0,
                        Additions = 0,
                        Deletions = 0
                    });

                    await EnsureRepositoryExistsAsync(repoFullName, cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error processing PR #{pr.Number}: {ex.Message}");
                }
            }

            Console.WriteLine($"✅ Successfully processed {contributions.Count} contributions");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error fetching PRs: {ex.Message}");
            throw;
        }

        return contributions;
    }
}