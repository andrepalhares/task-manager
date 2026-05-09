using NSubstitute;
using Shouldly;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Tasks.GetTaskById;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions.Tasks;
using DomainTaskStatus = TaskManager.Domain.Entities.TaskStatus;

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
        var userId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var task = TaskEntity.Create("Test Task", null, DomainTaskStatus.Pending, null, userId);
        _repositorySubstitute.GetByIdAsync(taskId, Arg.Any<CancellationToken>()).Returns(task);
        _currentUserSubstitute.UserId.Returns(userId);

        var result = await _useCase.ExecuteAsync(new GetTaskByIdInput(taskId), CancellationToken.None);

        result.Id.ShouldBe(task.Id);
        result.Title.ShouldBe("Test Task");
    }

    [Fact]
    public async Task ExecuteAsync_WhenTaskDoesNotExist_ThrowsTaskNotFoundException()
    {
        var taskId = Guid.NewGuid();
        _repositorySubstitute.GetByIdAsync(taskId, Arg.Any<CancellationToken>()).Returns((TaskEntity?)null);

        var ex = await Should.ThrowAsync<TaskNotFoundException>(() =>
            _useCase.ExecuteAsync(new GetTaskByIdInput(taskId), CancellationToken.None));
        ex.TaskId.ShouldBe(taskId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTaskBelongsToAnotherUser_ThrowsTaskAccessForbiddenException()
    {
        var ownerId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var task = TaskEntity.Create("Test Task", null, DomainTaskStatus.Pending, null, ownerId);
        _repositorySubstitute.GetByIdAsync(taskId, Arg.Any<CancellationToken>()).Returns(task);
        _currentUserSubstitute.UserId.Returns(currentUserId);

        var ex = await Should.ThrowAsync<TaskAccessForbiddenException>(() =>
            _useCase.ExecuteAsync(new GetTaskByIdInput(taskId), CancellationToken.None));
        ex.TaskId.ShouldBe(taskId);
    }
}
