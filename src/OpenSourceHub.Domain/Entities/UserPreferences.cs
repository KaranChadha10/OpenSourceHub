using System;

namespace OpenSourceHub.Domain.Entities;

public class UserPreferences
{
    public List<string> PreferredLanguages { get; set; } = new();
    public List<string> PreferredLabels { get; set; } = new();
    public int MinimumStars { get; set; } = 0;
    public bool EmailNotifications { get; set; } = true;
    public bool InAppNotifications { get; set; } = true;
    public string TimeZone { get; set; } = "UTC";

    //Empty Constructor for EF Core
    public UserPreferences() { }

}
