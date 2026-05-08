using System.IdentityModel.Tokens.Jwt;
using Shouldly;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Security;

namespace TaskManager.Infrastructure.Tests.Security;

public class JwtTokenIssuerTests
{
    private readonly JwtSettings _settings = new()
    {
        Key = "DEV-ONLY-replace-in-production-min-32-chars-required",
        Issuer = "TaskManager",
        Audience = "TaskManagerClients",
        AccessTokenLifetimeMinutes = 60
    };

    private static User NewUser() =>
        User.Create("test@example.com", "hashed_password", "Test User");

    private JwtTokenIssuer CreateSut(TimeProvider? timeProvider = null) =>
        new(_settings, timeProvider ?? TimeProvider.System);

    [Fact]
    public void CreateToken_ReturnsParseableJwt()
    {
        var token = CreateSut().CreateToken(NewUser());

        token.Value.ShouldNotBeNullOrEmpty();
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token.Value);
        jwt.ShouldNotBeNull();
    }

    [Fact]
    public void CreateToken_IncludesSubEmailNameAndJtiClaims()
    {
        var user = NewUser();

        var token = CreateSut().CreateToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token.Value);
        jwt.Claims.First(c => c.Type == "sub").Value.ShouldBe(user.Id.ToString());
        jwt.Claims.First(c => c.Type == "email").Value.ShouldBe("test@example.com");
        jwt.Claims.First(c => c.Type == "name").Value.ShouldBe("Test User");
        jwt.Claims.First(c => c.Type == "jti").Value.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public void CreateToken_SetsIssuerAndAudienceFromSettings()
    {
        var token = CreateSut().CreateToken(NewUser());

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token.Value);
        jwt.Issuer.ShouldBe("TaskManager");
        jwt.Audiences.First().ShouldBe("TaskManagerClients");
    }

    [Fact]
    public void CreateToken_SetsExpiryAccordingToConfiguredLifetime()
    {
        var fixedNow = new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var timeProvider = new FixedTimeProvider(fixedNow);

        var token = CreateSut(timeProvider).CreateToken(NewUser());

        token.ExpiresAt.ShouldBe(fixedNow.AddMinutes(60));
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token.Value);
        jwt.ValidTo.ShouldBe(fixedNow.AddMinutes(60).UtcDateTime);
    }

    [Fact]
    public void CreateToken_GeneratesUniqueJtiPerCall()
    {
        var sut = CreateSut();
        var user = NewUser();

        var jwt1 = new JwtSecurityTokenHandler().ReadJwtToken(sut.CreateToken(user).Value);
        var jwt2 = new JwtSecurityTokenHandler().ReadJwtToken(sut.CreateToken(user).Value);

        jwt1.Claims.First(c => c.Type == "jti").Value
            .ShouldNotBe(jwt2.Claims.First(c => c.Type == "jti").Value);
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _now;
        public FixedTimeProvider(DateTimeOffset now) => _now = now;
        public override DateTimeOffset GetUtcNow() => _now;
    }
}
