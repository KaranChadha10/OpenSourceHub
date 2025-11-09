public class IssueSearchFilters
{
    public string? Language { get; set; }
    public List<string> Labels { get; set; } = new();
    public int? MinimumStars { get; set; }
    public string? State { get; set; } = "open";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}