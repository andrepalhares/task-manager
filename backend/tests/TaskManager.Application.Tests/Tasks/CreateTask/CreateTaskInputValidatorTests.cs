using FluentValidation.TestHelper;
using TaskManager.Application.Tasks.CreateTask;
using DomainTaskStatus = TaskManager.Domain.Entities.TaskStatus;

namespace TaskManager.Application.Tests.Tasks.CreateTask;

public class CreateTaskInputValidatorTests
{
    private readonly CreateTaskInputValidator _validator = new();

    [Fact]
    public void Validate_WithValidInput_ShouldPass()
    {
        var input = new CreateTaskInput("Test Title", "Test Description", DomainTaskStatus.Pending, null);

        var result = _validator.TestValidate(input);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyTitle_ShouldFail()
    {
        var input = new CreateTaskInput("", null, DomainTaskStatus.Pending, null);

        var result = _validator.TestValidate(input);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_WithTitleExceedingMaxLength_ShouldFail()
    {
        var longTitle = new string('a', 201);
        var input = new CreateTaskInput(longTitle, null, DomainTaskStatus.Pending, null);

        var result = _validator.TestValidate(input);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_WithDescriptionExceedingMaxLength_ShouldFail()
    {
        var longDescription = new string('a', 2001);
        var input = new CreateTaskInput("Title", longDescription, DomainTaskStatus.Pending, null);

        var result = _validator.TestValidate(input);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_WithNullDescription_ShouldPass()
    {
        var input = new CreateTaskInput("Title", null, DomainTaskStatus.Pending, null);

        var result = _validator.TestValidate(input);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
