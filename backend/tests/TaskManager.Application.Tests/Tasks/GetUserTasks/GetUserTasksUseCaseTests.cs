using FluentValidation;
using NSubstitute;
using Shouldly;
using DomainTaskStatus = TaskManager.Domain.Entities.TaskStatus;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Tasks.GetUserTasks;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Tests.Tasks.GetUserTasks;

public class GetUserTasksUseCaseTests
{
    private readonly IValidator<GetUserTasksInput> _validator = new GetUserTasksInputValidator();
    private readonly ITaskRepository _repositorySubstitute = Substitute.For<ITaskRepository>();
    private readonly ICurrentUserService _currentUserSubstitute = Substitute.For<ICurrentUserService>();
    private readonly PaginationSettings _paginationSettings = new() { PageSize = 10 };
    private readonly GetUserTasksUseCase _useCase;

    public GetUserTasksUseCaseTests()
    {
        _useCase = new GetUserTasksUseCase(_validator, _repositorySubstitute, _currentUserSubstitute, _paginationSettings);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidPage_ReturnsMappedDtos()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var task = TaskEntity.Create("Task 1", null, DomainTaskStatus.Pending, null, userId);
        var paginatedResult = new PaginatedResult<TaskEntity>(new[] { task }.ToList(), 1, 10, 1);
        _repositorySubstitute.GetByUserIdPagedAsync(userId, 1, 10, Arg.Any<CancellationToken>())
            .Returns(paginatedResult);
        _currentUserSubstitute.UserId.Returns(userId);

        // Act
        var result = await _useCase.ExecuteAsync(new GetUserTasksInput(1), CancellationToken.None);

        // Assert
        result.Items.Count.ShouldBe(1);
        result.Items[0].Title.ShouldBe("Task 1");
        result.Page.ShouldBe(1);
        result.PageSize.ShouldBe(10);
        result.TotalCount.ShouldBe(1);
    }

    [Fact]
    public async Task ExecuteAsync_PassesCurrentUserIdToRepo()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var paginatedResult = new PaginatedResult<TaskEntity>(new List<TaskEntity>(), 1, 10, 0);
        _repositorySubstitute.GetByUserIdPagedAsync(userId, 1, 10, Arg.Any<CancellationToken>())
            .Returns(paginatedResult);
        _currentUserSubstitute.UserId.Returns(userId);

        // Act
        await _useCase.ExecuteAsync(new GetUserTasksInput(1), CancellationToken.None);

        // Assert
        await _repositorySubstitute.Received(1).GetByUserIdPagedAsync(userId, 1, 10, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidPage_ThrowsValidationException()
    {
        // Arrange
        var input = new GetUserTasksInput(0);

        // Act & Assert
        await Should.ThrowAsync<ValidationException>(() =>
            _useCase.ExecuteAsync(input, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_WhenRepoReturnsEmpty_ReturnsEmptyItems_WithPreservedPaging()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var paginatedResult = new PaginatedResult<TaskEntity>(new List<TaskEntity>(), 1, 10, 0);
        _repositorySubstitute.GetByUserIdPagedAsync(userId, 1, 10, Arg.Any<CancellationToken>())
            .Returns(paginatedResult);
        _currentUserSubstitute.UserId.Returns(userId);

        // Act
        var result = await _useCase.ExecuteAsync(new GetUserTasksInput(1), CancellationToken.None);

        // Assert
        result.Items.ShouldBeEmpty();
        result.TotalCount.ShouldBe(0);
        result.Page.ShouldBe(1);
        result.PageSize.ShouldBe(10);
    }
}
