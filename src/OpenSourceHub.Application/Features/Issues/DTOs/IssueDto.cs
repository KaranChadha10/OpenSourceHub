public class IssueDto
{
    public long GitHubId { get; set; }
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Body { get; set; }
    public string State { get; set; } = string.Empty;
    public List<string> Labels { get; set; } = new();
    public string RepositoryFullName { get; set; } = string.Empty;
    public string RepositoryUrl { get; set; } = string.Empty;
    public int RepositoryStars { get; set; }
    public string? RepositoryLanguage { get; set; }
    public string IssueUrl { get; set; } = string.Empty;
    public string? AuthorUsername { get; set; }
    public int CommentsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}