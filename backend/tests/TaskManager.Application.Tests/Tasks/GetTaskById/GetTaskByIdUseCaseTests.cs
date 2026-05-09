using NSubstitute;
using Shouldly;
using DomainTaskStatus = TaskManager.Domain.Entities.TaskStatus;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Tasks.GetTaskById;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions.Tasks;

namespace TaskManager.Application.Tests.Tasks.GetTaskById;

public class GetTaskByIdUseCaseTests
{
    private readonly ITaskRepository _repositorySubstitute = Substitute.For<ITaskRepository>();
    private readonly ICurrentUserService _currentUserSubstitute = Substitute.For<ICurrentUserService>();
    private readonly GetTaskByIdUseCase _useCase;

    public GetTaskByIdUseCaseTests()
    {
        _useCase = new GetTaskByIdUseCase(_repositorySubstitute, _currentUserSubstitute);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTaskExistsAndIsOwned_ReturnsDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var task = TaskEntity.Create("Test Task", null, DomainTaskStatus.Pending, null, userId);
        _repositorySubstitute.GetByIdAsync(taskId, Arg.Any<CancellationToken>()).Returns(task);
        _currentUserSubstitute.UserId.Returns(userId);

        // Act
        var result = await _useCase.ExecuteAsync(new GetTaskByIdInput(taskId), CancellationToken.None);

        // Assert
        result.Id.ShouldBe(task.Id);
        result.Title.ShouldBe("Test Task");
    }

    [Fact]
    public async Task ExecuteAsync_WhenTaskDoesNotExist_ThrowsTaskNotFoundException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        _repositorySubstitute.GetByIdAsync(taskId, Arg.Any<CancellationToken>()).Returns((TaskEntity?)null);

        // Act & Assert
        var ex = await Should.ThrowAsync<TaskNotFoundException>(() =>
            _useCase.ExecuteAsync(new GetTaskByIdInput(taskId), CancellationToken.None));
        ex.TaskId.ShouldBe(taskId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTaskBelongsToAnotherUser_ThrowsTaskAccessForbiddenException()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var task = TaskEntity.Create("Test Task", null, DomainTaskStatus.Pending, null, ownerId);
        _repositorySubstitute.GetByIdAsync(taskId, Arg.Any<CancellationToken>()).Returns(task);
        _currentUserSubstitute.UserId.Returns(currentUserId);

        // Act & Assert
        var ex = await Should.ThrowAsync<TaskAccessForbiddenException>(() =>
            _useCase.ExecuteAsync(new GetTaskByIdInput(taskId), CancellationToken.None));
        ex.TaskId.ShouldBe(taskId);
    }
}
