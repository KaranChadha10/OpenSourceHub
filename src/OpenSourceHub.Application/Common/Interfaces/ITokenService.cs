using OpenSourceHub.Domain.Entities;

public interface ITokenService
{
    string GenerateToken(User user);
}