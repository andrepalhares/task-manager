using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskManager.Application.Common.Interfaces;

namespace TaskManager.WebApi.Auth;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User
                ?? throw new InvalidOperationException("No HTTP context — current user is unavailable.");

            var sub =
                user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new InvalidOperationException("Authenticated user has no subject claim.");

            return Guid.Parse(sub);
        }
    }
}
