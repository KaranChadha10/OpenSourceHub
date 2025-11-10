using OpenSourceHub.Domain.Enum;

public class ContributionDTO
{
    public Guid Id { get; set; }
    public long PullRequestId { get; set; }
    public int PullRequestNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ContributionStatus Status { get; set; }

    // Metrics
    public int FilesChanged { get; set; }
    public int Additions { get; set; }
    public int Deletions { get; set; }
    public int CommentsCount { get; set; }

    // Repository Info
    public string RepositoryFullName { get; set; } = string.Empty;
    public string RepositoryUrl { get; set; } = string.Empty;
    public string PrUrl { get; set; } = string.Empty;

    // Dates
    public DateTime GitHubCreatedAt { get; set; }
    public DateTime? GitHubMergedAt { get; set; }
    public DateTime? GitHubClosedAt { get; set; }
    public DateTime LastSyncedAt { get; set; }

    // AI Summary (Phase 2)
    public string? AiSummary { get; set; }

    // Helper properties
    public bool IsMerged => Status == ContributionStatus.Merged;
    public bool IsOpen => Status == ContributionStatus.Open;
}