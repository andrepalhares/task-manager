using FluentValidation.TestHelper;
using Shouldly;
using DomainTaskStatus = TaskManager.Domain.Entities.TaskStatus;
using TaskManager.Application.Tasks.CreateTask;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Tests.Tasks.CreateTask;

public class CreateTaskInputValidatorTests
{
    private readonly CreateTaskInputValidator _validator = new();

    [Fact]
    public void Validate_WithValidInput_ShouldPass()
    {
        // Arrange
        var input = new CreateTaskInput("Test Title", "Test Description", DomainTaskStatus.Pending, null);

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyTitle_ShouldFail()
    {
        // Arrange
        var input = new CreateTaskInput("", null, DomainTaskStatus.Pending, null);

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_WithTitleExceedingMaxLength_ShouldFail()
    {
        // Arrange
        var longTitle = new string('a', 201);
        var input = new CreateTaskInput(longTitle, null, DomainTaskStatus.Pending, null);

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_WithDescriptionExceedingMaxLength_ShouldFail()
    {
        // Arrange
        var longDescription = new string('a', 2001);
        var input = new CreateTaskInput("Title", longDescription, DomainTaskStatus.Pending, null);

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_WithNullDescription_ShouldPass()
    {
        // Arrange
        var input = new CreateTaskInput("Title", null, DomainTaskStatus.Pending, null);

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
