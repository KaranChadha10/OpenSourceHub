using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContributionsController : ControllerBase
{
    private readonly IMediator _mediator;
    public ContributionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Sync contributions from GitHub
    /// </summary>
    [HttpPost("sync")]
    public async Task<IActionResult> SyncContributions()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "Invalid token" });
        }

        var command = new SyncContributionsCommand(Guid.Parse(userId));
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    /// <summary>
    /// Get user's contributions
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetContributions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "Invalid token" });
        }

        var query = new GetUserContributionQuery(Guid.Parse(userId), page, pageSize);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Get contribution stats
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "Invalid token" });
        }

        var query = new GetContributionStatsQuery(Guid.Parse(userId));
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}