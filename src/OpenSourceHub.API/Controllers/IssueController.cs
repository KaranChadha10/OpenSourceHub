using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IssueController : ControllerBase
{
    private readonly IMediator _mediator;

    public IssueController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Search for GitHub issues with filters
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IssueSearchResultDto>> Search(
        [FromQuery] string? language,
        [FromQuery] string? labels,
        [FromQuery] int? minimumStars,
        [FromQuery] string? state,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var filters = new IssueSearchFilters
        {
            Language = language,
            Labels = string.IsNullOrEmpty(labels)
                ? new List<string>()
                : labels.Split(',').Select(l => l.Trim()).ToList(),
            MinimumStars = minimumStars,
            State = state ?? "open",
            Page = page,
            PageSize = Math.Min(pageSize, 100) // Max 100 per page
        };

        var query = new SearchIssueQuery(filters);
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}