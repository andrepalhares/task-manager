using Shouldly;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Tests.Entities;

public class UserEntityTests
{
    [Fact]
    public void Create_WithValidArgs_ReturnsUserWithGeneratedId()
    {
        var email = "jane@example.com";
        var passwordHash = "hashed_password";
        var name = "Jane";

        var user = UserEntity.Create(email, passwordHash, name);

        user.Email.ShouldBe(email);
        user.PasswordHash.ShouldBe(passwordHash);
        user.Name.ShouldBe(name);
        user.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void Create_WithEmptyEmail_ThrowsArgumentException()
    {
        var ex = Should.Throw<ArgumentException>(() => UserEntity.Create("", "hash", "Name"));
        ex.ParamName.ShouldBe("email");
    }

    [Fact]
    public void Create_WithEmptyPasswordHash_ThrowsArgumentException()
    {
        var ex = Should.Throw<ArgumentException>(() => UserEntity.Create("jane@example.com", "", "Name"));
        ex.ParamName.ShouldBe("passwordHash");
    }

    [Fact]
    public void Create_WithEmptyName_ThrowsArgumentException()
    {
        var ex = Should.Throw<ArgumentException>(() => UserEntity.Create("jane@example.com", "hash", ""));
        ex.ParamName.ShouldBe("name");
    }

    [Fact]
    public void Rehydrate_WithValidArgs_ReturnsUserWithSpecificId()
    {
        var id = Guid.NewGuid();
        var email = "jane@example.com";
        var passwordHash = "hashed_password";
        var name = "Jane";

        var user = UserEntity.Rehydrate(id, email, passwordHash, name);

        user.Id.ShouldBe(id);
        user.Email.ShouldBe(email);
        user.PasswordHash.ShouldBe(passwordHash);
        user.Name.ShouldBe(name);
    }
}
