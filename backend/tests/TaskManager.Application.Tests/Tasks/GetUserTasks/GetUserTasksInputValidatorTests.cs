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
        // Arrange
        var input = new GetUserTasksInput(1);

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithPageZero_ShouldFail()
    {
        // Arrange
        var input = new GetUserTasksInput(0);

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Fact]
    public void Validate_WithNegativePage_ShouldFail()
    {
        // Arrange
        var input = new GetUserTasksInput(-1);

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }
}
