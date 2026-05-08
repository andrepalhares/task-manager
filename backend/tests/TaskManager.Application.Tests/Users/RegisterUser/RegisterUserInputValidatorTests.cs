using FluentValidation;
using Shouldly;
using TaskManager.Application.Users.RegisterUser;

namespace TaskManager.Application.Tests.Users.RegisterUser;

public class RegisterUserInputValidatorTests
{
    private readonly RegisterUserInputValidator _validator = new();

    [Fact]
    public void Validate_WithValidInput_Succeeds()
    {
        var input = new RegisterUserInput("jane@example.com", "password123", "Jane Doe");
        var result = _validator.Validate(input);
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WithEmptyEmail_Fails()
    {
        var input = new RegisterUserInput("", "password123", "Jane Doe");
        var result = _validator.Validate(input);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(RegisterUserInput.Email));
    }

    [Fact]
    public void Validate_WithMalformedEmail_Fails()
    {
        var input = new RegisterUserInput("not-an-email", "password123", "Jane Doe");
        var result = _validator.Validate(input);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(RegisterUserInput.Email));
    }

    [Fact]
    public void Validate_WithEmptyPassword_Fails()
    {
        var input = new RegisterUserInput("jane@example.com", "", "Jane Doe");
        var result = _validator.Validate(input);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(RegisterUserInput.Password));
    }

    [Fact]
    public void Validate_WithPasswordShorterThan8Chars_Fails()
    {
        var input = new RegisterUserInput("jane@example.com", "short", "Jane Doe");
        var result = _validator.Validate(input);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(RegisterUserInput.Password));
    }

    [Fact]
    public void Validate_WithEmptyName_Fails()
    {
        var input = new RegisterUserInput("jane@example.com", "password123", "");
        var result = _validator.Validate(input);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(RegisterUserInput.Name));
    }

    [Fact]
    public void Validate_WithNameLongerThan100Chars_Fails()
    {
        var longName = new string('a', 101);
        var input = new RegisterUserInput("jane@example.com", "password123", longName);
        var result = _validator.Validate(input);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(RegisterUserInput.Name));
    }
}
