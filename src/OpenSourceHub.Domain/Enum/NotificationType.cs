namespace OpenSourceHub.Domain.Enum;

public enum NotificationType
{
    PullRequestMerged = 0,
    PullRequestClosed = 1,
    NewMatchingIssue = 2,
    StreakMilestone = 3,
    RepositoryUpdate = 4
}
