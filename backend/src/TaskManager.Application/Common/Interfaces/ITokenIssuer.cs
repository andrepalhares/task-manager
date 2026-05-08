using TaskManager.Domain.Entities;

namespace TaskManager.Application.Common.Interfaces;

public interface ITokenIssuer
{
    AuthToken CreateToken(User user);
}

public sealed record AuthToken(string Value, DateTimeOffset ExpiresAt);
