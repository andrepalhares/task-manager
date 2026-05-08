using FluentValidation;

namespace TaskManager.Application.Tasks.CreateTask;

public sealed class CreateTaskInputValidator : AbstractValidator<CreateTaskInput>
{
    public CreateTaskInputValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.")
            .When(x => x.Description is not null);
    }
}
