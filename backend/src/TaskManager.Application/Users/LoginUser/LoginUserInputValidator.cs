using FluentValidation;

namespace TaskManager.Application.Users.LoginUser;

public sealed class LoginUserInputValidator : AbstractValidator<LoginUserInput>
{
    public LoginUserInputValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
