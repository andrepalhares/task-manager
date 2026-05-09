using FluentValidation.TestHelper;
using Shouldly;
using TaskManager.Application.Tasks.GetUserTasks;

namespace TaskManager.Application.Tests.Tasks.GetUserTasks;

public class GetUserTasksInputValidatorTests
{
    private readonly GetUserTasksInputValidator _validator = new();

    [Fact]
    public void Validate_WithValidPage_ShouldPass()
    {
        var input = new GetUserTasksInput(1);

        var result = _validator.TestValidate(input);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithPageZero_ShouldFail()
    {
        var input = new GetUserTasksInput(0);

        var result = _validator.TestValidate(input);

        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Fact]
    public void Validate_WithNegativePage_ShouldFail()
    {
        var input = new GetUserTasksInput(-1);

        var result = _validator.TestValidate(input);

        result.ShouldHaveValidationErrorFor(x => x.Page);
    }
}
