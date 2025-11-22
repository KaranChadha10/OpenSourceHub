using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenSourceHub.Application.Features.Auth.Commands;

namespace OpenSourceHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly GithubOptions _gitHubOptions;

    public AuthController(IMediator mediator, IOptions<GithubOptions> gitHubOptions)
    {
        _mediator = mediator;
        _gitHubOptions = gitHubOptions.Value;
    }

    [HttpGet("github/login")]
    public IActionResult GitHubLogin()
    {
        var redirectUrl = $"https://github.com/login/oauth/authorize?" +
                         $"client_id={_gitHubOptions.ClientId}&" +
                         $"redirect_uri={_gitHubOptions.RedirectUri}&" +
                         $"scope={_gitHubOptions.Scope}";

        return Ok(new { redirectUrl });
    }

    [HttpGet("github/callback")]
    public async Task<IActionResult> GitHubCallback([FromQuery] string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest(new { error = "Authorization code is required" });
        }

        try
        {
            var command = new GitHubCallbackCommand(code);
            var result = await _mediator.Send(command);

            // Redirect to frontend with token
            return Redirect($"http://localhost:5173/auth/callback?token={result.Token}");
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}