public class ContributionStatsDTO
{
    public int TotalContributions { get; set; }
    public int MergedContributions { get; set; }
    public int OpenContributions { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public DateTime? LastContributionDate { get; set; }
    public DateTime? LastSyncedAt { get; set; }
}
