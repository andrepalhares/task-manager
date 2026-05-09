using NSubstitute;
using Shouldly;
using DomainTaskStatus = TaskManager.Domain.Entities.TaskStatus;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Tasks.DeleteTask;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions.Tasks;

namespace TaskManager.Application.Tests.Tasks.DeleteTask;

public class DeleteTaskUseCaseTests
{
    private readonly ITaskRepository _repositorySubstitute = Substitute.For<ITaskRepository>();
    private readonly ICurrentUserService _currentUserSubstitute = Substitute.For<ICurrentUserService>();
    private readonly DeleteTaskUseCase _useCase;

    public DeleteTaskUseCaseTests()
    {
        _useCase = new DeleteTaskUseCase(_repositorySubstitute, _currentUserSubstitute);
    }

    [Fact]
    public async Task ExecuteAsync_WhenOwned_DeletesTask()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var task = TaskEntity.Create("Task", null, DomainTaskStatus.Pending, null, userId);
        _repositorySubstitute.GetByIdAsync(taskId, Arg.Any<CancellationToken>()).Returns(task);
        _currentUserSubstitute.UserId.Returns(userId);

        // Act
        await _useCase.ExecuteAsync(new DeleteTaskInput(taskId), CancellationToken.None);

        // Assert
        await _repositorySubstitute.Received(1).DeleteAsync(taskId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenNotFound_ThrowsTaskNotFoundException_AndDoesNotCallDelete()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        _repositorySubstitute.GetByIdAsync(taskId, Arg.Any<CancellationToken>()).Returns((TaskEntity?)null);

        // Act & Assert
        var ex = await Should.ThrowAsync<TaskNotFoundException>(() =>
            _useCase.ExecuteAsync(new DeleteTaskInput(taskId), CancellationToken.None));
        ex.TaskId.ShouldBe(taskId);
        await _repositorySubstitute.DidNotReceive().DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenForbidden_ThrowsTaskAccessForbiddenException_AndDoesNotCallDelete()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var task = TaskEntity.Create("Task", null, DomainTaskStatus.Pending, null, ownerId);
        _repositorySubstitute.GetByIdAsync(taskId, Arg.Any<CancellationToken>()).Returns(task);
        _currentUserSubstitute.UserId.Returns(currentUserId);

        // Act & Assert
        var ex = await Should.ThrowAsync<TaskAccessForbiddenException>(() =>
            _useCase.ExecuteAsync(new DeleteTaskInput(taskId), CancellationToken.None));
        ex.TaskId.ShouldBe(taskId);
        await _repositorySubstitute.DidNotReceive().DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}
