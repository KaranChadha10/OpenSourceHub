using OpenSourceHub.Domain.Entities;
using OpenSourceHub.Domain.Enum;

public class Repository : BaseEntity
{
    public long GitHubId { get; private set; }
    public string FullName { get; private set; } = string.Empty; // e.g. Facebook / react
    public string Name { get; private set; } = string.Empty;
    public string Owner { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Language { get; private set; }
    public string? Homepage { get; private set; }

    //Metrics
    public int StarsCount { get; private set; }
    public int ForksCount { get; private set; }
    public int OpenIssuesCount { get; private set; }
    public int WatchersCount { get; private set; }


    //MetaData
    public bool IsArchived { get; private set; }
    public bool IsFork { get; private set; }
    public string? DefaultBranch { get; private set; }
    public DateTime? GitHubCreatedAt { get; private set; }
    public DateTime? GitHubUpdatedAt { get; private set; }
    public DateTime LastSyncedAt { get; private set; }

    // Topics/Tags
    public List<string> Topics { get; private set; } = new();

    //Navigation Properties
    public ICollection<Issue> Issues { get; private set; } = new List<Issue>();
    public ICollection<Contribution> Contributions { get; private set; } = new List<Contribution>();
    public ICollection<Bookmark> Bookmarks { get; private set; } = new List<Bookmark>();

    private Repository() { }

    public static Repository Create(
        long gitHubId,
        string fullName,
        string name,
        string owner,
        string? language = null)
    {
        var repository = new Repository
        {
            GitHubId = gitHubId,
            FullName = fullName,
            Name = name,
            Owner = owner,
            Language = language,
            LastSyncedAt = DateTime.UtcNow
        };

        return repository;
    }

    public void UpdateMetadata(
        string? description,
        int starsCount,
        int forksCount,
        int openIssuesCount,
        int watchersCount,
        bool isArchived,
        bool isFork,
        string? defaultBranch,
        DateTime? gitHubCreatedAt,
        DateTime? gitHubUpdatedAt,
        List<string>? topics = null,
        string? homepage = null)
    {
        Description = description;
        StarsCount = starsCount;
        ForksCount = forksCount;
        OpenIssuesCount = openIssuesCount;
        WatchersCount = watchersCount;
        IsArchived = isArchived;
        IsFork = isFork;
        DefaultBranch = defaultBranch;
        GitHubCreatedAt = gitHubCreatedAt;
        GitHubUpdatedAt = gitHubUpdatedAt;
        Topics = topics ?? new List<string>();
        Homepage = homepage;
        LastSyncedAt = DateTime.UtcNow;
        UpdateTimeStamp();
    }

    public void IncrementStars() => StarsCount++;
    public void DecrementStars() => StarsCount = Math.Max(0, StarsCount - 1);
}