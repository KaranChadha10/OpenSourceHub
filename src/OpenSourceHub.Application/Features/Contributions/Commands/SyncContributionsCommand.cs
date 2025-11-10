using MediatR;

public record SyncContributionsCommand(Guid UserId) : IRequest<SyncContributionsResultDto>;