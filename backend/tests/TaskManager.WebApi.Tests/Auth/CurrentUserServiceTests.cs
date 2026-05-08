using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Shouldly;
using TaskManager.WebApi.Auth;

namespace TaskManager.WebApi.Tests.Auth;

public class CurrentUserServiceTests
{
    private readonly IHttpContextAccessor _httpContextAccessor = Substitute.For<IHttpContextAccessor>();

    private CurrentUserService CreateSut() => new(_httpContextAccessor);

    private void SetClaims(params Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims, authenticationType: "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var ctx = new DefaultHttpContext { User = principal };
        _httpContextAccessor.HttpContext.Returns(ctx);
    }

    [Fact]
    public void UserId_WhenSubClaimPresent_ReturnsParsedGuid()
    {
        var expected = Guid.NewGuid();
        SetClaims(new Claim("sub", expected.ToString()));

        CreateSut().UserId.ShouldBe(expected);
    }

    [Fact]
    public void UserId_WhenSubMissingButNameIdentifierPresent_ReturnsParsedGuid()
    {
        var expected = Guid.NewGuid();
        SetClaims(new Claim(ClaimTypes.NameIdentifier, expected.ToString()));

        CreateSut().UserId.ShouldBe(expected);
    }

    [Fact]
    public void UserId_WhenHttpContextNull_ThrowsInvalidOperationException()
    {
        _httpContextAccessor.HttpContext.Returns((HttpContext?)null);

        Should.Throw<InvalidOperationException>(() => _ = CreateSut().UserId);
    }

    [Fact]
    public void UserId_WhenNoSubjectClaim_ThrowsInvalidOperationException()
    {
        SetClaims(new Claim("email", "user@example.com"));

        Should.Throw<InvalidOperationException>(() => _ = CreateSut().UserId);
    }

    [Fact]
    public void UserId_WhenSubClaimNotAGuid_ThrowsFormatException()
    {
        SetClaims(new Claim("sub", "not-a-guid"));

        Should.Throw<FormatException>(() => _ = CreateSut().UserId);
    }
}
