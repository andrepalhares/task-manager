using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Shouldly;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Tasks.CreateTask;
using TaskManager.Application.Tasks.DeleteTask;
using TaskManager.Application.Tasks.GetTaskById;
using TaskManager.Application.Tasks.GetUserTasks;
using TaskManager.Application.Tasks.UpdateTask;
using DomainTaskStatus = TaskManager.Domain.Entities.TaskStatus;
using TaskManager.WebApi.Tasks;

namespace TaskManager.WebApi.Tests.Tasks;

public class TasksControllerTests
{
    private readonly IUseCase<CreateTaskInput, CreateTaskOutput> _create
        = Substitute.For<IUseCase<CreateTaskInput, CreateTaskOutput>>();
    private readonly IUseCase<GetTaskByIdInput, GetTaskByIdOutput> _getById
        = Substitute.For<IUseCase<GetTaskByIdInput, GetTaskByIdOutput>>();
    private readonly IUseCase<GetUserTasksInput, GetUserTasksOutput> _getUserTasks
        = Substitute.For<IUseCase<GetUserTasksInput, GetUserTasksOutput>>();
    private readonly IUseCase<UpdateTaskInput, UpdateTaskOutput> _update
        = Substitute.For<IUseCase<UpdateTaskInput, UpdateTaskOutput>>();
    private readonly IUseCase<DeleteTaskInput, DeleteTaskOutput> _delete
        = Substitute.For<IUseCase<DeleteTaskInput, DeleteTaskOutput>>();

    private TasksController CreateSut()
    {
        var controller = new TasksController(_create, _getById, _getUserTasks, _update, _delete);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        return controller;
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreatedAtActionWithTaskId()
    {
        var request = new CreateTaskRequest("Title", "Desc", "Pending", null);
        var output = new CreateTaskOutput(Guid.NewGuid(), "Title", "Desc", "Pending", null, Guid.NewGuid());
        _create.ExecuteAsync(Arg.Any<CreateTaskInput>(), Arg.Any<CancellationToken>()).Returns(output);

        var result = await CreateSut().CreateAsync(request, CancellationToken.None);

        var created = result.ShouldBeOfType<CreatedAtActionResult>();
        created.Value.ShouldBe(output);
        created.RouteValues!["id"].ShouldBe(output.Id);
        created.ActionName.ShouldBe("GetById");

        await _create.Received(1).ExecuteAsync(
            Arg.Is<CreateTaskInput>(i =>
                i.Title == "Title"
                && i.Description == "Desc"
                && i.Status == DomainTaskStatus.Pending),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetById_ReturnsOkWithUseCaseOutput()
    {
        var taskId = Guid.NewGuid();
        var output = new GetTaskByIdOutput(taskId, "Title", null, "Pending", null, Guid.NewGuid());
        _getById
            .ExecuteAsync(Arg.Is<GetTaskByIdInput>(i => i.TaskId == taskId), Arg.Any<CancellationToken>())
            .Returns(output);

        var result = await CreateSut().GetById(taskId, CancellationToken.None);

        var ok = result.ShouldBeOfType<OkObjectResult>();
        ok.Value.ShouldBe(output);
    }

    [Fact]
    public async Task ListAsync_PassesPageToUseCaseAndReturnsOk()
    {
        var output = new GetUserTasksOutput(new List<TaskItemOutput>(), Page: 3, PageSize: 10, TotalCount: 0);
        _getUserTasks
            .ExecuteAsync(Arg.Is<GetUserTasksInput>(i => i.Page == 3), Arg.Any<CancellationToken>())
            .Returns(output);

        var result = await CreateSut().ListAsync(page: 3, CancellationToken.None);

        var ok = result.ShouldBeOfType<OkObjectResult>();
        ok.Value.ShouldBe(output);
    }

    [Fact]
    public async Task ListAsync_DefaultsPageToOne_WhenOmitted()
    {
        var output = new GetUserTasksOutput(new List<TaskItemOutput>(), 1, 10, 0);
        _getUserTasks
            .ExecuteAsync(Arg.Is<GetUserTasksInput>(i => i.Page == 1), Arg.Any<CancellationToken>())
            .Returns(output);

        var result = await CreateSut().ListAsync();

        result.ShouldBeOfType<OkObjectResult>();
        await _getUserTasks.Received(1).ExecuteAsync(
            Arg.Is<GetUserTasksInput>(i => i.Page == 1),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_PassesRouteIdToInput_AndReturnsOk()
    {
        var taskId = Guid.NewGuid();
        var request = new UpdateTaskRequest("New", "New desc", "Completed", null);
        var output = new UpdateTaskOutput(taskId, "New", "New desc", "Completed", null, Guid.NewGuid());
        _update.ExecuteAsync(Arg.Any<UpdateTaskInput>(), Arg.Any<CancellationToken>()).Returns(output);

        var result = await CreateSut().UpdateAsync(taskId, request, CancellationToken.None);

        var ok = result.ShouldBeOfType<OkObjectResult>();
        ok.Value.ShouldBe(output);

        await _update.Received(1).ExecuteAsync(
            Arg.Is<UpdateTaskInput>(i =>
                i.TaskId == taskId
                && i.Title == "New"
                && i.Status == DomainTaskStatus.Completed),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_DelegatesToUseCaseAndReturnsNoContent()
    {
        var taskId = Guid.NewGuid();
        _delete
            .ExecuteAsync(Arg.Any<DeleteTaskInput>(), Arg.Any<CancellationToken>())
            .Returns(new DeleteTaskOutput());

        var result = await CreateSut().DeleteAsync(taskId, CancellationToken.None);

        result.ShouldBeOfType<NoContentResult>();
        await _delete.Received(1).ExecuteAsync(
            Arg.Is<DeleteTaskInput>(i => i.TaskId == taskId),
            Arg.Any<CancellationToken>());
    }
}
