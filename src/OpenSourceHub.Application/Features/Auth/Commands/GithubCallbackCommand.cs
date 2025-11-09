using MediatR;

namespace OpenSourceHub.Application.Features.Auth.Commands;

public record GitHubCallbackCommand(string Code) : IRequest<AuthResponseDto>;