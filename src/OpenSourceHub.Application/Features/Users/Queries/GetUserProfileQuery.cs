using MediatR;
using OpenSourceHub.Domain.Entities;

public record GetUserProfileQuery(Guid UserId) : IRequest<UserDto>;