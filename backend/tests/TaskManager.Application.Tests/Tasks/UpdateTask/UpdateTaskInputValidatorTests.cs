using FluentValidation.TestHelper;
using Shouldly;
using DomainTaskStatus = TaskManager.Domain.Entities.TaskStatus;
using TaskManager.Application.Tasks.UpdateTask;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Tests.Tasks.UpdateTask;

public class UpdateTaskInputValidatorTests
{
    private readonly UpdateTaskInputValidator _validator = new();

    [Fact]
    public void Validate_WithValidInput_ShouldPass()
    {
        // Arrange
        var input = new UpdateTaskInput(Guid.NewGuid(), "Test Title", "Test Description", DomainTaskStatus.Pending, null);

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyTitle_ShouldFail()
    {
        // Arrange
        var input = new UpdateTaskInput(Guid.NewGuid(), "", null, DomainTaskStatus.Pending, null);

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
        var input = new UpdateTaskInput(Guid.NewGuid(), longTitle, null, DomainTaskStatus.Pending, null);

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
        var input = new UpdateTaskInput(Guid.NewGuid(), "Title", longDescription, DomainTaskStatus.Pending, null);

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
