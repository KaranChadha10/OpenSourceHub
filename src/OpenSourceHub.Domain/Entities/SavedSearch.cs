using System;

namespace OpenSourceHub.Domain.Entities;

public class SavedSearch : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Language { get; private set; }
    public List<string> Labels { get; private set; } = new();
    public int? MinimumStars { get; private set; }
    public bool NotifyOnNewIssues { get; private set; }
    public DateTime? LastNotifiedAt { get; private set; }

    //Navigation properties
    public User User { get; private set; } = null!;

    private SavedSearch() { }

    public static SavedSearch Create(
        Guid userId,
        string name,
        string? language = null,
        List<string>? labels = null,
        int? minimumStars = null,
        bool notifyOnNewIssues = false)
    {
        var savedSearch = new SavedSearch
        {
            UserId = userId,
            Name = name,
            Language = language,
            Labels = labels ?? new List<string>(),
            MinimumStars = minimumStars,
            NotifyOnNewIssues = notifyOnNewIssues
        };

        return savedSearch;
    }

    public void Update(string name, string? language, List<string>? labels, int? minimumStars, bool notifyOnNewIssues)
    {
        Name = name;
        Language = language;
        Labels = labels ?? new List<string>();
        MinimumStars = minimumStars;
        NotifyOnNewIssues = notifyOnNewIssues;
        UpdateTimeStamp();
    }

    public void UpdateLastNotified()
    {
        LastNotifiedAt = DateTime.UtcNow;
        UpdateTimeStamp();
    }

}
