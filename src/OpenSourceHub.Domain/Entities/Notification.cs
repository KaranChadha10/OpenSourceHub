using System;
using OpenSourceHub.Domain.Enum;

namespace OpenSourceHub.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; private set; }
    public NotificationType Type { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public string? Link { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    //MetaData (JSON for flexibility)
    public Dictionary<string, string> Metadata { get; private set; } = new();

    //Navigation Properties
    public User User { get; private set; } = null!;

    private Notification() { }

    public static Notification Create(
        Guid userId,
        NotificationType type,
        string title,
        string message,
        string? link = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            Link = link,
            IsRead = false
        };

        return notification;
    }

    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
            UpdateTimeStamp();
        }
    }

    public void MarkAsUnread()
    {
        if (IsRead)
        {
            IsRead = false;
            ReadAt = null;
            UpdateTimeStamp();
        }
    }

    public void AddMetadata(string key, string value)
    {
        Metadata[key] = value;
        UpdateTimeStamp();
    }
}
