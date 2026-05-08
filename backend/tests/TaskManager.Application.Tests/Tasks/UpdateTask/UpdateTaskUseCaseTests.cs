using FluentValidation;
using NSubstitute;
using Shouldly;
using DomainTaskStatus = TaskManager.Domain.Entities.TaskStatus;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Tasks.UpdateTask;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions.Tasks;

namespace TaskManager.Application.Tests.Tasks.UpdateTask;

public class UpdateTaskUseCaseTests
{
    private readonly IValidator<UpdateTaskInput> _validator = new UpdateTaskInputValidator();
    private readonly ITaskRepository _repositorySubstitute = Substitute.For<ITaskRepository>();
    private readonly ICurrentUserService _currentUserSubstitute = Substitute.For<ICurrentUserService>();
    private readonly UpdateTaskUseCase _useCase;

    public UpdateTaskUseCaseTests()
    {
        _useCase = new UpdateTaskUseCase(_validator, _repositorySubstitute, _currentUserSubstitute);
    }

    [Fact]
    public async Task ExecuteAsync_WhenOwned_UpdatesTask_AndReturnsUpdatedDto()
    {
        var userId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var task = TaskItem.Create("Original Title", "Original Description", DomainTaskStatus.Pending, null, userId);
        var input = new UpdateTaskInput(taskId, "Updated Title", "Updated Description", DomainTaskStatus.InProgress, null);
        _repositorySubstitute.GetByIdAsync(taskId, Arg.Any<CancellationToken>()).Returns(task);
        _currentUserSubstitute.UserId.Returns(userId);

        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        result.Title.ShouldBe("Updated Title");
        result.Description.ShouldBe("Updated Description");
        result.Status.ShouldBe(DomainTaskStatus.InProgress.ToString());
        await _repositorySubstitute.Received(1).UpdateAsync(Arg.Any<TaskItem>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenTaskDoesNotExist_ThrowsTaskNotFoundException()
    {
        var taskId = Guid.NewGuid();
        var input = new UpdateTaskInput(taskId, "Title", null, DomainTaskStatus.Pending, null);
        _repositorySubstitute.GetByIdAsync(taskId, Arg.Any<CancellationToken>()).Returns((TaskItem?)null);

        var ex = await Should.ThrowAsync<TaskNotFoundException>(() =>
            _useCase.ExecuteAsync(input, CancellationToken.None));
        ex.TaskId.ShouldBe(taskId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTaskBelongsToAnotherUser_ThrowsTaskAccessForbiddenException()
    {
        var ownerId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var task = TaskItem.Create("Title", null, DomainTaskStatus.Pending, null, ownerId);
        var input = new UpdateTaskInput(taskId, "Updated", null, DomainTaskStatus.Pending, null);
        _repositorySubstitute.GetByIdAsync(taskId, Arg.Any<CancellationToken>()).Returns(task);
        _currentUserSubstitute.UserId.Returns(currentUserId);

        var ex = await Should.ThrowAsync<TaskAccessForbiddenException>(() =>
            _useCase.ExecuteAsync(input, CancellationToken.None));
        ex.TaskId.ShouldBe(taskId);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidInput_ThrowsValidationException()
    {
        var input = new UpdateTaskInput(Guid.NewGuid(), "", null, DomainTaskStatus.Pending, null);

        await Should.ThrowAsync<ValidationException>(() =>
            _useCase.ExecuteAsync(input, CancellationToken.None));
    }
}
