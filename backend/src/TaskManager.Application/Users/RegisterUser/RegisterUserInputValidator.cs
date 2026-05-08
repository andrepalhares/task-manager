using FluentValidation;

namespace TaskManager.Application.Users.RegisterUser;

public sealed class RegisterUserInputValidator : AbstractValidator<RegisterUserInput>
{
    public RegisterUserInputValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(100);
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(100);
    }
}
