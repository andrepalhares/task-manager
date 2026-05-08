using Shouldly;
using TaskManager.Infrastructure.Security;

namespace TaskManager.Infrastructure.Tests.Security;

public class BCryptPasswordHasherTests
{
    private readonly BCryptPasswordHasher _hasher = new();

    [Fact]
    public void Hash_ReturnsNonEmptyString()
    {
        // Arrange
        var password = "password123";

        // Act
        var hash = _hasher.Hash(password);

        // Assert
        hash.ShouldNotBeNullOrEmpty();
        hash.ShouldNotBe(password);
    }

    [Fact]
    public void Hash_ProducesDifferentHashesForSamePassword()
    {
        // Arrange
        var password = "password123";

        // Act
        var hash1 = _hasher.Hash(password);
        var hash2 = _hasher.Hash(password);

        // Assert
        hash1.ShouldNotBe(hash2);
    }

    [Fact]
    public void Verify_WithMatchingPassword_ReturnsTrue()
    {
        // Arrange
        var password = "password123";
        var hash = _hasher.Hash(password);

        // Act
        var result = _hasher.Verify(password, hash);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Verify_WithWrongPassword_ReturnsFalse()
    {
        // Arrange
        var password = "password123";
        var wrongPassword = "wrongpassword";
        var hash = _hasher.Hash(password);

        // Act
        var result = _hasher.Verify(wrongPassword, hash);

        // Assert
        result.ShouldBeFalse();
    }
}
