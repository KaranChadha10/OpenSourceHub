using OpenSourceHub.Domain.Entities;
using OpenSourceHub.Domain.Enum;

public class Contribution : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid RepositoryId { get; private set; }
    public long PullRequestId { get; private set; }
    public int PullRequestNumber { get; private set; }


    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public ContributionStatus Status { get; private set; }

    //Metrics
    public int FilesChanged { get; private set; }
    public int Additions { get; private set; }
    public int Deletions { get; private set; }
    public int CommentsCount { get; private set; }

    //Dates
    public DateTime GitHubCreatedAt { get; private set; }
    public DateTime? GitHubMergedAt { get; private set; }
    public DateTime? GitHubClosedAt { get; private set; }
    public DateTime LastSyncedAt { get; private set; }

    //AI Summary (Phase 2)
    public string? AiSummary { get; private set; }

    //Navigation Properties
    public User User { get; private set; } = null!;
    public Repository Repository { get; private set; } = null!;

    private Contribution() { }

    public static Contribution Create
    (Guid userId, Guid repositoryId, long pullRequestId, int pullRequestNumber, string title, DateTime gitHubCreatedAt)
    {
        var contribution = new Contribution
        {
            UserId = userId,
            RepositoryId = repositoryId,
            PullRequestId = pullRequestId,
            PullRequestNumber = pullRequestNumber,
            Title = title,
            Status = ContributionStatus.Open,
            GitHubCreatedAt = gitHubCreatedAt,
            LastSyncedAt = DateTime.UtcNow
        };

        return contribution;
    }

    public void UpdateDetails(
        string title,
        string? description,
        ContributionStatus status,
        int filesChanged,
        int additions,
        int deletions,
        int commentsCount,
        DateTime? gitHubMergedAt,
        DateTime? gitHubClosedAt)
    {
        Title = title;
        Description = description;
        Status = status;
        FilesChanged = filesChanged;
        Additions = additions;
        Deletions = deletions;
        CommentsCount = commentsCount;
        GitHubMergedAt = gitHubMergedAt;
        GitHubClosedAt = gitHubClosedAt;
        LastSyncedAt = DateTime.UtcNow;
        UpdateTimeStamp();
    }

    public void SetAiSummary(string summary)
    {
        AiSummary = summary;
        UpdateTimeStamp();
    }

    public bool IsMerged() => Status == ContributionStatus.Merged;
    public bool IsOpen() => Status == ContributionStatus.Open;
}