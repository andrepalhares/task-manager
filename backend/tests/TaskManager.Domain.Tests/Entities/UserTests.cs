using Shouldly;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void Create_WithValidArgs_ReturnsUserWithGeneratedId()
    {
        // Arrange
        var email = "jane@example.com";
        var passwordHash = "hashed_password";
        var name = "Jane";

        // Act
        var user = User.Create(email, passwordHash, name);

        // Assert
        user.Email.ShouldBe(email);
        user.PasswordHash.ShouldBe(passwordHash);
        user.Name.ShouldBe(name);
        user.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void Create_WithEmptyEmail_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => User.Create("", "hash", "Name"));
        ex.ParamName.ShouldBe("email");
    }

    [Fact]
    public void Create_WithEmptyPasswordHash_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => User.Create("jane@example.com", "", "Name"));
        ex.ParamName.ShouldBe("passwordHash");
    }

    [Fact]
    public void Create_WithEmptyName_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Should.Throw<ArgumentException>(() => User.Create("jane@example.com", "hash", ""));
        ex.ParamName.ShouldBe("name");
    }

    [Fact]
    public void Rehydrate_WithValidArgs_ReturnsUserWithSpecificId()
    {
        // Arrange
        var id = Guid.NewGuid();
        var email = "jane@example.com";
        var passwordHash = "hashed_password";
        var name = "Jane";

        // Act
        var user = User.Rehydrate(id, email, passwordHash, name);

        // Assert
        user.Id.ShouldBe(id);
        user.Email.ShouldBe(email);
        user.PasswordHash.ShouldBe(passwordHash);
        user.Name.ShouldBe(name);
    }
}
