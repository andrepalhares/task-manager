using FluentValidation;
using NSubstitute;
using Shouldly;
using DomainTaskStatus = TaskManager.Domain.Entities.TaskStatus;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Tasks.CreateTask;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Tests.Tasks.CreateTask;

public class CreateTaskUseCaseTests
{
    private readonly IValidator<CreateTaskInput> _validator = new CreateTaskInputValidator();
    private readonly ITaskRepository _repositorySubstitute = Substitute.For<ITaskRepository>();
    private readonly ICurrentUserService _currentUserSubstitute = Substitute.For<ICurrentUserService>();
    private readonly CreateTaskUseCase _useCase;

    public CreateTaskUseCaseTests()
    {
        _useCase = new CreateTaskUseCase(_validator, _repositorySubstitute, _currentUserSubstitute);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidInput_PersistsTask_AndReturnsDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var input = new CreateTaskInput("Test Title", "Test Description", DomainTaskStatus.Pending, null);
        _currentUserSubstitute.UserId.Returns(userId);

        // Act
        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        // Assert
        result.Title.ShouldBe("Test Title");
        result.Description.ShouldBe("Test Description");
        result.Status.ShouldBe(DomainTaskStatus.Pending);
        result.UserId.ShouldBe(userId);
        await _repositorySubstitute.Received(1).AddAsync(Arg.Any<TaskItem>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_AssignsCallerAsOwner()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var input = new CreateTaskInput("Title", null, DomainTaskStatus.Pending, null);
        _currentUserSubstitute.UserId.Returns(userId);
        TaskItem capturedTask = null!;
        await _repositorySubstitute.AddAsync(Arg.Do<TaskItem>(t => capturedTask = t), Arg.Any<CancellationToken>());

        // Act
        await _useCase.ExecuteAsync(input, CancellationToken.None);

        // Assert
        capturedTask.UserId.ShouldBe(userId);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidInput_ThrowsValidationException()
    {
        // Arrange
        var input = new CreateTaskInput("", null, DomainTaskStatus.Pending, null);

        // Act & Assert
        await Should.ThrowAsync<ValidationException>(() => _useCase.ExecuteAsync(input, CancellationToken.None));
    }
}
