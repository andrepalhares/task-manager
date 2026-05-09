using Shouldly;
using TaskManager.Infrastructure.Security;

namespace TaskManager.Infrastructure.Tests.Security;

public class BCryptPasswordHasherTests
{
    private readonly BCryptPasswordHasher _hasher = new();

    [Fact]
    public void Hash_ReturnsNonEmptyString()
    {
        var password = "password123";

        var hash = _hasher.Hash(password);

        hash.ShouldNotBeNullOrEmpty();
        hash.ShouldNotBe(password);
    }

    [Fact]
    public void Hash_ProducesDifferentHashesForSamePassword()
    {
        var password = "password123";

        var hash1 = _hasher.Hash(password);
        var hash2 = _hasher.Hash(password);

        hash1.ShouldNotBe(hash2);
    }

    [Fact]
    public void Verify_WithMatchingPassword_ReturnsTrue()
    {
        var password = "password123";
        var hash = _hasher.Hash(password);

        var result = _hasher.Verify(password, hash);

        result.ShouldBeTrue();
    }

    [Fact]
    public void Verify_WithWrongPassword_ReturnsFalse()
    {
        var password = "password123";
        var wrongPassword = "wrongpassword";
        var hash = _hasher.Hash(password);

        var result = _hasher.Verify(wrongPassword, hash);

        result.ShouldBeFalse();
    }
}
