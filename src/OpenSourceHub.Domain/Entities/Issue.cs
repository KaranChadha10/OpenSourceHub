using System.Diagnostics.Contracts;
using OpenSourceHub.Domain.Enum;

public class Issue : BaseEntity
{
    public long GitHubId { get; private set; }
    public Guid RepositoryId { get; private set; }
    public int Number { get; private set; } // Issue number in the repo
    public string Title { get; private set; } = string.Empty;
    public string? Body { get; private set; }
    public string State { get; private set; } = "open";
    public List<string> Labels { get; private set; } = new();

    //AI generated fields (Phase 2, but adding schema now)
    public IssueDifficulty? Difficulty { get; private set; }
    public int? EstimatedMinutes { get; private set; }

    //MetaData
    public string? AuthorUsername { get; private set; }
    public int CommentsCount { get; private set; }
    public DateTime GitHubCreatedAt { get; private set; }
    public DateTime? GitHubUpdatedAt { get; private set; }
    public DateTime? GitHubClosedAt { get; private set; }
    public DateTime LastSyncedAt { get; private set; }

    //Navigation Properties
    public Repository Repository { get; private set; } = null!;

    private Issue() { }

    public static Issue Create(
        long gitHubId,
        Guid repositoryId,
        int number,
        string title,
        DateTime gitHubCreatedAt)
    {
        var issue = new Issue
        {
            GitHubId = gitHubId,
            RepositoryId = repositoryId,
            Number = number,
            Title = title,
            GitHubCreatedAt = gitHubCreatedAt,
            LastSyncedAt = DateTime.UtcNow
        };

        return issue;
    }

    public void UpdateDetails(
        string title,
        string? body,
        string state,
        List<string> labels,
        string? authorUsername,
        int commentsCount,
        DateTime? gitHubUpdatedAt,
        DateTime? gitHubClosedAt)
    {
        Title = title;
        Body = body;
        State = state;
        Labels = labels;
        AuthorUsername = authorUsername;
        CommentsCount = commentsCount;
        GitHubUpdatedAt = gitHubUpdatedAt;
        GitHubClosedAt = gitHubClosedAt;
        LastSyncedAt = DateTime.UtcNow;
        UpdateTimeStamp();
    }

    public void SetDifficulty(IssueDifficulty difficulty, int estimatedMinutes)
    {
        Difficulty = difficulty;
        EstimatedMinutes = estimatedMinutes;
        UpdateTimeStamp();
    }

    public bool IsOpen() => State.Equals("open", StringComparison.OrdinalIgnoreCase);
    public bool HasLabel(string label) => Labels.Any(l => l.Equals(label, StringComparison.OrdinalIgnoreCase));
}