using FluentValidation;

namespace TaskManager.Application.Tasks.GetUserTasks;

public sealed class GetUserTasksInputValidator : AbstractValidator<GetUserTasksInput>
{
    public GetUserTasksInputValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be greater than or equal to 1.");
    }
}
