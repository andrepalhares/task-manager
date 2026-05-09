using Shouldly;
using TaskManager.Application.Users.LoginUser;

namespace TaskManager.Application.Tests.Users.LoginUser;

public class LoginUserInputValidatorTests
{
    private readonly LoginUserInputValidator _validator = new();

    [Fact]
    public void Validate_WithValidInput_HasNoErrors()
    {
        var input = new LoginUserInput("jane@example.com", "password123");
        var result = _validator.Validate(input);
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithMissingEmail_HasError(string email)
    {
        var input = new LoginUserInput(email, "password123");
        var result = _validator.Validate(input);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(LoginUserInput.Email));
    }

    [Fact]
    public void Validate_WithInvalidEmailFormat_HasError()
    {
        var input = new LoginUserInput("not-an-email", "password123");
        var result = _validator.Validate(input);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(LoginUserInput.Email));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithMissingPassword_HasError(string password)
    {
        var input = new LoginUserInput("jane@example.com", password);
        var result = _validator.Validate(input);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(LoginUserInput.Password));
    }
}
