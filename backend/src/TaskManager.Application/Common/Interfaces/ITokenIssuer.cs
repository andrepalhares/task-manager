using TaskManager.Domain.Entities;

namespace TaskManager.Application.Common.Interfaces;

public interface ITokenIssuer
{
    AuthToken CreateToken(UserEntity user);
}

public sealed record AuthToken(string Value, DateTimeOffset ExpiresAt);
