using OpenSourceHub.Domain.Common;

namespace OpenSourceHub.Domain.Entities;

public class User : BaseEntity
{
    public string GitHubId { get; private set; } = string.Empty;
    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? AvatarUrl { get; private set; }
    public string? Bio { get; private set; }
    public string? Location { get; private set; }
    public string? Company { get; private set; }

    // GitHub OAuth Token (will be encrypted in DB)
    public string AccessToken { get; private set; } = string.Empty;
    public DateTime? TokenExpiresAt { get; private set; }

    // User Stats (denormalized for performance)
    public int TotalContributions { get; private set; }
    public int MergedContributions { get; private set; }
    public int CurrentStreak { get; private set; }
    public int LongestStreak { get; private set; }
    public DateTime? LastContributionDate { get; private set; }
    public DateTime? LastSyncedAt { get; private set; }

    // Preferences
    public UserPreferences Preferences { get; private set; } = new();

    // Navigation Properties
    public ICollection<Bookmark> Bookmarks { get; private set; } = new List<Bookmark>();
    public ICollection<Contribution> Contributions { get; private set; } = new List<Contribution>();
    public ICollection<Notification> Notifications { get; private set; } = new List<Notification>();
    public ICollection<SavedSearch> SavedSearches { get; private set; } = new List<SavedSearch>();

    // Private constructor for EF Core
    private User() { }

    // Factory method for creating new users
    public static User Create(
        string gitHubId,
        string username,
        string email,
        string accessToken,
        string? avatarUrl = null)
    {
        var user = new User
        {
            GitHubId = gitHubId,
            Username = username,
            Email = email,
            AccessToken = accessToken,
            AvatarUrl = avatarUrl,
            TotalContributions = 0,
            MergedContributions = 0,
            CurrentStreak = 0,
            LongestStreak = 0
        };

        return user;
    }

    // Domain methods
    public void UpdateProfile(
        string username,
        string email,
        string? avatarUrl,
        string? bio,
        string? location,
        string? company)
    {
        Username = username;
        Email = email;
        AvatarUrl = avatarUrl;
        Bio = bio;
        Location = location;
        Company = company;
        UpdateTimeStamp();
    }

    public void UpdateAccessToken(string accessToken, DateTime? expiresAt = null)
    {
        AccessToken = accessToken;
        TokenExpiresAt = expiresAt;
        UpdateTimeStamp();
    }

    public void UpdateStats(
        int totalContributions,
        int mergedContributions,
        int currentStreak,
        int longestStreak,
        DateTime? lastContributionDate)
    {
        TotalContributions = totalContributions;
        MergedContributions = mergedContributions;
        CurrentStreak = currentStreak;
        LongestStreak = longestStreak;
        LastContributionDate = lastContributionDate;
        UpdateTimeStamp();
    }

    public void UpdateLastSynced()
    {
        LastSyncedAt = DateTime.UtcNow;
        UpdateTimeStamp();
    }

    public void UpdatePreferences(UserPreferences preferences)
    {
        Preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
        UpdateTimeStamp();
    }

    public bool IsTokenExpired()
    {
        return TokenExpiresAt.HasValue && TokenExpiresAt.Value <= DateTime.UtcNow;
    }
}