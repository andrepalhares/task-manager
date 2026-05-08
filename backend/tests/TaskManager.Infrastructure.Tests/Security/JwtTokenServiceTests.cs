using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Shouldly;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Security;

namespace TaskManager.Infrastructure.Tests.Security;

public class JwtTokenServiceTests
{
    private readonly JwtSettings _jwtSettings = new()
    {
        Key = "DEV-ONLY-replace-in-production-min-32-chars-required",
        Issuer = "TaskManager",
        Audience = "TaskManagerClients",
        AccessTokenLifetimeMinutes = 60
    };

    [Fact]
    public void CreateToken_ReturnsTokenSignedWithConfiguredKey()
    {
        var options = Options.Create(_jwtSettings);
        var service = new JwtTokenIssuer(options, TimeProvider.System);

        var user = User.Create("test@example.com", "hashed_password", "Test User");

        var token = service.CreateToken(user);

        token.Value.ShouldNotBeNullOrEmpty();
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token.Value);
        jwtToken.ShouldNotBeNull();
    }

    [Fact]
    public void CreateToken_IncludesSubEmailNameAndJtiClaims()
    {
        var options = Options.Create(_jwtSettings);
        var service = new JwtTokenIssuer(options, TimeProvider.System);

        var user = User.Create("test@example.com", "hashed_password", "Test User");

        var token = service.CreateToken(user);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token.Value);

        var subClaim = jwtToken.Claims.First(c => c.Type == "sub");
        subClaim.Value.ShouldBe(user.Id.ToString());

        var emailClaim = jwtToken.Claims.First(c => c.Type == "email");
        emailClaim.Value.ShouldBe("test@example.com");

        var nameClaim = jwtToken.Claims.First(c => c.Type == "name");
        nameClaim.Value.ShouldBe("Test User");

        var jtiClaim = jwtToken.Claims.First(c => c.Type == "jti");
        jtiClaim.Value.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public void CreateToken_SetsIssuerAndAudienceFromSettings()
    {
        var options = Options.Create(_jwtSettings);
        var service = new JwtTokenIssuer(options, TimeProvider.System);

        var user = User.Create("test@example.com", "hashed_password", "Test User");

        var token = service.CreateToken(user);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token.Value);

        jwtToken.Issuer.ShouldBe("TaskManager");
        jwtToken.Audiences.First().ShouldBe("TaskManagerClients");
    }

    [Fact]
    public void CreateToken_SetsExpiryAccordingToConfiguredLifetime()
    {
        var options = Options.Create(_jwtSettings);
        var service = new JwtTokenIssuer(options, TimeProvider.System);

        var user = User.Create("test@example.com", "hashed_password", "Test User");
        var beforeCreation = DateTime.UtcNow;

        var token = service.CreateToken(user);

        var afterCreation = DateTime.UtcNow;

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token.Value);

        var expectedExpiryMin = beforeCreation.AddMinutes(60).AddSeconds(-1);
        var expectedExpiryMax = afterCreation.AddMinutes(60).AddSeconds(1);

        jwtToken.ValidTo.ShouldBeGreaterThanOrEqualTo(expectedExpiryMin);
        jwtToken.ValidTo.ShouldBeLessThanOrEqualTo(expectedExpiryMax);
    }

    [Fact]
    public void CreateToken_GeneratesUniqueJtiPerCall()
    {
        var options = Options.Create(_jwtSettings);
        var service = new JwtTokenIssuer(options, TimeProvider.System);

        var user = User.Create("test@example.com", "hashed_password", "Test User");

        var token1 = service.CreateToken(user);
        var token2 = service.CreateToken(user);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken1 = handler.ReadJwtToken(token1.Value);
        var jwtToken2 = handler.ReadJwtToken(token2.Value);

        var jti1 = jwtToken1.Claims.First(c => c.Type == "jti").Value;
        var jti2 = jwtToken2.Claims.First(c => c.Type == "jti").Value;

        jti1.ShouldNotBe(jti2);
    }
}
