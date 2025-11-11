using System;

namespace OpenSourceHub.Domain.Entities;

public class Bookmark : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid RepositoryId { get; private set; }
    public string? Notes { get; private set; }
    public List<string> Tags { get; private set; } = new();

    //Navigation Properties
    public User User { get; private set; } = null!;
    public Repository Repository { get; private set; } = null!;

    private Bookmark() { }

    public static Bookmark Create(Guid userId, Guid repositoryId, string? notes = null)
    {
        var bookmark = new Bookmark
        {
            UserId = userId,
            RepositoryId = repositoryId,
            Notes = notes
        };

        return bookmark;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
        UpdateTimeStamp();
    }

    public void AddTag(string tag)
    {
        if (!Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
        {
            Tags.Add(tag);
            UpdateTimeStamp();
        }
    }

    public void RemoveTag(string tag)
    {
        Tags.RemoveAll(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase));
        UpdateTimeStamp();
    }
}
