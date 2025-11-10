public class SyncContributionsResultDto
{
    public int NewContributions { get; set; }
    public int UpdatedContributions { get; set; }
    public int TotalContributions { get; set; }
    public ContributionStatsDTO Stats { get; set; } = new();
    public DateTime SyncedAt { get; set; }
}