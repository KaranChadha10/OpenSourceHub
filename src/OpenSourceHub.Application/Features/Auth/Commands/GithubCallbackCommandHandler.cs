using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenSourceHub.Application.Common.Interfaces;
using OpenSourceHub.Domain.Entities;

namespace OpenSourceHub.Application.Features.Auth.Commands;

public class GitHubCallbackCommandHandler : IRequestHandler<GitHubCallbackCommand, AuthResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IGitHubService _gitHubService;
    private readonly ITokenService _tokenService;

    public GitHubCallbackCommandHandler(
        IApplicationDbContext context,
        IGitHubService gitHubService,
        ITokenService tokenService)
    {
        _context = context;
        _gitHubService = gitHubService;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> Handle(GitHubCallbackCommand request, CancellationToken cancellationToken)
    {
        // 1. Exchange code for access token
        var accessToken = await _gitHubService.GetAccessTokenAsync(request.Code);

        // 2. Get user info from GitHub
        var gitHubUser = await _gitHubService.GetUserAsync(accessToken);

        // 3. Find or create user in database
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.GitHubId == gitHubUser.GitHubId, cancellationToken);

        if (user == null)
        {
            // Create new user
            user = User.Create(
                gitHubUser.GitHubId,
                gitHubUser.Login,
                gitHubUser.Email,
                accessToken,
                gitHubUser.AvatarUrl
            );

            user.UpdateProfile(
                gitHubUser.Login,
                gitHubUser.Email,
                gitHubUser.AvatarUrl,
                gitHubUser.Bio,
                gitHubUser.Location,
                gitHubUser.Company
            );

            _context.Users.Add(user);
        }
        else
        {
            // Update existing user
            user.UpdateProfile(
                gitHubUser.Login,
                gitHubUser.Email,
                gitHubUser.AvatarUrl,
                gitHubUser.Bio,
                gitHubUser.Location,
                gitHubUser.Company
            );
            user.UpdateAccessToken(accessToken);
        }

        await _context.SaveChangesAsync(cancellationToken);

        // 4. Generate JWT token
        var jwtToken = _tokenService.GenerateToken(user);

        // 5. Return response
        return new AuthResponseDto
        {
            Token = jwtToken,
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl,
                Bio = user.Bio,
                Location = user.Location,
                Company = user.Company,
                TotalContributions = user.TotalContributions,
                MergedContributions = user.MergedContributions,
                CurrentStreak = user.CurrentStreak
            }
        };
    }
}