public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public string? Location { get; set; }
    public string? Company { get; set; }
    public int TotalContributions { get; set; }
    public int MergedContributions { get; set; }
    public int CurrentStreak { get; set; }
}